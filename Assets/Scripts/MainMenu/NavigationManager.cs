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
        AsteroidController asteroidController = FindObjectOfType<AsteroidController>();
        Debug.Log("Starting Game Again");
        // Load the gameplay scene
        GameManager.score = 0; // Reset score to 0
        Time.timeScale = 1f;
        asteroidController.maxHealth = 3; // Reset player life to 3
        SceneManager.LoadScene("GamePlay");
    }
}
