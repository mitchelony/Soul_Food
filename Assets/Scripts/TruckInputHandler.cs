using UnityEngine;
using UnityEngine.InputSystem;

public class TruckInputHandler : MonoBehaviour
{
    TruckControllerScript truckController;

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    private InputAction moveAction;

    void Awake()
    {
        truckController = GetComponent<TruckControllerScript>();

        // Get the move action from the Input Actions asset
        if (inputActions != null)
        {
            moveAction = inputActions.FindAction("Move");
        }
    }

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
    }

    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        if (moveAction != null)
        {
            // Read the move input as Vector2
            inputVector = moveAction.ReadValue<Vector2>();
        }

        truckController.SetInputVector(inputVector);
    }
}
