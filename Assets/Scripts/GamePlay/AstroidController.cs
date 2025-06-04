using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 1f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 10f; // Tốc độ tối đa
    private float timer = 0f;
    public float interval = 5f; // thời gian mỗi lần tăng
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 100f);

    [Header("Health System")]
    public int maxHealth = 3; // Số lần bắn để phá hủy
    private int currentHealth;

    [Header("Pushback Effect")]
    public float pushbackForce = 2f; // Lực đẩy lùi
    public float pushbackDuration = 0.3f; // Thời gian bị đẩy lùi
    private bool isPushedBack = false;
    private float pushbackTimer = 0f;
    private Vector3 pushbackDirection;

    [Header("Visual Effects")]
    public GameObject explosionAndFlarePrefab; // Hiệu ứng nổ
    public GameObject hitEffectPrefab; // Hiệu ứng khi bị bắn (không nổ)
    public GameObject starPrefab;              // ngôi sao để spawn sau nổ

    [Header("Audio")]
    public AudioClip explosionSound;  // Âm thanh nổ
    public AudioClip hitSound;        // Âm thanh khi bị bắn nhưng chưa nổ
    public AudioClip dieSound;        // Âm thanh thăng thiên
    private AudioSource audioSource;

    [Header("Game Settings")]
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
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Khởi tạo health
        currentHealth = maxHealth;

        // Lưu màu gốc và giữ nguyên
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // Xử lý pushback
        if (isPushedBack)
        {
            pushbackTimer -= Time.deltaTime;

            // Di chuyển theo hướng pushback
            transform.Translate(pushbackDirection * pushbackForce * Time.deltaTime, Space.World);

            if (pushbackTimer <= 0f)
            {
                isPushedBack = false;
            }
        }
        else
        {
            // Di chuyển bình thường
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
        }

        // Xoay quanh chính nó
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        // Tăng tốc độ theo thời gian
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
        //Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Laser"))
        {
            // Xử lý bị bắn bởi laser
            HandleLaserHit(collision);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision();
        }
    }

    private void HandleLaserHit(Collision2D collision)
    {
        // Giảm health
        currentHealth--;

        if (currentHealth <= 0)
        {
            // Check if this is a splitting asteroid (Round 3)
            //if (gameObject.CompareTag("SplittingAsteroid"))
            //{
            //    // Spawn small asteroids
            //    if (GameManager.Instance != null)
            //    {
            //        GameManager.Instance.SpawnSmallAsteroids(transform.position);
            //    }
            //}
            // Asteroid nổ - tự cộng điểm và explode
            AddScoreToGameManager();
            Explode();
        }
        else
        {
            // Asteroid chưa nổ - pushback và hiệu ứng hit
            ApplyPushback(collision.contacts[0].point);
            ShowHitEffect();
            PlayHitSound();
        }

        // Hủy laser
        Destroy(collision.gameObject);
    }

    private void ApplyPushback(Vector3 hitPoint)
    {
        // Tính hướng pushback (ngược với hướng laser đến)
        pushbackDirection = (transform.position - hitPoint).normalized;

        // Chỉ pushback theo trục X và Y, không theo Z
        pushbackDirection.z = 0;

        // Kích hoạt pushback
        isPushedBack = true;
        pushbackTimer = pushbackDuration;

        //Debug.Log($"Asteroid bị pushback, health còn: {currentHealth}");
    }

    private void ShowHitEffect()
    {
        // Tạo hiệu ứng hit (khác với explosion)
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Flash effect khi bị hit
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        // Flash màu trắng rồi trở về màu gốc
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // Trở về màu gốc
    }

    private void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            PlaySoundAtPosition(hitSound, transform.position, 0.7f);
        }
    }

    private void HandlePlayerCollision()
    {
        // Tự kiểm tra shield và xử lý va chạm với player
        CheckPlayerShieldAndDamage();

        // Tự phát âm thanh nổ
        PlayExplosionSound();

        // Tự tạo hiệu ứng nổ
        CreateExplosionEffect();

        // Tự spawn ngôi sao với xác suất
        TrySpawnStar();

        // Tự phát âm thanh thăng thiên
        PlayDieSound();

        // Tự hủy
        Destroy(this.gameObject);
    }

    private void CheckPlayerShieldAndDamage()
    {
        // Lấy thông tin shield từ GameManager (vẫn cần GameManager để lấy player reference)
        if (GameManager.Instance != null && GameManager.Instance.playerShield != null &&
            GameManager.Instance.playerShield.IsShieldActive())
        {
            // Nếu shield đang hoạt động, không làm gì cả
            Debug.Log("Player shield is active, no life lost.");
        }
        else
        {
            // Nếu không có shield hoặc shield không hoạt động, giảm mạng
            Debug.Log("Player shield is not active, life lost.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    private void AddScoreToGameManager()
    {
        // Tự cộng điểm vào GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
    }

    private void PlayExplosionSound()
    {
        if (explosionSound != null && audioSource != null)
        {
            PlaySoundAtPosition(explosionSound, transform.position, 1f);
        }
    }

    private void CreateExplosionEffect()
    {
        if (explosionAndFlarePrefab != null)
        {
            Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);
        }
    }

    private void TrySpawnStar()
    {
        if (starPrefab != null && Random.value < starSpawnChance)
        {
            Instantiate(starPrefab, transform.position, Quaternion.identity);
        }
    }

    private void PlayDieSound()
    {
        if (dieSound != null && audioSource != null)
        {
            PlaySoundAtPosition(dieSound, transform.position, 1f);
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

    //void Explode()
    //{
    //    // Tự tạo hiệu ứng nổ
    //    CreateExplosionEffect();

    //    // Tự spawn ngôi sao với xác suất
    //    TrySpawnStar();

    //    // Tự phát âm thanh nổ
    //    PlayExplosionSound();

    //    // Kiểm tra xem trước khi xóa hẳn thì có nên xuất hiện thiên thạch con không
    //    if (splitOnDeath && childAsteroidPrefab != null)
    //    {
    //        SplitIntoChildren();
    //    }

    //    // Tự hủy
    //    Destroy(gameObject);
    //}

    void Explode()
    {
        // Tự tạo hiệu ứng nổ
        CreateExplosionEffect();

        // Tự spawn ngôi sao với xác suất
        TrySpawnStar();

        // Tự phát âm thanh nổ
        PlayExplosionSound();

        // 💥 Chỉ phân chia nếu đang ở Round 3
        if (splitOnDeath &&
            childAsteroidPrefab != null &&
            GameManager.Instance != null &&
            GameManager.Instance.GetCurrentRound() == 3)
        {
            //Debug.Log("Đang ở Round 3 → tạo thiên thạch con");
            SplitIntoChildren();
        }
        else
        {
            //Debug.Log("Không tạo thiên thạch con. splitOnDeath=" + splitOnDeath +
            //          ", childAsteroidPrefab null=" + (childAsteroidPrefab == null) +
            //          ", Round=" + GameManager.Instance?.GetCurrentRound());
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

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDestroyed()
    {
        return currentHealth <= 0;
    }

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