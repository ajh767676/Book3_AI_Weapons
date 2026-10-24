using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
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
}