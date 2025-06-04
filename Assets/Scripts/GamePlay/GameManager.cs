using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Asteroid Prefabs")]
    public GameObject asteroidPrefab;
    public GameObject smallAsteroidPrefab; // For round 3 small asteroids

    [Header("Spawn Settings")]
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    public float destroyAfterSeconds = 10f;
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f; [Header("Round System")]
    public int currentRound = 1;
    // public TextMeshProUGUI roundText; // TODO: Re-enable when TMPro is available

    [Header("Round 1 Settings")]
    public float round1SpawnInterval = 3f;
    public int round1MaxAsteroids = 8;
    public float round1RowSpacing = 1.5f;
    public int round1RequiredScore = 500;

    [Header("Round 2 Settings")]
    public float round2SpawnInterval = 2f;
    public float round2IntervalDecrease = 0.2f;
    public float round2MinInterval = 0.5f;
    public int round2MaxAsteroidsPerRow = 10;
    public float round2MaxSpeed = 10f;
    public int round2RequiredScore = 1000;

    [Header("Round 3 Settings")]
    public float round3SpawnInterval = 2.5f;
    public int round3RequiredScore = 2000; [Header("Game Stats")]
    public int score = 0;
    public int lives = 3;
    // public TextMeshProUGUI scoreText; // TODO: Re-enable when TMPro is available
    // public TextMeshProUGUI livesText; // TODO: Re-enable when TMPro is available    [Header("Player References")]
    public PlayerMovement playerMovement;
    public ShieldController shieldController;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    // public TextMeshProUGUI finalScoreText; // TODO: Re-enable when TMPro is available

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    [Header("Game State")]
    public bool IsPaused { get; private set; } = false;

    private Coroutine spawnCoroutine;
    private bool gameIsOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
        StartSpawning();
        PlayBackgroundMusic();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
        CheckRoundProgression();
    }

    public void LoseLife()
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }
    private void UpdateUI()
    {
        // TODO: Re-enable when TMPro is available
        // if (scoreText != null)
        //     scoreText.text = "Score: " + score;

        // if (livesText != null)
        //     livesText.text = "Lives: " + lives;

        // if (roundText != null)
        //     roundText.text = "Round: " + currentRound;

        Debug.Log($"Score: {score}, Lives: {lives}, Round: {currentRound}");
    }

    private void CheckRoundProgression()
    {
        switch (currentRound)
        {
            case 1:
                if (score >= round1RequiredScore)
                {
                    AdvanceToRound2();
                }
                break;
            case 2:
                if (score >= round2RequiredScore)
                {
                    AdvanceToRound3();
                }
                break;
        }
    }

    private void AdvanceToRound2()
    {
        currentRound = 2;
        UpdateUI();
        Debug.Log("Advanced to Round 2!");
    }

    private void AdvanceToRound3()
    {
        currentRound = 3;
        UpdateUI();
        Debug.Log("Advanced to Round 3!");
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    private void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnAsteroids());
    }

    private IEnumerator SpawnAsteroids()
    {
        while (!gameIsOver)
        {
            switch (currentRound)
            {
                case 1:
                    yield return StartCoroutine(SpawnRound1());
                    break;
                case 2:
                    yield return StartCoroutine(SpawnRound2());
                    break;
                case 3:
                    yield return StartCoroutine(SpawnRound3());
                    break;
            }
        }
    }

    private IEnumerator SpawnRound1()
    {
        SpawnAsteroidRow(round1MaxAsteroids);
        yield return new WaitForSeconds(round1SpawnInterval);
    }

    private IEnumerator SpawnRound2()
    {
        SpawnAsteroidRow(round2MaxAsteroidsPerRow);
        yield return new WaitForSeconds(round2SpawnInterval);
    }

    private IEnumerator SpawnRound3()
    {
        SpawnSplittingAsteroid();
        yield return new WaitForSeconds(round3SpawnInterval);
    }

    private void SpawnAsteroidRow(int count)
    {
        float spacing = (spawnRangeX * 2) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float xPosition = -spawnRangeX + (i * spacing);
            Vector3 spawnPosition = new Vector3(xPosition, spawnHeight, 0);

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    private void SpawnSplittingAsteroid()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
        Destroy(asteroid, destroyAfterSeconds);
    }

    public void GameOver()
    {
        gameIsOver = true;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // TODO: Re-enable when TMPro is available
        // if (finalScoreText != null)
        // {
        //     finalScoreText.text = "Final Score: " + score;
        // }

        PlayGameOverSound();
        Debug.Log("Game Over! Final Score: " + score);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your main menu scene name
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void PlayGameOverSound()
    {
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.clip = gameOverSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }
}
