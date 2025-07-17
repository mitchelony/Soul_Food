using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;
    private BoxCollider2D borderCollider;
    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the border object on the CameraBorder layer
        GameObject[] borderObjects = GameObject.FindGameObjectsWithTag("Border");
        foreach (GameObject border in borderObjects)
        {
            if (border.layer == LayerMask.NameToLayer("CameraBorder"))
            {
                borderCollider = border.GetComponent<BoxCollider2D>();
                break;
            }
        }

        if (borderCollider == null)
        {
            Debug.LogWarning("CameraBorder not found! Make sure there's an object with 'Border' tag on 'CameraBorder' layer.");
        }

        // Get the camera component
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Truck");
        if (target == null)
        {
            Debug.LogWarning("Target not found! Make sure the player has the 'Truck' tag.");
            return;
        }

        // Calculate desired camera position
        Vector3 desiredPosition = new Vector3(target.transform.position.x, target.transform.position.y, -10);

        // Clamp camera position to border bounds if border exists
        if (borderCollider != null && cam != null)
        {
            // Calculate camera bounds
            float cameraHalfHeight = cam.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * cam.aspect;

            // Get border bounds
            Bounds borderBounds = borderCollider.bounds;

            // Clamp camera position to stay within border
            float clampedX = Mathf.Clamp(desiredPosition.x,
                borderBounds.min.x + cameraHalfWidth,
                borderBounds.max.x - cameraHalfWidth);

            float clampedY = Mathf.Clamp(desiredPosition.y,
                borderBounds.min.y + cameraHalfHeight,
                borderBounds.max.y - cameraHalfHeight);

            transform.position = new Vector3(clampedX, clampedY, -10);
        }
        else
        {
            // Fallback to original behavior if no border
            transform.position = desiredPosition;
        }
    }
}
