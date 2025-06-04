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
        AsteroidController.playerLife = 3; // Reset player life to 3
        SceneManager.LoadScene("GamePlay");
    }
}
