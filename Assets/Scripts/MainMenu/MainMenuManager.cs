using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện quản lý cảnh

public class MainMenuManager : MonoBehaviour
{
    // SerializeField cho phép gán giá trị trực tiếp mà không cần public 
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject recordTablePanel;

    void Start()
    {
        // Ẩn Panel khi bắt đầu
        instructionsPanel.SetActive(false);
        recordTablePanel.SetActive(false);

    }

    public void StartGame()
    {

        Debug.Log("Start Game");
        GameManager.score = 0; // Reset score to 0
        Time.timeScale = 1f;
        AsteroidController.playerLife = 3; // Reset player life to 3
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
