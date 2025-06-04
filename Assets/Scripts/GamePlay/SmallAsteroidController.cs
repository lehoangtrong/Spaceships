using UnityEngine;

public class SmallAsteroidController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float playerTrackingForce = 0.5f; // How much it follows the player
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 150f);

    [Header("Visual Effects")]
    public GameObject explosionAndFlarePrefab;
    public GameObject starPrefab;

    [Header("Audio")]
    public AudioClip explosionSound;
    private AudioSource audioSource;

    [Header("Game Settings")]
    public int scoreValue = 2; // Small asteroids give more points
    [Range(0f, 1f)]
    public float starSpawnChance = 0.3f;

    private Vector3 initialDirection;
    private GameObject player;
    private Vector3 currentDirection;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Make small asteroids smaller
        transform.localScale = Vector3.one * 0.6f;
    }

    public void Initialize(Vector3 direction, GameObject playerObject)
    {
        initialDirection = direction.normalized;
        currentDirection = initialDirection;
        player = playerObject;
    }

    void Update()
    {
        UpdateMovement();

        // Rotate
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }

    void UpdateMovement()
    {
        if (player != null)
        {
            // Calculate direction to player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            // Blend initial direction with player direction
            currentDirection = Vector3.Lerp(currentDirection, directionToPlayer, playerTrackingForce * Time.deltaTime);
            currentDirection.Normalize();
        }

        // Move in the current direction
        transform.Translate(currentDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            // Small asteroids are destroyed in one hit
            HandleLaserHit(collision);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision();
        }
    }

    private void HandleLaserHit(Collision2D collision)
    {
        // Add score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Destroy laser
        Destroy(collision.gameObject);

        // Explode
        Explode();
    }

    private void HandlePlayerCollision()
    {
        // Check shield and damage player
        CheckPlayerShieldAndDamage();

        // Explode
        Explode();
    }

    private void CheckPlayerShieldAndDamage()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerShield != null &&
            GameManager.Instance.playerShield.IsShieldActive())
        {
            Debug.Log("Player shield blocked small asteroid.");
        }
        else
        {
            Debug.Log("Small asteroid hit player.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    void Explode()
    {
        // Create explosion effect
        if (explosionAndFlarePrefab != null)
        {
            Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);
        }

        // Spawn star with chance
        if (starPrefab != null && Random.value < starSpawnChance)
        {
            Instantiate(starPrefab, transform.position, Quaternion.identity);
        }

        // Play explosion sound
        if (explosionSound != null)
        {
            PlaySoundAtPosition(explosionSound, transform.position, 0.8f);
        }

        // Destroy this object
        Destroy(gameObject);
    }

    private void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 0f;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}