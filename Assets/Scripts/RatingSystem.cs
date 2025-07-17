using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class RatingSystem : MonoBehaviour, IDropHandler
{
    int customersServed = 0;
    float overallRating = 5.0f;
    float ratingReceived = 0.0f;

    public TMP_Text ratingText; 

    void Awake()
    {
        if (ratingText == null)
        {
            ratingText = GetComponentInChildren<TMP_Text>();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragAndDrop draggedItem = eventData.pointerDrag.GetComponent<DragAndDrop>();

        if (draggedItem != null)
        {
            // Increment customers served and get a random rating
            customersServed += 1;
            ratingReceived = Random.Range(2.0f, 5.0f);
            
            UpdateOverallRating(ratingReceived);

            UpdateUI();
            //Resets position
            draggedItem.ResetObjectPosition(); 
        }
    }

    private void UpdateOverallRating(float newRating)
    {
        if (customersServed == 1)
        {
            overallRating = newRating;
        }
        else
        {
            overallRating = ((overallRating * (customersServed - 1)) + newRating) / customersServed;
        }
    }

    private void UpdateUI()
    {
        if (ratingText != null)
        {
            ratingText.text = "Ratings: " + overallRating.ToString("F1");
        }
    }

    void Start()
    {
        if (ratingText != null)
        {
            ratingText.text = "Ratings: " + overallRating.ToString("F1");
        }
    }
}