//using UnityEngine;
//using System.Collections;
//using TMPro;
//using UnityEngine.UIElements;
//using UnityEngine.SceneManagement;

//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance;

//    [Header("Asteroid Prefabs")]
//    public GameObject asteroidPrefab;
//    public GameObject smallAsteroidPrefab; // For round 3 small asteroids

//    [Header("Spawn Settings")]
//    public float spawnRangeX = 8f;
//    public float spawnHeight = 6f;
//    public float destroyAfterSeconds = 10f;

//    [Header("Round System")]
//    public int currentRound = 1;
//    public TextMeshProUGUI roundText;

//    [Header("Round 1 Settings")]
//    public float round1SpawnInterval = 3f;
//    public int round1MaxAsteroids = 8;
//    public float round1RowSpacing = 1.5f;
//    public int round1RequiredScore = 500;

//    [Header("Round 2 Settings")]
//    public float round2SpawnInterval = 2f;
//    public float round2IntervalDecrease = 0.2f;
//    public float round2MinInterval = 0.5f;
//    public int round2MaxAsteroidsPerRow = 10;
//    public float round2MaxSpeed = 10f;
//    public int round2RequiredScore = 1000;

//    [Header("Round 3 Settings")]
//    public float round3SpawnInterval = 2.5f;
//    public int round3RequiredScore = 2000;

//    [Header("Score")]
//    public static int score = 0;
//    public TextMeshProUGUI scoreText;

//    [Header("Player Lives")]
//    public int playerLives = 3;
//    public TextMeshProUGUI livesText;
//    public GameObject playerObject;
//    public ShieldController shieldController;

//    // Round-specific variables
//    private float nextSpawnTime;
//    private float nextDifficultyIncrease;
//    private int currentAsteroidCount = 1;
//    private float currentSpawnInterval;
//    private float currentAsteroidSpeed = 1f;
//    private bool gameStarted = false;
//    public bool IsPaused { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        InitializeGame();
//        FindPlayerComponents();
//        UpdateUI();
//        StartRound(1);
//    }

//    void InitializeGame()
//    {
//        score = 0;
//        playerLives = 3;
//        currentRound = 1;
//        gameStarted = true;
//    }

//    void FindPlayerComponents()
//    {
//        if (playerObject == null)
//        {
//            playerObject = GameObject.FindGameObjectWithTag("Player");
//        }
//        if (playerObject != null)
//        {
//            shieldController = playerObject.GetComponent<ShieldController>();
//        }
//    }
//    // Add this method for testing - you can call it from Update with a key press
//    void TestRoundProgression()
//    {
//        if (Input.GetKeyDown(KeyCode.N)) // Press N to advance to next round
//        {
//            //Debug.Log("Manual round advancement - Current round: " + currentRound);
//            if (currentRound < 3)
//            {
//                StartRound(currentRound + 1);
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.P)) // Press P to add 10 points
//        {
//            AddScore(10);
//            //Debug.Log("Added 10 points for testing");
//        }
//    }

//    void Update()
//    {
//        if (!gameStarted || playerLives <= 0) return;
//        TestRoundProgression();
//        CheckRoundProgression();

//        switch (currentRound)
//        {
//            case 1:
//                UpdateRound1();
//                break;
//            case 2:
//                UpdateRound2();
//                break;
//            case 3:
//                UpdateRound3();
//                break;
//        }
//    }

//    void CheckRoundProgression()
//    {
//        //Debug.Log($"CheckRoundProgression: Current Round={currentRound}, Score={score}");

