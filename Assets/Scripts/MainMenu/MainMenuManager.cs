using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện quản lý cảnh
using UnityEngine.UI; // Thư viện UI để sử dụng các thành phần giao diện người dùng
using UnityEngine.Audio; // Thư viện âm thanh để quản lý âm lượng và âm thanh
public class MainMenuManager : MonoBehaviour
{
    // SerializeField cho phép gán giá trị trực tiếp mà không cần public 
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject recordTablePanel;

    public Slider volumeSlider; // Slider để điều chỉnh âm lượng
    public AudioMixer audioMixer; // AudioMixer để quản lý âm lượng

    public void SetVolume()
    {
        audioMixer.SetFloat("volume", volumeSlider.value);
    }

    void Start()
    {
        // Ẩn Panel khi bắt đầu
        instructionsPanel.SetActive(false);
        recordTablePanel.SetActive(false);

    }
    public void StartGame()
    {
        Debug.Log("Start Game");

        // Reset game state through GameManager if it exists
        if (GameManager.Instance != null)
        {
            GameManager.score = 0; // Reset score to 0
            GameManager.Instance.SetCurrentLives(3); // Reset player lives to 3
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("GamePlay"); // Tên cảnh cần load
    }

    public void ShowInstructions()
    {
        Debug.Log("Show Instructions");
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Instructions panel is not assigned in the inspector.");
        }
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Instructions panel is not assigned in the inspector.");
        }
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
