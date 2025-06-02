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

    private void Awake()
    {
        // Singleton để các script khác gọi GameManager.Instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
        StartCoroutine(SpawnAsteroids());
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
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("000");
        }
    }
}