//        switch (currentRound)
//        {
//            case 1:
//                //Debug.Log($"Round 1 check: Score={score}, Required={round1RequiredScore}");
//                if (score >= round1RequiredScore)
//                {
//                    //Debug.Log("Advancing to Round 2!");
//                    StartRound(2);
//                }
//                break;
//            case 2:
//                //Debug.Log($"Round 2 check: Score={score}, Required={round2RequiredScore}");
//                if (score >= round2RequiredScore)
//                {
//                    //Debug.Log("Advancing to Round 3!");
//                    StartRound(3);
//                }
//                break;
//            case 3:
//                //Debug.Log($"Round 3 check: Score={score}, Required={round3RequiredScore}");
//                if (score >= round3RequiredScore)
//                {
//                    Debug.Log("Game Completed!");
//                    GameCompleted();
//                }
//                break;
//        }
//    }

//    void StartRound(int roundNumber)
//    {
//        currentRound = roundNumber;
//        StopAllCoroutines();

//        Debug.Log($"Starting Round {currentRound}");

//        switch (currentRound)
//        {
//            case 1:
//                InitializeRound1();
//                break;
//            case 2:
//                InitializeRound2();
//                break;
//            case 3:
//                InitializeRound3();
//                break;
//        }

//        UpdateRoundText();
//    }

//    // ========== ROUND 1: Progressive spawning ==========
//    void InitializeRound1()
//    {
//        nextSpawnTime = Time.time;
//        currentAsteroidCount = 3; // Start with 3 asteroids per row
//    }

//    void UpdateRound1()
//    {
//        if (Time.time >= nextSpawnTime)
//        {
//            SpawnRound1Row();
//            nextSpawnTime = Time.time + round1SpawnInterval;

//            // Gradually increase asteroids per row (max 8)
//            if (currentAsteroidCount < round1MaxAsteroids)
//            {
//                currentAsteroidCount++;
//            }
//        }
//    }

//    void SpawnRound1Row()
//    {
//        // Calculate spacing between asteroids
//        float totalWidth = (currentAsteroidCount - 1) * round1RowSpacing;
//        float startX = -totalWidth / 2f;

//        // Create array of positions and shuffle them
//        Vector3[] positions = new Vector3[currentAsteroidCount];
//        for (int i = 0; i < currentAsteroidCount; i++)
//        {
//            positions[i] = new Vector3(startX + i * round1RowSpacing, spawnHeight, 0f);
//        }

//        // Shuffle positions
//        for (int i = 0; i < positions.Length; i++)
//        {
//            Vector3 temp = positions[i];
//            int randomIndex = Random.Range(i, positions.Length);
//            positions[i] = positions[randomIndex];
//            positions[randomIndex] = temp;
//        }

//        // Spawn asteroids at shuffled positions
//        for (int i = 0; i < currentAsteroidCount; i++)
//        {
//            GameObject asteroid = Instantiate(asteroidPrefab, positions[i], Quaternion.identity);

//            AsteroidController controller = asteroid.GetComponent<AsteroidController>();
//            if (controller != null)
//            {
//                controller.SetSpeed(2f + Random.Range(-0.5f, 0.5f));
//                controller.maxHealth = 3;
//            }

//            Destroy(asteroid, destroyAfterSeconds);
//        }

//        //Debug.Log($"Round 1: Spawned row of {currentAsteroidCount} asteroids");
//    }

//    // ========== ROUND 2: Progressive spawning (formerly Round 1) ==========
//    void InitializeRound2()
//    {
//        currentAsteroidCount = 1;
//        currentSpawnInterval = round2SpawnInterval;
//        currentAsteroidSpeed = 1f;
//        nextSpawnTime = Time.time;
//        nextDifficultyIncrease = Time.time + 5f; // Increase every 5 seconds
//    }

//    void UpdateRound2()
//    {
//        // Spawn asteroids
//        if (Time.time >= nextSpawnTime)
//        {
//            SpawnRound2Asteroids();
//            nextSpawnTime = Time.time + currentSpawnInterval;
//        }

//        // Increase difficulty every 5 seconds
//        if (Time.time >= nextDifficultyIncrease)
//        {
//            IncreaseRound2Difficulty();
//            nextDifficultyIncrease = Time.time + 5f;
//        }
//    }

