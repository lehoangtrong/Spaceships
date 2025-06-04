using UnityEngine;

public class StarController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip starSource;
    private float fallSpeed = 1f;
    private float destroyAfterSeconds = 10f;
    public int pointValue = 10;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (audioSource != null && starSource != null)
            {
                GameAudioManager.PlaySoundThroughMixer(starSource, transform.position, 1f);
            }

            Debug.Log("Star collected!");
            GameManager.Instance.AddScore(pointValue);
            Destroy(gameObject); // Destroy the star object
        }
    }
}
