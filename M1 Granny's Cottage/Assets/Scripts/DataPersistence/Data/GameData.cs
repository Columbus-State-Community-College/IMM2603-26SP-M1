using UnityEngine;

[System.Serializable]
public class GameData
{
    public int highScore;

    // This constructor initializes the new game game data state
    public GameData()
    {
        this.highScore = 0;
    }

}
