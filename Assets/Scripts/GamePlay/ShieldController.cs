using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] private float shieldDuration = 3f; // Thời gian kích hoạt shield (giây)
    [SerializeField] private GameObject shieldVisual; // Game object hiển thị hình ảnh shield
    [SerializeField] private AudioClip shieldActivateSound; // Âm thanh khi shield được kích hoạt
    [SerializeField] private AudioClip shieldDeactivateSound; // Âm thanh khi shield tắt

    private float shieldActiveTimer = 0f; // Đếm thời gian shield được kích hoạt
    private bool isShieldActive = false; // Trạng thái hiện tại của shield
    private AudioSource audioSource; // Component audio source

    private void Awake()
    {
        // Tìm audio source hoặc tạo mới nếu cần
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Đảm bảo shield ban đầu không được kích hoạt
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private void Update()
    {
        // Kiểm tra nếu shield đang kích hoạt
        if (isShieldActive)
        {
            // Đếm ngược thời gian
            shieldActiveTimer -= Time.deltaTime;

            // Kiểm tra nếu hết thời gian
            if (shieldActiveTimer <= 0)
            {
                DeactivateShield();
            }
        }
    }    // Kích hoạt shield
    public void ActivateShield()
    {
        // Nếu shield đã đang hoạt động, reset thời gian
        if (isShieldActive)
        {
            shieldActiveTimer = shieldDuration;
            return;
        }

        isShieldActive = true;
        shieldActiveTimer = shieldDuration;

        // Hiển thị shield
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        // Phát âm thanh kích hoạt shield
        if (audioSource != null && shieldActivateSound != null)
        {
            audioSource.clip = shieldActivateSound;
            audioSource.Play();
        }

        Debug.Log("Shield activated! Duration: " + shieldDuration + " seconds");
    }

    // Tắt shield
    public void DeactivateShield()
    {
        if (!isShieldActive)
            return;

        isShieldActive = false;

        // Ẩn shield
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        // Phát âm thanh tắt shield
        if (audioSource != null && shieldDeactivateSound != null)
        {
            audioSource.clip = shieldDeactivateSound;
            audioSource.Play();
        }

        Debug.Log("Shield deactivated!");
    }

    // Kiểm tra xem shield có đang hoạt động không
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
