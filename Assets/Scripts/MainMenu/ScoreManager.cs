using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreValue;
    private HighscoreHandler highscoreHandler;
    private int scoreIndex = -1;

    [SerializeField] GameObject enterNamePanel;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject highscoreUIElementPrefab;
    [SerializeField] Transform elementWrapper;
    [SerializeField] TMP_InputField nameInputField;

    List<GameObject> uiElements = new List<GameObject>();

    public void Start()
    {
        int score = GameManager.score;

        if (scoreValue != null)
        {
            scoreValue.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Value is not assigned in the GameManager.");
        }

        highscoreHandler = FindObjectOfType<HighscoreHandler>();
        if (highscoreHandler == null)
        {
            Debug.LogError("HighscoreHandler not found in scene!");
            return;
        }

        scoreIndex = highscoreHandler.ScoreIndex(score);

        ShowPanel();
        TryShowEnterNamePanel(scoreIndex);
    }

    public void OnSaveButtonClick()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty. Please enter a valid name.");
            return;
        }

        if (highscoreHandler == null)
        {
            Debug.LogError("HighscoreHandler is not initialized.");
            return;
        }

        HighscoreElement newEntry = new HighscoreElement(playerName, GameManager.score);
        highscoreHandler.AddHighscore(scoreIndex, newEntry);
        //highscoreHandler.AddHighscoreIfPossible(newEntry); // ví dụ
        enterNamePanel.SetActive(false);
    }

    private void TryShowEnterNamePanel(int scoreIndex)
    {
        if (scoreIndex >= 0 && scoreIndex < 5)
        {
            Debug.Log("Score is high enough to enter the leaderboard.");
            enterNamePanel.SetActive(true);
        }
        else
        {
            Debug.Log("Score is not high enough to enter the leaderboard.");
            enterNamePanel.SetActive(false);
        }
    }

    ////////////////////////


    private void OnEnable()
    {
        HighscoreHandler.onHighscoreListChanged += UpdateUI;
    }

    private void OnDisable()
    {
        HighscoreHandler.onHighscoreListChanged -= UpdateUI;
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    private void UpdateUI(List<HighscoreElement> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            HighscoreElement el = list[i];

            if (el != null && el.score >= 0)
            {
                if (i >= uiElements.Count)
                {
                    // instantiate new entry
                    var inst = Instantiate(highscoreUIElementPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(elementWrapper, false);

                    uiElements.Add(inst);
                }

                // write or overwrite name & score
                var texts = uiElements[i].GetComponentsInChildren<TextMeshProUGUI>();

                if (texts.Length < 3)
                {
                    Debug.LogError($"UI Element at index {i} is missing text components.");
                    continue;
                }
                int rank = i + 1;
                texts[0].text = rank.ToString();
                texts[1].text = el.playerName;
                texts[2].text = el.score.ToString();
            }
        }
    }

}

