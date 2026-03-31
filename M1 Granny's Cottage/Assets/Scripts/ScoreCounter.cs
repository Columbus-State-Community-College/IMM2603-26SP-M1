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
        this.highScore = (this.totalScore > this.highScore) ? this.highScore : this.totalScore; 
        this.pointBank += this.totalScore;
        UpdateHighScoreText();
    }


    public void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + this.highScore;   
    }

    public void LoadData(GameData data)
    {
        this.highScore = data.highScore;
        UpdateHighScoreText();
        this.pointBank = data.pointBank;
    }

    public void SaveData(ref GameData data)
    {
        data.highScore = this.highScore;
        data.pointBank = this.pointBank;
    }
}
