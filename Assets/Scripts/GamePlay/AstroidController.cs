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

            if (GameManager.Instance.playerShield != null && GameManager.Instance.playerShield.IsShieldActive())
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

        Destroy(gameObject);
    }
}
