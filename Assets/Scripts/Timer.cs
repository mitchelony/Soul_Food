using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remaningTime;

    void Update()
    {
        if (remaningTime > 0)
        {
            remaningTime -= Time.deltaTime;
        }

        else if (remaningTime < 0)
        {
            remaningTime = 0;
            SceneManager.LoadScene("StartScreen");
        }

        int minutes = Mathf.FloorToInt(remaningTime / 60);
        int seconds = Mathf.FloorToInt(remaningTime % 60);
        timerText.text = string.Format("Time Left: {0:00}:{1:00}", minutes, seconds);
    }
}
