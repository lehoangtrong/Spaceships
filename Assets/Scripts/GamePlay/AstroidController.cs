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

    public static int playerLife = 3; // Biến tĩnh để theo dõi số mạng của người chơi
    public AudioClip explosionSound;  // Âm thanh nổ
    public AudioClip dieSound;        //Âm thanh thăng thiên
    private AudioSource audioSource;

    public int asteroidSpawnCount = 5; // Số lượng asteroid sẽ được sinh ra mỗi lần

    public int scoreValue = 1;
    [Range(0f, 1f)]
    public float starSpawnChance = 0.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

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
        if (collision.gameObject.CompareTag("Laser") || collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue);
            Explode();
            // Destroy(collision.gameObject); //lỏ
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (explosionSound != null && audioSource != null)
            {
                PlaySoundAtPosition(explosionSound, transform.position, 1f);
            }

            Destroy(this.gameObject);
            playerLife--; // Giảm số mạng của người chơi khi va chạm với asteroid

            if (playerLife <= 0)
            {
                if (dieSound != null && audioSource != null)
                {
                    PlaySoundAtPosition(dieSound, transform.position, 1f);
                }

                Debug.Log("Avenger end game");
                Destroy(collision.gameObject); // Xóa người chơi
                Time.timeScale = 0f;
                SceneManager.LoadScene("EndGame");
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
