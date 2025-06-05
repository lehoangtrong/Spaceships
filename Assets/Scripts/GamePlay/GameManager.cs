using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject asteroidPrefab;
    public GameObject smallAsteroidPrefab;

    [Header("Spawn Settings")]
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    public float destroyAfterSeconds = 15f;

    [Header("Round 1 Settings")]
    public float round1SpawnInterval = 3f;
    public int round1MaxAsteroids = 10;
    public float round1RowSpacing = 1f;
    public int round1RequiredScore = 100;
    public float round1DifficultyIncreaseTime = 5f; // Tăng độ khó mỗi 10 giây

    [Header("Round 2 Settings")]
    public float round2SpawnInterval = 3f;
    public float round2IntervalDecrease = 0.2f;
    public float round2MinInterval = 0.3f;
    public int round2MaxAsteroidsPerRow = 15;
    public float round2MaxSpeed = 15f;
    public int round2RequiredScore = 200;

    [Header("Round 3 Settings")]
    public float round3SpawnInterval = 2.5f;
    public int round3RequiredScore = 300;
    public float round3MinInterval = 0.5f;
    public float round3MaxSpeed = 12f;

    [Header("Round System")]
    public int currentRound = 1;
    public TextMeshProUGUI roundText;

    [Header("Score")]
    public static int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Player")]
    public GameObject playerObject;
    public ShieldController shieldController;
    public TextMeshProUGUI livesText;

    [Header("UI")]
    public GameObject pauseMenuScreen;

    // Round-specific variables
    private float nextSpawnTime;
    private float nextDifficultyIncrease;
    private int currentAsteroidCount = 1;
    private float currentSpawnInterval;
    private float currentAsteroidSpeed = 1f;
    private bool gameStarted = false;

    public bool IsPaused { get; set; }
    private bool isPaused = false;

    // Difficulty tracking
    private float round1NextDifficultyIncrease;
    private float round3NextDifficultyIncrease;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        HealthManager.health = 3;
    }

    private void Start()
    {
        InitializeGame();
        FindPlayerComponents();
        UpdateUI();
        StartRound(1);

        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if (!gameStarted || HealthManager.health <= 0) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        CheckRoundProgression();
        switch (currentRound)
        {
            case 1:
                UpdateRound1();
                break;
            case 2:
                UpdateRound2();
                break;
            case 3:
                UpdateRound3();
                break;
        }
    }

    void CheckRoundProgression()
    {
        switch (currentRound)
        {
            case 1:
                if (score >= round1RequiredScore)
                {
                    // Tăng độ khó cho cycle tiếp theo
                    round1RequiredScore += 30;
                    round2RequiredScore += 40;
                    round3RequiredScore += 50;
                    
                    StartRound(2);
                }
                break;
            case 2:
                if (score >= round2RequiredScore)
                {
                    StartRound(3);
                }
                break;
            case 3:
                if (score >= round3RequiredScore)
                {
                    Debug.Log("Completing Round 3, looping back to Round 1!");
                    
                    // Reset về Round 1 với độ khó cao hơn
                    StartRound(1);
                }
                break;
        }
    }

    void StartRound(int roundNumber)
    {
        currentRound = roundNumber;
        StopAllCoroutines();

        Debug.Log($"Starting Round {currentRound}");

        switch (currentRound)
        {
            case 1:
                InitializeRound1();
                break;
            case 2:
                InitializeRound2();
                break;
            case 3:
                InitializeRound3();
                break;
        }

        UpdateRoundText();
    }

    void InitializeRound1()
    {
        Debug.Log("Initializing Round 1");
        nextSpawnTime = Time.time;
        currentAsteroidCount = 3;
        currentSpawnInterval = round1SpawnInterval;
        currentAsteroidSpeed = 2f;
        round1NextDifficultyIncrease = Time.time + round1DifficultyIncreaseTime;
    }

    void UpdateRound1()
    {
        // Spawn asteroids
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound1Row();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }

        // Increase difficulty over time
        if (Time.time >= round1NextDifficultyIncrease)
        {
            IncreaseRound1Difficulty();
            round1NextDifficultyIncrease = Time.time + round1DifficultyIncreaseTime;
        }
    }

    void IncreaseRound1Difficulty()
    {
        // Increase asteroid count
        if (currentAsteroidCount < round1MaxAsteroids)
        {
            currentAsteroidCount++;
        }

        // Decrease spawn interval (faster spawning)
        currentSpawnInterval = Mathf.Max(0.5f, currentSpawnInterval - 0.2f);

        // Increase asteroid speed
        currentAsteroidSpeed = Mathf.Min(8f, currentAsteroidSpeed + 0.5f);

        Debug.Log($"Round 1 Difficulty: Count={currentAsteroidCount}, Speed={currentAsteroidSpeed}, Interval={currentSpawnInterval}");
    }

    void SpawnRound1Row()
    {
        float totalWidth = (currentAsteroidCount - 1) * round1RowSpacing;
        float startX = -totalWidth / 2f;

        Vector3[] positions = new Vector3[currentAsteroidCount];
        for (int i = 0; i < currentAsteroidCount; i++)
        {
            positions[i] = new Vector3(startX + i * round1RowSpacing, spawnHeight, 0f);
        }

        // Shuffle positions
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 temp = positions[i];
            int randomIndex = Random.Range(i, positions.Length);
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }

        // Spawn asteroids
        for (int i = 0; i < currentAsteroidCount; i++)
        {
            GameObject asteroid = Instantiate(asteroidPrefab, positions[i], Quaternion.identity);

            AsteroidController controller = asteroid.GetComponent<AsteroidController>();
            if (controller != null)
            {
                controller.SetSpeed(currentAsteroidSpeed + Random.Range(-0.5f, 0.5f));
                controller.maxHealth = 3;
            }

            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    void InitializeRound2()
    {
        Debug.Log("Initializing Round 2");
        currentAsteroidCount = 1;
        currentSpawnInterval = round2SpawnInterval;
        currentAsteroidSpeed = 1f;
        nextSpawnTime = Time.time;
        nextDifficultyIncrease = Time.time + 5f;
    }

    void UpdateRound2()
    {
        // Spawn asteroids
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound2Asteroids();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }

        // Increase difficulty continuously
        if (Time.time >= nextDifficultyIncrease)
        {
            IncreaseRound2Difficulty();
            nextDifficultyIncrease = Time.time + 3f; // Tăng độ khó mỗi 3 giây
        }
    }

    void SpawnRound2Asteroids()
    {
        for (int i = 0; i < currentAsteroidCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

            AsteroidController controller = asteroid.GetComponent<AsteroidController>();
            if (controller != null)
            {
                controller.SetSpeed(currentAsteroidSpeed + Random.Range(-0.5f, 0.5f));
                controller.maxHealth = 3;
            }

            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    void IncreaseRound2Difficulty()
    {
        // Increase asteroid count
        if (currentAsteroidCount < round2MaxAsteroidsPerRow)
        {
            currentAsteroidCount++;
        }

        // Decrease spawn interval (faster spawning)
        currentSpawnInterval = Mathf.Max(round2MinInterval, currentSpawnInterval - round2IntervalDecrease);

        // Increase speed
        currentAsteroidSpeed = Mathf.Min(round2MaxSpeed, currentAsteroidSpeed + 0.3f);

        Debug.Log($"Round 2 Difficulty: Count={currentAsteroidCount}, Speed={currentAsteroidSpeed}, Interval={currentSpawnInterval}");
    }

    void InitializeRound3()
    {
        Debug.Log("Initializing Round 3");
        nextSpawnTime = Time.time;
        currentSpawnInterval = round3SpawnInterval;
        currentAsteroidSpeed = 1.5f;
        round3NextDifficultyIncrease = Time.time + 8f;
    }

    void UpdateRound3()
    {
        // Spawn asteroids
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound3Asteroid();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }

        // Increase difficulty over time
        if (Time.time >= round3NextDifficultyIncrease)
        {
            IncreaseRound3Difficulty();
            round3NextDifficultyIncrease = Time.time + 6f; // Tăng độ khó mỗi 6 giây
        }
    }

    void IncreaseRound3Difficulty()
    {
        // Decrease spawn interval (faster spawning)
        currentSpawnInterval = Mathf.Max(round3MinInterval, currentSpawnInterval - 0.15f);

        // Increase asteroid speed
        currentAsteroidSpeed = Mathf.Min(round3MaxSpeed, currentAsteroidSpeed + 0.4f);

        Debug.Log($"Round 3 Difficulty: Speed={currentAsteroidSpeed}, Interval={currentSpawnInterval}");
    }

    void SpawnRound3Asteroid()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        AsteroidController controller = asteroid.GetComponent<AsteroidController>();
        if (controller != null)
        {
            controller.SetSpeed(currentAsteroidSpeed + Random.Range(-0.3f, 0.3f));
            controller.maxHealth = 1;
            asteroid.tag = "SplittingAsteroid";
        }

        Destroy(asteroid, destroyAfterSeconds);
    }

    public void SpawnSmallAsteroids(Vector3 position)
    {
        if (smallAsteroidPrefab == null) return;

        for (int i = 0; i < 3; i++)
        {
            float angle = i * 120f;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), -1f, 0f).normalized;

            Vector3 spawnPos = position + direction * 0.5f;
            GameObject smallAsteroid = Instantiate(smallAsteroidPrefab, spawnPos, Quaternion.identity);

            SmallAsteroidController smallController = smallAsteroid.GetComponent<SmallAsteroidController>();
            if (smallController == null)
            {
                smallController = smallAsteroid.AddComponent<SmallAsteroidController>();
            }

            smallController.Initialize(direction, playerObject);
            Destroy(smallAsteroid, destroyAfterSeconds);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = spawnHeight + Random.Range(-0.5f, 0.5f);
        return new Vector3(randomX, randomY, 0f);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuScreen.SetActive(true);
        isPaused = true;
    }

    void InitializeGame()
    {
        score = 0;
        HealthManager.health = 3;
        currentRound = 1;
        gameStarted = true;
        
        // Initialize required scores
        round1RequiredScore = 100;
        round2RequiredScore = 200;
        round3RequiredScore = 300;
    }

    void FindPlayerComponents()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        if (playerObject != null)
        {
            shieldController = playerObject.GetComponent<ShieldController>();
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void LoseLife()
    {
        HealthManager.health--;
        UpdateLivesText();

        if (HealthManager.health <= 0)
        {
            Invoke("GameOver", 0.1f);
        }
        else if (shieldController != null)
        {
            shieldController.ActivateShield();
        }
    }

    void GameOver()
    {
        gameStarted = false;
        StopAllCoroutines();
        if (playerObject != null)
        {
            Destroy(playerObject);
        }
        Time.timeScale = 0f;
        SceneManager.LoadScene("EndGame");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuScreen.SetActive(false);
        isPaused = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void UpdateUI()
    {
        UpdateLivesText();
        UpdateScoreText();
        UpdateRoundText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + HealthManager.health.ToString();
        }
    }

    void UpdateRoundText()
    {
        if (roundText != null)
        {
            roundText.text = "Round: " + currentRound.ToString();
        }
    }

    public int GetCurrentRound() => currentRound;
    public int GetCurrentLives() => HealthManager.health;
    public void SetCurrentLives(int lives)
    {
        HealthManager.health = Mathf.Max(0, lives);
        UpdateLivesText();
    }
    public int GetCurrentScore() => score;
}
