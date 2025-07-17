using UnityEngine;

public class TruckControllerScript : MonoBehaviour
{
    [Header("Truck Settings")]
    public float driftFactor = 0.1f; // Reduced from 0.5f for less sliding
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float brakeFactor = 50.0f; // New: How strong the brakes are
    public float frictionFactor = 10.0f; // New: General friction when not accelerating
    public float minSpeedBeforeAllowTurn = 0.4f; // Minimum speed required to turn

    float accelerationInput = 0;
    float steeringInput = 0;
    float lastSteeringInput = 0; // Track previous steering input
    float steeringHoldTime = 0; // Track how long steering is held
    bool hasCompletedFirstTurn = false; // Track if first 45-degree turn is done

    public float rotationAngle = 0;
    private float targetRotationAngle = 0; // Target angle for smooth rotation

    private bool canMove = true; // Track if truck can move
    private bool canMoveForward = true; // Track if truck can move forward
    private Vector3 lastCollisionNormal = Vector3.zero; // Store collision direction

    Rigidbody2D truckRigidBody2D;

    void Awake()
    {
        truckRigidBody2D = GetComponent<Rigidbody2D>();

        // Set up physics constraints for 2D top-down movement
        truckRigidBody2D.gravityScale = 0; // Disable gravity
        truckRigidBody2D.freezeRotation = true; // Prevent physics rotation (we handle rotation manually)
        truckRigidBody2D.linearDamping = 2f; // Add some damping to prevent drift
        truckRigidBody2D.angularDamping = 5f; // Prevent unwanted spinning

        // Enable continuous collision detection for EdgeCollider2D detection
        truckRigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize rotation angles to current transform rotation
        rotationAngle = transform.eulerAngles.z;
        targetRotationAngle = rotationAngle;

        // Snap to nearest 45-degree angle at start (since we now use 45-degree turns)
        targetRotationAngle = Mathf.Round(rotationAngle / 45f) * 45f;
        rotationAngle = targetRotationAngle;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        Debug.Log($"Truck starting rotation: {rotationAngle} degrees");

        // Debug truck setup
        Debug.Log($"Truck has Rigidbody2D: {GetComponent<Rigidbody2D>() != null}");
        Debug.Log($"Truck has Collider2D: {GetComponent<Collider2D>() != null}");
        Debug.Log($"Truck layer: {gameObject.layer}");
        Debug.Log($"Truck tag: {gameObject.tag}");

        // Find and debug road objects
        GameObject[] roadObjects = GameObject.FindGameObjectsWithTag("Road");
        Debug.Log($"Found {roadObjects.Length} objects with 'Road' tag");
        foreach (GameObject road in roadObjects)
        {
            Collider2D[] colliders = road.GetComponents<Collider2D>();
            Debug.Log($"Road object '{road.name}' - Layer: {road.layer}, Has {colliders.Length} colliders");

            foreach (Collider2D col in colliders)
            {
                if (col is BoxCollider2D)
                {
                    Debug.Log($"  - BoxCollider2D: IsTrigger={col.isTrigger}, Enabled={col.enabled}");
                }
                else if (col is EdgeCollider2D edge)
                {
                    Debug.Log($"  - EdgeCollider2D: IsTrigger={col.isTrigger}, Enabled={col.enabled}, Points={edge.points.Length}");
                    if (edge.points.Length < 2)
                    {
                        Debug.LogError($"EdgeCollider2D on {road.name} has insufficient points! Needs at least 2 points.");
                    }
                }
                else if (col is PolygonCollider2D)
                {
                    Debug.Log($"  - PolygonCollider2D: IsTrigger={col.isTrigger}, Enabled={col.enabled}");
                }
                else
                {
                    Debug.Log($"  - {col.GetType().Name}: IsTrigger={col.isTrigger}, Enabled={col.enabled}");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplyBrakingAndFriction();
        killOrthogonalVelocity();
        ApplySteeringForce();
    }

    void ApplyEngineForce()
    {
        if (canMove)
        {
            // Check if we're trying to move forward (positive input) and if forward movement is blocked
            bool isMovingForward = accelerationInput > 0;
            bool isMovingBackward = accelerationInput < 0;

            // Allow movement if:
            // 1. We can move forward and we're trying to move forward, OR
            // 2. We're trying to move backward (always allowed even when hitting boundaries)
            if ((canMoveForward && isMovingForward) || isMovingBackward)
            {
                Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;
                truckRigidBody2D.AddForce(engineForceVector, ForceMode2D.Force);
            }
            else if (isMovingForward && !canMoveForward)
            {
                Debug.Log("Forward movement blocked by road boundary - try reversing");
            }
        }
    }

    void ApplyBrakingAndFriction()
    {
        // Apply strong friction when not accelerating or when braking
        if (Mathf.Abs(accelerationInput) < 0.1f || !canMove)
        {
            // Apply friction to slow down the truck
            Vector2 frictionForce = -truckRigidBody2D.linearVelocity.normalized * frictionFactor;
            truckRigidBody2D.AddForce(frictionForce, ForceMode2D.Force);

            // If moving very slowly, stop completely and lock position
            if (truckRigidBody2D.linearVelocity.magnitude < 0.5f)
            {
                truckRigidBody2D.linearVelocity = Vector2.zero;
                // Prevent any tiny movements when stopped
                truckRigidBody2D.angularVelocity = 0;
            }
        }

        // Extra braking when reversing direction
        if (accelerationInput < -0.1f && Vector2.Dot(truckRigidBody2D.linearVelocity, transform.up) > 0)
        {
            Vector2 brakeForce = -truckRigidBody2D.linearVelocity.normalized * brakeFactor;
            truckRigidBody2D.AddForce(brakeForce, ForceMode2D.Force);
        }
        else if (accelerationInput > 0.1f && Vector2.Dot(truckRigidBody2D.linearVelocity, transform.up) < 0)
        {
            Vector2 brakeForce = -truckRigidBody2D.linearVelocity.normalized * brakeFactor;
            truckRigidBody2D.AddForce(brakeForce, ForceMode2D.Force);
        }
    }

    void ApplySteeringForce()
    {
        // Check if truck is moving fast enough to turn
        bool canTurn = truckRigidBody2D.linearVelocity.magnitude >= minSpeedBeforeAllowTurn;

        if (canTurn)
        {
            // Check for new steering input (when key is first pressed)
            if (Mathf.Abs(steeringInput) > 0.1f && Mathf.Abs(lastSteeringInput) < 0.1f)
            {
                // First press - turn 45 degrees
                if (steeringInput < 0) // Left turn
                {
                    targetRotationAngle += 45f;
                }
                else // Right turn
                {
                    targetRotationAngle -= 45f;
                }

                hasCompletedFirstTurn = false;
                steeringHoldTime = 0;

                // Keep angle within 0-360 range
                targetRotationAngle = targetRotationAngle % 360f;
                if (targetRotationAngle < 0) targetRotationAngle += 360f;
            }
            // Check if key is being held down
            else if (Mathf.Abs(steeringInput) > 0.1f && Mathf.Abs(lastSteeringInput) > 0.1f)
            {
                steeringHoldTime += Time.fixedDeltaTime;

                // If held for 0.3 seconds and first turn is complete, do second 45-degree turn
                if (steeringHoldTime >= 0.3f && !hasCompletedFirstTurn)
                {
                    if (steeringInput < 0) // Left turn
                    {
                        targetRotationAngle += 45f;
                    }
                    else // Right turn
                    {
                        targetRotationAngle -= 45f;
                    }

                    hasCompletedFirstTurn = true;

                    // Keep angle within 0-360 range
                    targetRotationAngle = targetRotationAngle % 360f;
                    if (targetRotationAngle < 0) targetRotationAngle += 360f;
                }
            }
            // Reset when key is released
            else if (Mathf.Abs(steeringInput) < 0.1f)
            {
                steeringHoldTime = 0;
                hasCompletedFirstTurn = false;
            }
        }

        // Smoothly rotate to target angle
        rotationAngle = Mathf.LerpAngle(rotationAngle, targetRotationAngle, Time.fixedDeltaTime * 8f);
        truckRigidBody2D.MoveRotation(rotationAngle);

        // Update last steering input for next frame
        lastSteeringInput = steeringInput;
    }

    void killOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(truckRigidBody2D.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(truckRigidBody2D.linearVelocity, transform.right);
        truckRigidBody2D.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
        truckRigidBody2D.angularVelocity = 0; // Reset angular velocity to prevent spinning
    }
    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string colliderType = other.GetType().Name;
        Debug.Log($"Truck triggered by: {other.gameObject.name} with tag '{other.tag}' on layer {other.gameObject.layer} - Collider Type: {colliderType}");
        if (other.CompareTag("Road"))
        {
            Debug.Log($"Hit road boundary ({colliderType}) - blocking forward movement only");
            canMoveForward = false; // Block forward movement only
        }
        // Ignore Border tag - that's only for camera
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string colliderType = other.GetType().Name;
        Debug.Log($"Truck exited trigger with: {other.gameObject.name} with tag '{other.tag}' - Collider Type: {colliderType}");
        if (other.CompareTag("Road"))
        {
            Debug.Log($"Left road boundary ({colliderType}) - allowing all movement");
            canMoveForward = true; // Allow forward movement again
            canMove = true; // Ensure general movement is allowed
        }
        // Ignore Border tag - that's only for camera
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string colliderType = collision.collider.GetType().Name;
        Debug.Log($"Truck collided with: {collision.gameObject.name} with tag '{collision.gameObject.tag}' on layer {collision.gameObject.layer} - Collider Type: {colliderType}");
        if (collision.gameObject.CompareTag("Road"))
        {
            Debug.Log($"Collided with road boundary ({colliderType}) - blocking forward movement only");
            canMoveForward = false; // Block forward movement only
            // Optional: Push truck back slightly
            Vector2 pushDirection = -collision.contacts[0].normal;
            truckRigidBody2D.AddForce(pushDirection * 5f, ForceMode2D.Impulse);
        }
        // Ignore Border tag - that's only for camera
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        string colliderType = collision.collider.GetType().Name;
        Debug.Log($"Truck stopped colliding with: {collision.gameObject.name} with tag '{collision.gameObject.tag}' - Collider Type: {colliderType}");
        if (collision.gameObject.CompareTag("Road"))
        {
            Debug.Log($"Stopped colliding with road boundary ({colliderType}) - allowing all movement");
            canMoveForward = true; // Allow forward movement again
            canMove = true; // Ensure general movement is allowed
        }
        // Ignore Border tag - that's only for camera
    }
}