//    void SpawnRound2Asteroids()
//    {
//        for (int i = 0; i < currentAsteroidCount; i++)
//        {
//            Vector3 spawnPos = GetRandomSpawnPosition();
//            GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

//            AsteroidController controller = asteroid.GetComponent<AsteroidController>();
//            if (controller != null)
//            {
//                controller.SetSpeed(currentAsteroidSpeed + Random.Range(-0.5f, 0.5f));
//                controller.maxHealth = 3; // 3 hits to destroy
//            }

//            Destroy(asteroid, destroyAfterSeconds);
//        }
//    }

//    void IncreaseRound2Difficulty()
//    {
//        // Increase asteroid count (max 8)
//        if (currentAsteroidCount < round2MaxAsteroidsPerRow)
//        {
//            currentAsteroidCount++;
//        }

//        // Decrease spawn interval (min 0.5s)
//        currentSpawnInterval = Mathf.Max(round2MinInterval, currentSpawnInterval - round2IntervalDecrease);

//        // Increase speed (max 10)
//        currentAsteroidSpeed = Mathf.Min(round2MaxSpeed, currentAsteroidSpeed + 0.5f);

//        //Debug.Log($"Round 2 Difficulty: Count={currentAsteroidCount}, Speed={currentAsteroidSpeed}, Interval={currentSpawnInterval}");
//    }

//    // ========== ROUND 3: Splitting asteroids ==========
//    void InitializeRound3()
//    {
//        nextSpawnTime = Time.time;
//    }

//    void UpdateRound3()
//    {
//        if (Time.time >= nextSpawnTime)
//        {
//            SpawnRound3Asteroid();
//            nextSpawnTime = Time.time + round3SpawnInterval;
//        }
//    }

//    void SpawnRound3Asteroid()
//    {
//        Vector3 spawnPos = GetRandomSpawnPosition();
//        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

//        AsteroidController controller = asteroid.GetComponent<AsteroidController>();
//        if (controller != null)
//        {
//            controller.SetSpeed(1.5f + Random.Range(-0.5f, 0.5f));
//            controller.maxHealth = 1; // Only 1 hit to split

//            // Mark this as a splitting asteroid
//            asteroid.tag = "SplittingAsteroid";
//        }

//        Destroy(asteroid, destroyAfterSeconds);
//    }

//    // This method will be called by AsteroidController when a splitting asteroid is destroyed
//    public void SpawnSmallAsteroids(Vector3 position)
//    {
//        if (smallAsteroidPrefab == null) return;

//        for (int i = 0; i < 3; i++)
//        {
//            // Calculate spread directions
//            float angle = i * 120f; // 120 degrees apart
//            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), -1f, 0f).normalized;

//            Vector3 spawnPos = position + direction * 0.5f;
//            GameObject smallAsteroid = Instantiate(smallAsteroidPrefab, spawnPos, Quaternion.identity);

//            // Add component for player tracking
//            SmallAsteroidController smallController = smallAsteroid.GetComponent<SmallAsteroidController>();
//            if (smallController == null)
//            {
//                smallController = smallAsteroid.AddComponent<SmallAsteroidController>();
//            }

//            smallController.Initialize(direction, playerObject);

//            Destroy(smallAsteroid, destroyAfterSeconds);
//        }
//    }

//    Vector3 GetRandomSpawnPosition()
//    {
//        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
//        float randomY = spawnHeight + Random.Range(-0.5f, 0.5f);
//        return new Vector3(randomX, randomY, 0f);
//    }

//    // ========== GAME STATE MANAGEMENT ==========
//    public void LoseLife()
//    {
//        HealthManager.health--;
//        UpdateLivesText();

//        if (HealthManager.health <= 0)
//        {
//            Invoke("GameOver", 0.1f); 
//        }
//        else if (shieldController != null)
//        {
//            shieldController.ActivateShield();
//        }
//    }

