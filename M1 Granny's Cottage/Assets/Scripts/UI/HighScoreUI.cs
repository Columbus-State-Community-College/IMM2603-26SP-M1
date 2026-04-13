using TMPro;
using UnityEngine;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    public void LoadData(GameData data)
    {
        highScoreText.text = "High Score: " + data.highScore;
    }
}
