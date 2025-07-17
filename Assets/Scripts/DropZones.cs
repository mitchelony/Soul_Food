using UnityEngine;

public class DropZones : MonoBehaviour
{
    public GameObject dropIndicator; // Reference to the child object (image/sprite)
    public float showDistance = 3f;   // How close the player needs to be

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Truck").transform;
        if (dropIndicator != null)
            dropIndicator.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Truck") && dropIndicator != null)
            dropIndicator.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Truck") && dropIndicator != null)
            dropIndicator.SetActive(false);
    }

}
