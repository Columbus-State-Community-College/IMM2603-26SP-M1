using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    public int totalScore = 0;
    public int score = 100;
    public TMP_Text scoreCounter;

    void Start()
    {
        scoreCounter.text = "Score: " + totalScore.ToString();
    }

    void Update()
    {
        totalScore += score;
        scoreCounter.text = "Score: " + totalScore.ToString();
    }
}
