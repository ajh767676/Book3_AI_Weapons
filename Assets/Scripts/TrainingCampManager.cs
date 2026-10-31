using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class TrainingCampManager : MonoBehaviour
{
    public static TrainingCampManager instance;

    public TMP_Text pauseButtonText;
    public TMP_Text scoreText;

    public FirstPersonController playerController;

    private bool isPaused = true;
    private int score = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreText();

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

    public void AddScore()
    {
        score++;
        UpdateScoreText();

        if (score >= 3)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene("winScene");
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Collectibles: " + score + " / 3";
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