//    void GameOver()
//    {
//        Debug.Log("Game Over!");
//        gameStarted = false;
//        StopAllCoroutines();

//        if (playerObject != null)
//        {
//            Destroy(playerObject);
//        }
//        Time.timeScale = 0f;
//        SceneManager.LoadScene("EndGame"); // Load Game Over scene
//    }

//    void GameCompleted()
//    {
//        Debug.Log("Game Completed! You finished all rounds!");
//        gameStarted = false;
//        StopAllCoroutines();
//        // Add game completion UI here
//    }

//    public void RestartGame()
//    {
//        Time.timeScale = 1f;

//        // Clear all asteroids
//        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
//        foreach (GameObject asteroid in asteroids)
//        {
//            Destroy(asteroid);
//        }

//        GameObject[] splittingAsteroids = GameObject.FindGameObjectsWithTag("SplittingAsteroid");
//        foreach (GameObject asteroid in splittingAsteroids)
//        {
//            Destroy(asteroid);
//        }

//        InitializeGame();
//        UpdateUI();
//        StartRound(1);
//    }

//    // ========== SCORE MANAGEMENT ==========
//    public void AddScore(int points)
//    {
//        score += points;
//        //Debug.Log($"Score added: +{points}, Total score: {score}");
//        UpdateScoreText();
//    }

//    void UpdateUI()
//    {
//        UpdateLivesText();
//        UpdateScoreText();
//        UpdateRoundText();
//    }

//    void UpdateLivesText()
//    {
//        if (livesText != null)
//        {
//            livesText.text = "Lives: " + HealthManager.health.ToString();
//        }   
//    }

//    void UpdateScoreText()
//    {
//        if (scoreText != null)
//        {
//            int requiredScore = GetRequiredScoreForCurrentRound();
//            scoreText.text = $"Score: {score}/{requiredScore}";
//        }
//    }

//    void UpdateRoundText()
//    {
//        if (roundText != null)
//        {
//            roundText.text = "Round: " + currentRound.ToString();
//        }
//    }

//    int GetRequiredScoreForCurrentRound()
//    {
//        switch (currentRound)
//        {
//            case 1: return round1RequiredScore;
//            case 2: return round2RequiredScore;
//            case 3: return round3RequiredScore;
//            default: return 0;
//        }
//    }

