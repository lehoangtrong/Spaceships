using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Asteroid Prefab")]
    public GameObject asteroidPrefab;

    [Header("Spawn Settings")]
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    public float destroyAfterSeconds = 10f;

    [Header("Initial Settings (lúc bắt đầu)")]
    public int initialAsteroidCount = 3;
    public float initialSpawnInterval = 2f;
    public float initialAsteroidSpeed = 1f;

    [Header("Difficulty Increase (mỗi 10 giây)")]
    public float difficultyIncreaseInterval = 10f;
    public int asteroidCountIncrease = 1;
    public float spawnIntervalDecrease = 0.2f;
    public float speedIncrease = 1f;

    [Header("Maximum Limits")]
    public int maxAsteroidCount = 10;
    public float minSpawnInterval = 0.5f;
    public float maxAsteroidSpeed = 15f;

    [Header("Score")]
    public static int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Player Lives")]
    public TextMeshProUGUI livesText;
    public GameObject playerObject;
    public ShieldController playerShield;

    [Header("UI")]
    public GameObject pauseMenuScreen;

    // Biến trạng thái hiện tại
    private int currentAsteroidCount;
    private float currentSpawnInterval;
    private float currentAsteroidSpeed;

    // Thêm biến theo dõi trạng thái tạm dừng
    private bool isPaused = false;

    // Thêm property để cho phép các script khác biết game có đang tạm dừng không
    public bool IsPaused
    {
        get { return isPaused; }
    }

    // Timer
    private float nextDifficultyIncreaseTime;
    private int difficultyLevel = 0;
    private bool gameStarted = false;

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
        // Khởi tạo số mạng trong HealthManager (mặc định là 3)
        HealthManager.health = 3;
    }

    private void Start()
    {
        // Khởi tạo giá trị ban đầu
        InitializeGame();

        // Tìm player và shield controller nếu chưa được gán
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        if (playerObject != null)
        {
            playerShield = playerObject.GetComponent<ShieldController>();
        }

        // Cập nhật hiển thị ban đầu
        UpdateLivesText();

        // Đảm bảo pauseMenuScreen ẩn khi bắt đầu
        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(false);
        }
        UpdateScoreText();

        // Bắt đầu spawn asteroids
        StartCoroutine(SpawnAsteroids());

        Debug.Log($"Game bắt đầu - Asteroid: {currentAsteroidCount}, Speed: {currentAsteroidSpeed}, Interval: {currentSpawnInterval}");
    }

    void InitializeGame()
    {
        currentAsteroidCount = initialAsteroidCount;
        currentSpawnInterval = initialSpawnInterval;
        currentAsteroidSpeed = initialAsteroidSpeed;
        difficultyLevel = 0;
        gameStarted = true;

        nextDifficultyIncreaseTime = Time.time + difficultyIncreaseInterval;
    }

    void Update()
    {
        if (!gameStarted) return;

        // Kiểm tra và tăng độ khó theo thời gian
        if (Time.time >= nextDifficultyIncreaseTime)
        {
            IncreaseDifficulty();
            nextDifficultyIncreaseTime = Time.time + difficultyIncreaseInterval;
        }

        // Kiểm tra khi người chơi bấm phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nếu game đang chạy, tạm dừng. Nếu đang tạm dừng, tiếp tục
            TogglePause();
        }
    }

    void IncreaseDifficulty()
    {
        difficultyLevel++;

        // Tăng số lượng asteroid (không vượt quá max)
        currentAsteroidCount = Mathf.Min(maxAsteroidCount,
            currentAsteroidCount + asteroidCountIncrease);

        // Giảm thời gian spawn (spawn nhanh hơn, không nhỏ hơn min)
        currentSpawnInterval = Mathf.Max(minSpawnInterval,
            currentSpawnInterval - spawnIntervalDecrease);

        // Tăng tốc độ asteroid (không vượt quá max)
        currentAsteroidSpeed = Mathf.Min(maxAsteroidSpeed,
            currentAsteroidSpeed + speedIncrease);

        Debug.Log($"Độ khó tăng lên cấp {difficultyLevel}!");
        Debug.Log($"Asteroid count: {currentAsteroidCount}, Speed: {currentAsteroidSpeed}, Spawn interval: {currentSpawnInterval}");
    }

    IEnumerator SpawnAsteroids()
    {
        while (gameStarted && HealthManager.health > 0)  // Thay playerLives thành HealthManager.health
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            // Spawn multiple asteroids
            for (int i = 0; i < currentAsteroidCount; i++)
            {
                SpawnSingleAsteroid();

                // Delay nhỏ giữa các asteroid để không spawn cùng lúc
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void SpawnSingleAsteroid()
    {
        // Tạo vị trí spawn ngẫu nhiên
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Spawn asteroid
        GameObject newAsteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        // Set tốc độ cho asteroid
        AsteroidController asteroidController = newAsteroid.GetComponent<AsteroidController>();
        if (asteroidController != null)
        {
            // Set tốc độ hiện tại với một chút biến thiên ngẫu nhiên
            float randomSpeedVariation = Random.Range(-0.5f, 0.5f);
            asteroidController.speed = currentAsteroidSpeed + randomSpeedVariation;

            // Đảm bảo tốc độ không âm
            asteroidController.speed = Mathf.Max(1f, asteroidController.speed);
        }

        // Hủy thiên thạch sau X giây nếu chưa va chạm
        Destroy(newAsteroid, destroyAfterSeconds);
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Tạo vị trí X ngẫu nhiên trong phạm vi
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);

        // Thêm một chút biến thiên cho Y để asteroid không spawn cùng lúc ở cùng độ cao
        float randomY = spawnHeight + Random.Range(-0.5f, 0.5f);

        return new Vector3(randomX, randomY, 0f);
    }

    // ========== PLAYER & GAME STATE MANAGEMENT ==========

    // Hàm giảm mạng người chơi và kích hoạt shield
    public void LoseLife()
    {
        HealthManager.health--; // Giảm trực tiếp số mạng trong HealthManager
        UpdateLivesText(); // Cập nhật UI

        if (HealthManager.health <= 0)
        {
            Invoke("GameOver", 0.1f); // Gọi hàm GameOver sau 0.1 giây để tránh lỗi khi đang trong quá trình giảm mạng
        }
        else if (playerShield != null)
        {
            // Kích hoạt shield khi mất mạng nhưng vẫn còn mạng
            playerShield.ActivateShield();
        }
    }

    // Cập nhật hiển thị số mạng
    void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + HealthManager.health.ToString();
        }
    }

    // Xử lý game over
    void GameOver()
    {
        Debug.Log("Game Over!");
        gameStarted = false; // Dừng spawn

        if (playerObject != null)
        {
            Destroy(playerObject);
        }
        Time.timeScale = 0f; // Dừng game
        // Có thể thêm code hiển thị màn hình game over ở đây
    }

    // Hàm restart game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        HealthManager.health = 3;  // Thay playerLives thành HealthManager.health
        score = 0;

        // Reset các giá trị difficulty
        InitializeGame();

        UpdateLivesText();
        UpdateScoreText();

        // Xóa tất cả asteroid hiện tại
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }

        // Restart spawn coroutine
        StartCoroutine(SpawnAsteroids());

        Debug.Log("Game đã được restart!");
    }

    // ========== SCORE MANAGEMENT ==========

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text is not assigned in the GameManager.");
        }
    }

    // Thêm phương thức để chuyển đổi giữa tạm dừng và tiếp tục
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

    public void PauseGame()
    {
        Time.timeScale = 0; // Dừng game
        pauseMenuScreen.SetActive(true);
        isPaused = true;
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

    // ========== GETTER METHODS ==========

    public int GetCurrentScore()
    {
        return score;
    }

    public int GetCurrentLives()
    {
        return HealthManager.health;  // Thay playerLives thành HealthManager.health
    }

    public int GetCurrentDifficultyLevel()
    {
        return difficultyLevel;
    }

    public int GetCurrentAsteroidCount()
    {
        return currentAsteroidCount;
    }

    public float GetCurrentSpeed()
    {
        return currentAsteroidSpeed;
    }

    public float GetCurrentSpawnInterval()
    {
        return currentSpawnInterval;
    }
}
