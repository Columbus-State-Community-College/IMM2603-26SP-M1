using UnityEngine;
using TMPro;

public class HighScoreScript : MonoBehaviour, IDataPersistence
{
    private int highScore = 0;

    private TextMeshProUGUI highScoreText;

    void Awake()
    {
        highScoreText = this.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // tbd
    }

    void LoadData(GameData data)
    {
        this.highScore = data.highScore;
    }
    void SaveData(ref GameData data)
    {
        data.highScore = this.highScore;
    }

    void IDataPersistence.LoadData(GameData data)
    {
        LoadData(data);
    }

    void IDataPersistence.SaveData(ref GameData data)
    {
        SaveData(ref data);
    }
}
