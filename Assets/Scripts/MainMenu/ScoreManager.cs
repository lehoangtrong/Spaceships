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
    [SerializeField] GameObject UiButtonPanel;
    [SerializeField] GameObject menuTextTitle;

    List<GameObject> uiElements = new List<GameObject>();

    private void Awake()
    {
        Debug.Log("ScoreManager: Awake - Registering event listener");
        HighscoreHandler.onHighscoreListChanged += UpdateUI;
    }

    public void Start()
    {
        Debug.Log("ScoreManager: Start");
        int score = GameManager.Instance != null ? GameManager.score : 0;

        if (scoreValue != null)
        {
            scoreValue.text = score.ToString();
            Debug.Log($"ScoreManager: Score displayed: {score}");
        }
        else
        {
            Debug.LogWarning("Score Value is not assigned in the GameManager.");
        }

        highscoreHandler = FindFirstObjectByType<HighscoreHandler>();
        if (highscoreHandler == null)
        {
            Debug.LogError("HighscoreHandler not found in scene!");
            return;
        }

        ShowPanel();

        scoreIndex = highscoreHandler.ScoreIndex(score);
        Debug.Log($"ScoreManager: Score index: {scoreIndex}");

        Debug.Log("Score Index: " + scoreIndex);
        TryShowEnterNamePanel(scoreIndex);

        StartCoroutine(DelayedUpdateUI());
    }

    private System.Collections.IEnumerator DelayedUpdateUI()
    {
        yield return null;

        Debug.Log("ScoreManager: Requesting initial UI update");
        if (highscoreHandler != null)
        {
            var currentList = GetCurrentHighscoreList();
            if (currentList != null)
            {
                Debug.Log($"ScoreManager: Found {currentList.Count} existing highscores");
                UpdateUI(currentList);
            }
        }
    }

    private List<HighscoreElement> GetCurrentHighscoreList()
    {
        if (highscoreHandler == null) return null;

        try
        {
            var list = highscoreHandler.CurrentHighscoreList;
            Debug.Log($"ScoreManager: GetCurrentHighscoreList - got {list?.Count ?? 0} records from HighscoreHandler");

            if (list == null || list.Count == 0)
            {
                Debug.Log("ScoreManager: List empty, trying to read directly from file");
                list = FileHandler.ReadListFromJSON<HighscoreElement>("highscores.json");
                Debug.Log($"ScoreManager: Read {list?.Count ?? 0} records directly from file");
            }

            return list ?? new List<HighscoreElement>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ScoreManager: Error loading highscores: {e.Message}");
            return new List<HighscoreElement>();
        }
    }

    public void ShowMenuRecords()
    {
        if (highscoreHandler == null)
        {
            highscoreHandler = FindFirstObjectByType<HighscoreHandler>();

            if (highscoreHandler == null)
            {
                Debug.LogError("HighscoreHandler not found in scene!");
                return;
            }
        }

        UiButtonPanel.SetActive(false);
        menuTextTitle.SetActive(false);
        ShowPanel();

        var currentList = GetCurrentHighscoreList();
        if (currentList != null)
        {
            Debug.Log($"ScoreManager: ShowMenuRecords - displaying {currentList.Count} records");
            UpdateUI(currentList);
        }
        else
        {
            Debug.LogWarning("ScoreManager: No highscore data available");
            UpdateUI(new List<HighscoreElement>());
        }
    }

    public void CloseMenuRecord()
    {
        UiButtonPanel.SetActive(true);
        menuTextTitle.SetActive(true);
        ClosePanel();
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

        HighscoreElement newEntry = new HighscoreElement(playerName, GameManager.Instance != null ? GameManager.score : 0);
        Debug.Log($"ScoreManager: Adding highscore - Name: {playerName}, Score: {(GameManager.Instance != null ? GameManager.score : 0)}");
        highscoreHandler.AddHighscore(scoreIndex, newEntry);
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

    private void OnDisable()
    {
        HighscoreHandler.onHighscoreListChanged -= UpdateUI;
    }

    private void OnDestroy()
    {
        HighscoreHandler.onHighscoreListChanged -= UpdateUI;
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
        Debug.Log("ScoreManager: Panel shown");
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        Debug.Log("ScoreManager: Panel closed");
    }

    private void UpdateUI(List<HighscoreElement> list)
    {
        Debug.Log($"ScoreManager: UpdateUI called with {list?.Count ?? 0} elements");

        if (highscoreUIElementPrefab == null)
        {
            Debug.LogError("ScoreManager: highscoreUIElementPrefab is not assigned!");
            return;
        }

        if (elementWrapper == null)
        {
            Debug.LogError("ScoreManager: elementWrapper is not assigned!");
            return;
        }

        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("ScoreManager: Highscore list is null or empty");
            foreach (var uiElement in uiElements)
            {
                if (uiElement != null)
                {
                    uiElement.SetActive(false);
                }
            }
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            HighscoreElement el = list[i];

            if (el != null && el.score >= 0)
            {
                //Debug.Log($"ScoreManager: Processing highscore {i}: {el.playerName} - {el.score}");

                if (i >= uiElements.Count)
                {
                    //Debug.Log($"ScoreManager: Creating new UI element for index {i}");
                    var inst = Instantiate(highscoreUIElementPrefab, elementWrapper);
                    if (inst == null)
                    {
                        Debug.LogError($"Failed to instantiate highscoreUIElementPrefab for index {i}");
                        continue;
                    }

                    uiElements.Add(inst);
                }

                uiElements[i].SetActive(true);

                var texts = uiElements[i].GetComponentsInChildren<TextMeshProUGUI>();

                if (texts.Length < 3)
                {
                    Debug.LogError($"UI Element at index {i} is missing text components. Found: {texts.Length}");
                    continue;
                }
                int rank = i + 1;
                texts[0].text = rank.ToString();
                texts[1].text = el.playerName;
                texts[2].text = el.score.ToString();
            }
        }

        for (int i = list.Count; i < uiElements.Count; i++)
        {
            if (uiElements[i] != null)
            {
                uiElements[i].SetActive(false);
            }
        }

        Debug.Log("ScoreManager: UpdateUI completed successfully");
    }

}

