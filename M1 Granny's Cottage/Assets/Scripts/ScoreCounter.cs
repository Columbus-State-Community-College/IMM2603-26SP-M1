using UnityEngine;
using System.IO;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public int totalScore = 0;
    public TMP_Text scoreCounter;
    public TMP_Text overallScoreText;
    private string scoreFile = "HighScore.txt";

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

    public void startFileWrite()
    {
        string path = Path.Combine(Application.persistentDataPath, scoreFile);
        WriteToFile(path, "High Score: " + totalScore.ToString());
    }

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
    }
}
