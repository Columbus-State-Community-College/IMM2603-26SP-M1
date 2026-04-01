using UnityEngine;
using System.IO;
using TMPro;

public class ScoreCounter : MonoBehaviour, IDataPersistence
{
    public int totalScore = 0;
    public int highScore;
    public int pointBank;
    public TMP_Text scoreCounter;
    public TMP_Text overallScoreText;
    public TMP_Text highScoreText;

    void Start()
    {
        scoreCounter.text = "Score: " + totalScore.ToString();
    }

    public void addPoints(int points)
    {
        totalScore += points;
        scoreCounter.text = "Score: " + totalScore.ToString();
        overallScoreText.text = "Overall Score: " + totalScore.ToString();
    }

    public void GameOverProcess()
    {
        if (totalScore > highScore)
        {
            highScore = totalScore;
        }

        UpdateHighScoreText();

        DataPersistenceManager.instance.SaveGame();
    }


    public void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    public void LoadData(GameData data)
    {
        highScore = data.highScore;
        UpdateHighScoreText();
    }

    public void SaveData(ref GameData data)
    {
        if (totalScore > highScore)
        {
            highScore = totalScore;
        }

        data.highScore = highScore;
    }
}
