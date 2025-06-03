using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Mạng (Lives)")]
    public int maxLives = 3;           // Số mạng ban đầu
    private int currentLives;          // Mạng hiện tại
    public GameObject heartPrefab;     // Prefab hình trái tim
    public Transform livesContainer;   // Container UI để chứa các trái tim
    private List<GameObject> hearts = new List<GameObject>();

    [Header("Prefab thiên thạch")]
    public GameObject asteroidPrefab;

    [Header("Khoảng thời gian giữa các lần spawn (giây)")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;

    [Header("Giới hạn trục X để spawn thiên thạch")]
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Thời gian tự hủy thiên thạch")]
    public float destroyAfterSeconds = 10f;

    [Header("Điểm (Score)")]
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int[] scoreThresholds = { 100, 200, 300, 500 };
    public SpriteRenderer backgroundRenderer;
    public Sprite[] levelBackgrounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Nếu bạn muốn giữ manager khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLives = maxLives;
    }

    private void Start()
    {
        UpdateLivesUI();
        UpdateScoreUI();
        StartCoroutine(SpawnAsteroids());
        UpdateLevel(); // Cập nhật background lúc đầu
    }

    public void LoseLife()
    {
        if (currentLives <= 0) return;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    void UpdateLivesUI()
    {
        // Xóa hết các trái tim cũ
        foreach (var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        // Tạo lại trái tim theo số mạng hiện tại
        for (int i = 0; i < currentLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, livesContainer);
            hearts.Add(heart);
        }
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), 6f, 0f);
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

            Destroy(asteroid, destroyAfterSeconds);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        CheckLevelUp();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("000");
        }
    }

    void CheckLevelUp()
    {
        if (currentLevel < scoreThresholds.Length && score >= scoreThresholds[currentLevel - 1])
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        Debug.Log("Level up! Now level: " + currentLevel);

        if (backgroundRenderer != null && currentLevel - 1 < levelBackgrounds.Length)
        {
            backgroundRenderer.sprite = levelBackgrounds[currentLevel - 1];
        }

        // Tăng độ khó (giảm delay spawn)
        minSpawnDelay = Mathf.Max(0.1f, minSpawnDelay - 0.1f);
        maxSpawnDelay = Mathf.Max(0.5f, maxSpawnDelay - 0.2f);
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        // Ví dụ tạm thời: dừng thời gian chơi
        Time.timeScale = 0f;
        // Bạn có thể thêm UI báo game over hoặc chuyển scene...
    }
}
