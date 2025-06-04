using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float cooldownTime = 0.2f; // Thời gian giữa các lần bắn
    public bool autoFire = false; // Bắn tự động khi giữ nút
    public AudioClip laserSound; // Âm thanh khi bắn

    [Header("Input Settings")]
    public bool useMouseInput = true;
    public bool useKeyboardInput = true;

    private GameObject currentLaser;
    private float cooldownTimer = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        // Tạo audio source nếu cần
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && laserSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.7f;
        }
    }

    void Update()
    {
        // Cập nhật thời gian cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Kiểm tra input để bắn
        bool fireInput = (useMouseInput && Input.GetMouseButton(0)) ||
                         (useKeyboardInput && Input.GetKey(KeyCode.Space));

        // Kiểm tra input down cho bắn một phát
        bool fireDownInput = (useMouseInput && Input.GetMouseButtonDown(0)) ||
                             (useKeyboardInput && Input.GetKeyDown(KeyCode.Space));

        // Kiểm tra input up để dừng bắn
        bool fireUpInput = (useMouseInput && Input.GetMouseButtonUp(0)) ||
                           (useKeyboardInput && Input.GetKeyUp(KeyCode.Space));

        // Xử lý bắn tự động khi giữ nút
        if (autoFire && fireInput)
        {
            if (cooldownTimer <= 0)
            {
                FireLaser();
            }
        }
        // Xử lý bắn một phát khi nhấn nút (không tự động)
        else if (!autoFire && fireDownInput)
        {
            FireLaser();
        }

        // Dừng bắn laser khi thả nút
        if (fireUpInput && currentLaser != null)
        {
            Destroy(currentLaser);
            currentLaser = null;
        }
    }

    // Hàm bắn laser
    void FireLaser()
    {
        // Tạo laser mới
        GameObject newLaser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // Gắn laser vào firePoint để di chuyển cùng phi thuyền
        newLaser.transform.parent = firePoint;
        newLaser.transform.localPosition = Vector3.zero;
        newLaser.transform.localRotation = Quaternion.identity;

        // Nếu đang bắn liên tục, lưu trữ laser hiện tại
        if (autoFire)
        {
            if (currentLaser != null)
            {
                Destroy(currentLaser);
            }
            currentLaser = newLaser;
            currentLaser.transform.parent = firePoint;
            currentLaser.transform.rotation = Quaternion.identity;
        }
        else
        {
            // Lưu trữ laser hiện tại ngay cả khi không phải autoFire
            if (currentLaser != null)
            {
                Destroy(currentLaser);
            }
            currentLaser = newLaser;
            Destroy(newLaser, 2f); // Tự hủy sau 2 giây nếu không va chạm
        }

        // Phát âm thanh
        if (laserSound != null && audioSource != null)
        {
            //audioSource.PlayOneShot(laserSound);
            PlaySoundAtPosition(laserSound, transform.position, 1f);
        }

        // Reset thời gian cooldown
        cooldownTimer = cooldownTime;
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
