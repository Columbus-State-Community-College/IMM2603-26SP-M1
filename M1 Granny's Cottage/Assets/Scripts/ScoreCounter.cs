using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public int totalScore = 0;
    public TMP_Text scoreCounter;
    public TMP_Text overallScoreText;

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
}
