using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreHandler : MonoBehaviour
{
    List<HighscoreElement> highscoreList = new List<HighscoreElement>();
    [SerializeField] int maxCount = 5;
    [SerializeField] string filename = "highscores.json";

    public delegate void OnHighscoreListChanged(List<HighscoreElement> list);
    public static event OnHighscoreListChanged onHighscoreListChanged;

    // ✅ FIX: Thêm property để truy cập danh sách highscore
    public List<HighscoreElement> CurrentHighscoreList
    {
        get { return new List<HighscoreElement>(highscoreList); }
    }

    private void Awake()
    {
        LoadHighscores();

    }
    //private void LoadHighscores()
    //{
    //    highscoreList = FileHandler.ReadListFromJSON<HighscoreElement>(filename);

    //    while (highscoreList.Count > maxCount)
    //    {
    //        highscoreList.RemoveAt(maxCount);
    //    }

    //    if (onHighscoreListChanged != null)
    //    {
    //        onHighscoreListChanged.Invoke(highscoreList);
    //    }
    //}

    private void LoadHighscores()
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                Debug.LogError("Filename is not set for HighscoreHandler!");
                filename = "highscores.json";
            }

            highscoreList = FileHandler.ReadListFromJSON<HighscoreElement>(filename);

            while (highscoreList.Count > maxCount)
            {
                highscoreList.RemoveAt(maxCount);
            }

            if (onHighscoreListChanged != null)
            {
                onHighscoreListChanged.Invoke(highscoreList);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load highscores: {e.Message}");
            highscoreList = new List<HighscoreElement>();
        }
    }

    //private void SaveHighscore()
    //{
    //    FileHandler.SaveToJSON<HighscoreElement>(highscoreList, filename);
    //}
    private void SaveHighscore()
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                Debug.LogError("Cannot save: filename is not set!");
                return;
            }

            FileHandler.SaveToJSON<HighscoreElement>(highscoreList, filename);
            Debug.Log($"Highscores saved successfully to {filename}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save highscores: {e.Message}");
        }
    }

    public int ScoreIndex(int score)
    {
        if (highscoreList.Count == 0)
            return 0;

        for (int i = 0; i < highscoreList.Count; i++)
        {
            if (highscoreList[i].score < score)
            {
                return i;
            }
        }

        if (highscoreList.Count < maxCount)
        {
            return highscoreList.Count;
        }

        return -1;
    }

    public void AddHighscore(int index, HighscoreElement element)
    {
        if (index < 0 || index >= maxCount && highscoreList.Count >= maxCount)
        {
            Debug.LogError($"Index out of bounds for highscore list. Index: {index}, Count: {highscoreList.Count}, MaxCount: {maxCount}");
            return;
        }
        if (index >= highscoreList.Count)
        {
            highscoreList.Add(element);
        }
        else
        {
            highscoreList.Insert(index, element);
        }

        while (highscoreList.Count > maxCount)
        {
            highscoreList.RemoveAt(maxCount);
        }

        SaveHighscore();

        if (onHighscoreListChanged != null)
        {
            onHighscoreListChanged.Invoke(highscoreList);
        }
    }



    public void AddHighscoreIfPossible(HighscoreElement element)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (i >= highscoreList.Count || element.score > highscoreList[i].score)
            {
                // add new high score
                highscoreList.Insert(i, element);

                while (highscoreList.Count > maxCount)
                {
                    highscoreList.RemoveAt(maxCount);
                }

                SaveHighscore();

                if (onHighscoreListChanged != null)
                {
                    onHighscoreListChanged.Invoke(highscoreList);
                }

                break;
            }
        }
    }

}