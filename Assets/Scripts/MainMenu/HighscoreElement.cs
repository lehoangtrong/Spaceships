using UnityEngine;
using System;

[Serializable]
public class HighscoreElement
{
    public string playerName;
    public int score;

    public HighscoreElement(string name, int score)
    {
        playerName = name;
        this.score = score;
    }

}

