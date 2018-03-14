using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData {    
    public string playerInitials;
    public int playerScore;

    public ScoreData(string initials, int score)
    {
        playerInitials = initials;
        playerScore = score;
    }
}
