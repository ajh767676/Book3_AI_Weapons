using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("gameLevel");
    }

    public void OpenSimpleAI()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("simple_ai");
    }

    public void OpenTrainingCamp()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("training_camp");
    }

    public void SelectScene(int option)
    {
        Time.timeScale = 1f;

        if (option == 0)
        {
            SceneManager.LoadScene("gameLevel");
        }
        else if (option == 1)
        {
            SceneManager.LoadScene("loseScene");
        }
        else if (option == 2)
        {
            SceneManager.LoadScene("winScene");
        }
        else if (option == 3)
        {
            SceneManager.LoadScene("gameLevelAuto");
        }
    }
}