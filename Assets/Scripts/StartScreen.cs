using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("mitchel scene");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
