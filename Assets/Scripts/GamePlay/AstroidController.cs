using UnityEngine;
using UnityEngine.SceneManagement;

public class AsteroidController : MonoBehaviour
{
    public float speed = 1f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 10f; // Tốc độ tối đa
    private float timer = 0f;
    public float interval = 5f; // thời gian mỗi lần tăng
    public GameObject explosionAndFlarePrefab; // Hiệu ứng nổ
    public GameObject starPrefab;              // ngôi sao để spawn sau nổ
    public AudioClip explosionSound;  // Âm thanh nổ
    public AudioClip dieSound;        //Âm thanh thăng thiên
    private AudioSource audioSource;
    public int asteroidSpawnCount = 5; // Số lượng asteroid sẽ được sinh ra mỗi lần

    public Vector3 rotationSpeed = new Vector3(0f, 0f, 100f);

    public int scoreValue = 1;
    [Range(0f, 1f)]
    public float starSpawnChance = 0.5f;

    [Header("Split Settings")]
    public bool splitOnDeath = false;
    public GameObject childAsteroidPrefab; // Prefab thiên thạch con
    public int numberOfChildren = 3;
    public float childSpreadAngle = 60f;
    public float childSpeedMultiplier = 1.2f;
    public float childScaleFactor = 0.5f;

    // Visual components
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Xoay quanh chính nó
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            speed += speedIncrement;
            speed = Mathf.Min(speed, maxSpeed); // Đảm bảo tốc độ không vượt quá tốc độ tối đa
            timer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Laser"))
        {
            PlaySoundAtPosition(explosionSound, transform.position, 1f); // Phát âm thanh nổ
            GameManager.Instance.AddScore(scoreValue);
            Explode();
            // Destroy(collision.gameObject); //lỏ
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.shieldController != null && GameManager.Instance.shieldController.IsShieldActive())
            {
                // Nếu shield đang hoạt động, không làm gì cả
                Debug.Log("Player shield is active, no life lost.");
            }
            else
            {
                // Nếu không có shield hoặc shield không hoạt động, giảm mạng
                Debug.Log("Player shield is not active, life lost.");
                GameManager.Instance.LoseLife();  // Gọi LoseLife để cập nhật UI và kiểm tra game over
            }

            // Nếu có âm thanh nổ, phát âm thanh tại vị trí của thiên thạch
            if (explosionSound != null && audioSource != null)
            {
                PlaySoundAtPosition(explosionSound, transform.position, 1f);
            }

            // Xóa thiên thạch sau khi xử lý va chạm
            Destroy(this.gameObject);

            // Nếu có hiệu ứng nổ, tạo hiệu ứng nổ tại vị trí của thiên thạch
            if (explosionAndFlarePrefab != null)
            {
                Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);
            }

            // Nếu có ngôi sao, tạo ngôi sao tại vị trí của thiên thạch
            if (starPrefab != null && Random.value < starSpawnChance)
            {
                Instantiate(starPrefab, transform.position, Quaternion.identity);  // Spawn ngôi sao
            }

            // Nếu có âm thanh thăng thiên, phát âm thanh tại vị trí của thiên thạch
            if (dieSound != null && audioSource != null)
            {
                PlaySoundAtPosition(dieSound, transform.position, 1f);
            }
        }
    }

    private void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 0f; // 0 = 2D sound
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    void Explode()
    {
        if (explosionAndFlarePrefab != null)
            Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);

        // tạo ra ngôi sao
        float randomChance = Random.value;
        if (starPrefab != null && Random.value < starSpawnChance)
            Instantiate(starPrefab, transform.position, Quaternion.identity);  // Spawn ngôi sao

        // Tự phát âm thanh nổ
        if (explosionSound != null && audioSource != null)
        {
            PlaySoundAtPosition(explosionSound, transform.position, 1f);
        }

        // 💥 Chỉ phân chia nếu đang ở Round 3
        if (splitOnDeath &&
            childAsteroidPrefab != null &&
            GameManager.Instance != null &&
            GameManager.Instance.GetCurrentRound() == 3)
        {
            SplitIntoChildren();
        }

        // Tự hủy
        Destroy(gameObject);
    }

    // phương thức xuất hiện thiên thách
    void SplitIntoChildren()
    {
        for (int i = 0; i < numberOfChildren; i++)
        {
            // Tính hướng phân tán ngẫu nhiên theo vòng tròn
            float angle = (i - (numberOfChildren - 1) / 2f) * childSpreadAngle / (numberOfChildren - 1);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 direction = rotation * Vector3.down;

            // Tạo thiên thạch con
            GameObject child = Instantiate(childAsteroidPrefab, transform.position, Quaternion.identity);

            // Giảm kích thước thiên thạch con
            child.transform.localScale = transform.localScale * childScaleFactor;

            // Thiết lập hướng và tốc độ
            AsteroidController childCtrl = child.GetComponent<AsteroidController>();
            if (childCtrl != null)
            {
                childCtrl.SetSpeed(speed * childSpeedMultiplier);
                childCtrl.rotationSpeed = rotationSpeed * 1.5f; // có thể cho xoay nhanh hơn
            }

            // Đẩy nhẹ thiên thạch con ra ngoài
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * speed * childSpeedMultiplier;
            }
        }
    }

    // ========== PUBLIC METHODS FOR EXTERNAL ACCESS ==========

    // Method để GameManager có thể set tốc độ khi spawn
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Method để lấy tốc độ hiện tại
    public float GetCurrentSpeed()
    {
        return speed;
    }
}
