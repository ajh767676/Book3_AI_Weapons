using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleAIGameManager : MonoBehaviour
{
    public static SimpleAIGameManager instance;

    public TMP_Text levelText;

    private int currentLevel = 1;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text pauseButtonText;

    public Slider intensitySlider;
    public TMP_Text intensityText;
    public LaunchProjectile launcher;

    private int score = 0;
    private float elapsedTime = 0f;
    private bool isPaused = true;

    public Slider spawnIntervalSlider;
    public TMP_Text spawnIntervalText;

    public TMP_Dropdown collectiblesDropdown;
    public TMP_Text collectiblesText;

    private int collectiblesNeeded = 3;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreText();
        UpdateLevelText();
        timerText.text = "Time: 0.0";

        intensitySlider.onValueChanged.AddListener(
            SetProjectileIntensity
        );

        SetProjectileIntensity(intensitySlider.value);

        Time.timeScale = 0f;
        pauseButtonText.text = "START";

        spawnIntervalSlider.onValueChanged.AddListener(
            SetSpawnInterval
        );

        SetSpawnInterval(spawnIntervalSlider.value);

        collectiblesDropdown.onValueChanged.AddListener(
            SetCollectiblesNeeded
        );

        SetCollectiblesNeeded(collectiblesDropdown.value);
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = "Time: " + elapsedTime.ToString("F1");
        }
    }

    public void SetProjectileIntensity(float sliderValue)
    {
        float intensity = sliderValue * 100f;

        launcher.projectileIntensity = intensity;
        intensityText.text =
            "Projectile Intensity: " + intensity.ToString("F0");
    }

    public void SetSpawnInterval(float seconds)
    {
        launcher.spawnInterval = seconds;

        spawnIntervalText.text =
            "Spawn Interval: " + seconds.ToString("F0") + " seconds";
    }

    public void SetCollectiblesNeeded(int option)
    {
        if (option == 0)
            collectiblesNeeded = 3;
        else if (option == 1)
            collectiblesNeeded = 5;
        else
            collectiblesNeeded = 10;

        collectiblesText.text =
            "Objects to Collect: " + collectiblesNeeded;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseButtonText.text = "RESUME";

            intensitySlider.interactable = true;
            spawnIntervalSlider.interactable = true;
            collectiblesDropdown.interactable = true;
        }
        else
        {
            Time.timeScale = 1f;
            pauseButtonText.text = "PAUSE";

            intensitySlider.interactable = false;
            spawnIntervalSlider.interactable = false;
            collectiblesDropdown.interactable = false;
        }
    }



    public void AddScore()
    {
        score++;
        UpdateScoreText();

        if (score >= collectiblesNeeded)
        {
            currentLevel++;
            score = 0;

            UpdateScoreText();
            UpdateLevelText();

            Debug.Log("Level completed! Starting level " + currentLevel);
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateLevelText()
    {
        levelText.text = "Level: " + currentLevel;
    }
}