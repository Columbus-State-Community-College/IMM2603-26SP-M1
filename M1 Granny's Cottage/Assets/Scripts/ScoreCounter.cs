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
    //private string scoreFile = "HighScore.txt";

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
    /* public void startFileWrite()
    {
        string filePath = Path.Combine(Application.persistentDataPath, scoreFile);
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            if (int.TryParse(fileContent, out int result))
            {
                Debug.Log("Parsed Integer: " + result);

                if (totalScore >= result)
                {
                    string path = Path.Combine(Application.persistentDataPath, scoreFile);
                    WriteToFile(path, totalScore.ToString());
                }

                else 
                {
                    ReadFile();
                }
            }
        }

        else
        {
            string path = Path.Combine(Application.persistentDataPath, scoreFile);
            WriteToFile(path, totalScore.ToString());
        }
    } */

    public void WriteToFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content + System.Environment.NewLine);
            Debug.Log("Successfully wrote to file at: " + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error writing to file: " + ex.Message);
        }

        UpdateHighScoreText();
    }

    public void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + this.highScore;   
    }

    public void LoadData(GameData data)
    {
        this.highScore = data.highScore;
        this.pointBank = data.pointBank;
    }

    public void SaveData(ref GameData data)
    {
        data.highScore = this.highScore;
        data.pointBank = this.pointBank;
    }
}
