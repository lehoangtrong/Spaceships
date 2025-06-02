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

    private void Start()
    {
        StartCoroutine(SpawnAsteroids());
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
