using TMPro;
using UnityEngine;

public class StatsMenuUI : MonoBehaviour
{
    public TMP_Text killsText;
    public TMP_Text waveText;
    public TMP_Text runsText;

    void Start()
    {
        GameData data = DataPersistenceManager.instance.GameData;

        killsText.text = "Total Enemies Killed: " + data.totalEnemiesKilled;
        waveText.text = "Highest Wave: " + data.highestWave;
        runsText.text = "Total Runs: " + data.totalRuns;
    }
}