//    // ========== GETTERS ==========
//    public int GetCurrentScore() => score;  
//    public int GetCurrentLives() => HealthManager.health;
//    public int GetCurrentRound() => currentRound;
//}


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
    public float destroyAfterSeconds = 10f;

    [Header("Round 1 Settings")]
    public float round1SpawnInterval = 3f;
    public int round1MaxAsteroids = 8;
    public float round1RowSpacing = 1.5f;
    public int round1RequiredScore = 10;

    [Header("Round 2 Settings")]
    public float round2SpawnInterval = 2f;
    public float round2IntervalDecrease = 0.2f;
    public float round2MinInterval = 0.5f;
    public int round2MaxAsteroidsPerRow = 10;
    public float round2MaxSpeed = 10f;
    public int round2RequiredScore = 25;

    [Header("Round System")]
    public int currentRound = 1;
    public TextMeshProUGUI roundText;
    public float round3SpawnInterval = 2.5f;
    public int round3RequiredScore = 35;

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

    public bool IsPaused { get; private set; }

    private bool isPaused = false;
 
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

        //if (currentRound == 3 && Time.time >= nextSpawnTime)
        //{
        //    SpawnRound3Asteroid();
        //    nextSpawnTime = Time.time + round3SpawnInterval;
        //}

        //TestRoundProgression();
        //CheckRoundProgression();
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

    public void PauseGame()
    {
        Time.timeScale = 0; // Dừng game
        pauseMenuScreen.SetActive(true);
        isPaused = true;
    }

    void InitializeGame()
    {
        score = 0;
        HealthManager.health = 3;
        currentRound = 1;
        gameStarted = true;
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
        nextSpawnTime = Time.time;
        currentAsteroidCount = 3; // Start with 3 asteroids per row
    }

    void UpdateRound1()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound1Row();
            nextSpawnTime = Time.time + round1SpawnInterval;

            // Gradually increase asteroids per row (max 8)
            if (currentAsteroidCount < round1MaxAsteroids)
            {
                currentAsteroidCount++;
            }
        }
    }

    void SpawnRound1Row()
    {
        // Calculate spacing between asteroids
        float totalWidth = (currentAsteroidCount - 1) * round1RowSpacing;
        float startX = -totalWidth / 2f;

        // Create array of positions and shuffle them
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

        // Spawn asteroids at shuffled positions
        for (int i = 0; i < currentAsteroidCount; i++)
        {
            GameObject asteroid = Instantiate(asteroidPrefab, positions[i], Quaternion.identity);

            AsteroidController controller = asteroid.GetComponent<AsteroidController>();
            if (controller != null)
            {
                controller.SetSpeed(2f + Random.Range(-0.5f, 0.5f));
                controller.maxHealth = 3;
            }

            Destroy(asteroid, destroyAfterSeconds);
        }

        //Debug.Log($"Round 1: Spawned row of {currentAsteroidCount} asteroids");
    }

    void InitializeRound2()
    {
        currentAsteroidCount = 1;
        currentSpawnInterval = round2SpawnInterval;
        currentAsteroidSpeed = 1f;
        nextSpawnTime = Time.time;
        nextDifficultyIncrease = Time.time + 5f; // Increase every 5 seconds
    }

    void UpdateRound2()
    {
        // Spawn asteroids
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound2Asteroids();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }

        // Increase difficulty every 5 seconds
        if (Time.time >= nextDifficultyIncrease)
        {
            IncreaseRound2Difficulty();
            nextDifficultyIncrease = Time.time + 5f;
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
                controller.maxHealth = 3; // 3 hits to destroy
            }

            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = spawnHeight + Random.Range(-0.5f, 0.5f);
        return new Vector3(randomX, randomY, 0f);
    }

    void IncreaseRound2Difficulty()
    {
        // Increase asteroid count (max 8)
        if (currentAsteroidCount < round2MaxAsteroidsPerRow)
        {
            currentAsteroidCount++;
        }

        // Decrease spawn interval (min 0.5s)
        currentSpawnInterval = Mathf.Max(round2MinInterval, currentSpawnInterval - round2IntervalDecrease);

        // Increase speed (max 10)
        currentAsteroidSpeed = Mathf.Min(round2MaxSpeed, currentAsteroidSpeed + 0.5f);

        //Debug.Log($"Round 2 Difficulty: Count={currentAsteroidCount}, Speed={currentAsteroidSpeed}, Interval={currentSpawnInterval}");
    }

    // ========== ROUND 3: Splitting asteroids ==========
    void InitializeRound3()
    {
        Debug.Log("Initializing Round 3");
        nextSpawnTime = Time.time;
    }

    void UpdateRound3()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnRound3Asteroid();
            nextSpawnTime = Time.time + round3SpawnInterval;
        }
    }


    IEnumerator SpawnAsteroids()
    {
        while (currentRound != 3)
        {
            float delay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(delay);
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight, 0f);
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    void SpawnRound3Asteroid()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight, 0f);
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        AsteroidController controller = asteroid.GetComponent<AsteroidController>();
        if (controller != null)
        {
            controller.SetSpeed(1.5f + Random.Range(-0.5f, 0.5f));
            controller.maxHealth = 1;
            controller.splitOnDeath = true;
            controller.childAsteroidPrefab = smallAsteroidPrefab;
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
        Time.timeScale = 1; // Tiếp tục game
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
        HealthManager.health = Mathf.Max(0, lives); // Ensure health doesn't go below 0
        UpdateLivesText();
    }
    public int GetCurrentScore() => score;
}
