using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;
    public GameObject explosionAndFlarePrefab; // Hiệu ứng nổ
    public GameObject starPrefab;              // ngôi sao để spawn sau nổ

    public static int playerLife = 3; // Biến tĩnh để theo dõi số mạng của người chơi
    public AudioClip explosionSound;  // Âm thanh nổ
    public AudioClip dieSound;        //Âm thanh thăng thiên
    private AudioSource audioSource;

    public int asteroidSpawnCount = 5; // Số lượng asteroid sẽ được sinh ra mỗi lần

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        for (int i = 0; i < asteroidSpawnCount; i++)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Explode();
            Destroy(collision.gameObject); //lỏ
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
        if (starPrefab != null)
            Instantiate(starPrefab, transform.position, Quaternion.identity);  // Spawn ngôi sao

        Destroy(gameObject);
    }
}
