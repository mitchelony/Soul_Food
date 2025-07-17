using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private float holdTime = 2f;  // Time to hold before dragging starts
    [SerializeField] private Canvas canvas; // Assign your main UI Canvas here

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    private bool isHolding = false;
    private float holdTimer = 0f;
    private bool isDraggable = false;

    private Vector2 originalPosition;  // To store the original position for respawning

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        // Make sure to store the original anchored position for UI elements
        originalPosition = rectTransform.anchoredPosition; 
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime && !isDraggable)
            {
                isDraggable = true;
                Debug.Log("Object is now draggable!");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
        Debug.Log("Pointer Down: Holding...");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        // If the hold time wasn't completed, ensure the object snaps back if it moved slightly
        // This is important if you want it to not move at all unless fully draggable
        if (!isDraggable)
        {
            rectTransform.anchoredPosition = originalPosition;
            Debug.Log("Hold time not completed, resetting position.");
        }
        Debug.Log("Pointer Up: Hold released.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            Debug.Log("Begin Drag");
            // This makes the object transparent to raycasts so you can drop it on objects behind it
            canvasGroup.blocksRaycasts = false; 
            canvasGroup.alpha = 0.6f; // Make it slightly transparent while dragging
        }
        else
        {
            // If not draggable, cancel the drag immediately
            eventData.pointerDrag = null; 
            Debug.Log("Hold not long enough, cannot drag.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            // Move the UI element with the pointer, accounting for canvas scaling
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            Debug.Log("On Drag");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset properties after dragging ends
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f; // Make it fully opaque again
        Debug.Log("End Drag");

        // If the item wasn't successfully dropped on a valid target (e.g., customer),
        // we might want it to return to its original position here.
        // However, since `RatingSystem` will call `ResetObjectPosition`, 
        // this can be conditional or removed if `RatingSystem` handles all resets.
        // For simplicity, let's assume `RatingSystem` is the primary trigger for reset.
        // If you want it to snap back *always* on end drag unless successfully dropped,
        // you'd add logic here to check if it was dropped on a valid target.
        // For now, `RatingSystem` is responsible for calling `ResetObjectPosition`.
    }

    // Public method to reset the UI element's position
    public void ResetObjectPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        isDraggable = false;  // Reset draggable state for the next interaction
        isHolding = false; // Ensure holding is false too
        holdTimer = 0f; // Reset hold timer
        Debug.Log("Object has been reset to its original position.");
    }
}