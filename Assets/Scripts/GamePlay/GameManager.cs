using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    [Header("Score")]
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Player Lives")]
    public TextMeshProUGUI livesText; // UI Text hiển thị số mạng còn lại
    public GameObject playerObject; // Đối tượng người chơi
    public ShieldController playerShield; // Component Shield của player

    private void Start()
    {
        StartCoroutine(SpawnAsteroids());

        // Tìm player và shield controller nếu chưa được gán
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerObject != null)
        {
            playerShield = playerObject.GetComponent<ShieldController>();
        }

        // Cập nhật hiển thị số mạng ban đầu
        UpdateLivesText();
    }

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
        HealthManager.health = 3; // Đổi từ playerLives thành số cố định 3
    }

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
            // Sửa từ playerLives thành HealthManager.health
            livesText.text = "Lives: " + HealthManager.health.ToString();
        }
    }

    // Xử lý game over
    void GameOver()
    {
        Debug.Log("Game Over!");

        if (playerObject != null)
        {
            Destroy(playerObject);
        }

        Time.timeScale = 0f; // Dừng game
        // Có thể thêm code hiển thị màn hình game over ở đây
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            // Chờ thời gian ngẫu nhiên trước khi spawn tiếp
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            // Tạo thiên thạch tại vị trí ngẫu nhiên theo trục X
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), 6f, 0f);
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

            // Hủy thiên thạch sau X giây nếu chưa va chạm
            Destroy(asteroid, destroyAfterSeconds);
        }
    }

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
}
