using UnityEngine;
using TMPro;
using System.Collections;

public class EndingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI startScreenText;
    private bool canCheckRating = false;

    void Start()
    {
        startScreenText.color = Color.white;
        startScreenText.text = "Welcome to Soul Fuel! Press play to begin.";
        StartCoroutine(WaitBeforeCheck());
    }

    IEnumerator WaitBeforeCheck()
    {
        yield return new WaitForSeconds(10f); // Wait 10 seconds
        canCheckRating = true;
    }

    void Update()
    {
        if (!canCheckRating) return;

        if (RatingSystem.overallRating >= 2.5f)
        {
            startScreenText.color = Color.green;
            startScreenText.text = "Congrats! You've received 2.5 stars or above, and won today. Play again?";
        }
        else
        {
            startScreenText.color = Color.red;
            startScreenText.text = "Sorry! You've received under 2.5 stars, and gone out of business. Play again?";
        }
    }
}
