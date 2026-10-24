using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class TrainingCampManager : MonoBehaviour
{
    public TMP_Text pauseButtonText;

    public FirstPersonController playerController;

    private bool isPaused = true;

    void Start()
    {
        playerController.enabled = false;
        Time.timeScale = 0f;
        pauseButtonText.text = "START";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseButtonText.text = "RESUME";
            playerController.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            pauseButtonText.text = "PAUSE";
            playerController.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}