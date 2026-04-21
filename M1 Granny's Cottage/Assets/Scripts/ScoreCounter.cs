using UnityEngine;
using System.IO;
using System.Collections;
using TMPro;

public class ScoreCounter : MonoBehaviour, IDataPersistence
{
    public int totalScore = 0;
    public int highScore;
    public int pointBank;
    public TMP_Text scoreCounter;
    public TMP_Text overallScoreText;
    public TMP_Text highScoreText;
    public TMP_Text pointsGotText;
    private int pointsGotInstances;
    private int timerTime;
    private bool timeGoing = false;
    private IEnumerator timerCoroutine;

    // NEW
    private float scoreMultiplier = 1f;

    void Start()
    {
        scoreCounter.text = "Trespassing Fines: $" + totalScore.ToString();
    }

    // NEW
    public void SetScoreMultiplier(float multiplierValue)
    {
        scoreMultiplier = multiplierValue;
        Debug.Log("[SCORE COUNTER] Multiplier set to: " + scoreMultiplier);
    }

    public void addPoints(int points)
    {
        PointsGotInstanceChecker();

        // NEW: apply multiplier
        int finalPoints = Mathf.RoundToInt(points * scoreMultiplier);

        totalScore += finalPoints;

        scoreCounter.text = "Trespassing Fines: $" + totalScore.ToString();
        overallScoreText.text = "Today's Payout: $" + totalScore.ToString();
        pointsGotText.text += "$" + finalPoints.ToString() + "\n";
    }

    public void PointsGotInstanceChecker()
    {
        pointsGotInstances += 1;
        PointsGotTextTimerStart();
        Debug.Log("Instances: " + pointsGotInstances.ToString());

        if (pointsGotInstances == 6)
        {
            pointsGotText.text = " ";
            pointsGotInstances = 1;
        }
    }

    public void PointsGotTextTimerStart()
    {
        if(timeGoing == false)
        {
            timeGoing = true;
            timerCoroutine = InstanceTimer(1.0f);
            StartCoroutine(timerCoroutine);
        }
    }

    IEnumerator InstanceTimer(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("Timer Time: " + timerTime);
            timerTime += 1;
            PointsGotTextTimerStoppingPoint();
        }
    }

    public void PointsGotTextTimerStoppingPoint()
    {
        if (timerTime == 16)
        {
            print("It's time for the timer to stop!");
            pointsGotText.text = " ";
            pointsGotInstances = 1;
            StopCoroutine(timerCoroutine);
            timeGoing = false;
            timerTime = 0;
        }
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
            highScoreText.text = "Biggest Payday: $" + highScore;
        }
    }

    public void LoadData(GameData data)
    {
        highScore = data.highScore;
        UpdateHighScoreText();
    }

    public void SaveData(ref GameData data)
    {
        // add run score to pointBank
        data.pointBank += totalScore;

        if (totalScore > highScore)
        {
            highScore = totalScore;
        }

        data.highScore = highScore;
    }
}