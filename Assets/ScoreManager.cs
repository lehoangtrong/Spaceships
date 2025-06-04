using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (scoreValue != null)
        {
            scoreValue.text = GameManager.score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Value is not assigned in the GameManager.");
        }

    }
}
