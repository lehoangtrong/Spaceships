using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void ReturnHomeScreen()
    {
        Debug.Log("Returning to Home Screen");
        // Load the main menu scene 
        SceneManager.LoadScene("MainMenu"); 
    }

    public void StartAgain()
    {
        Debug.Log("Starting Game Again");
        // Load the gameplay scene
        GameManager.score = 0; // Reset score to 0
        Time.timeScale = 1f;
        SceneManager.LoadScene("GamePlay");
    }
}
