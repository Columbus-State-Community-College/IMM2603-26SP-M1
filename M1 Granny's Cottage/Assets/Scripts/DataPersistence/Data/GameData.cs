using TMPro;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Score / Sunk Cost Data
    public int highScore;
    public int pointBank;

    // This constructor initializes the new game game data state
    public GameData()
    {
        this.highScore = 0;
        this.pointBank = 0;

    }

}
