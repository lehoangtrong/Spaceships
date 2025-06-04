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
                PlaySoundAtPosition(starSource, transform.position, 1f);
            }

            Debug.Log("Star collected!");
            GameManager.Instance.AddScore(pointValue);
            Destroy(gameObject); // Destroy the star object
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
}
