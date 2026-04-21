using UnityEngine;

public class ScoreMultiplierZombie : MonoBehaviour
{
    [Header("Multiplier Spawn Location")]
    [SerializeField] private Transform multiplierSpawnPoint;

    private int dayCount = 1;

    private void OnEnable()
    {
        DayNightCycle.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        DayNightCycle.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(DayNightCycle.TimeOfDay time)
    {
        if (time == DayNightCycle.TimeOfDay.Day)
        {
            dayCount++;
            Debug.Log("[MULTIPLIER ZOMBIE] Day count: " + dayCount);
        }
    }

    public void OnScoreMultiplierPickedUp()
    {
        if (Spawner.Instance == null)
        {
            Debug.LogWarning("[MULTIPLIER ZOMBIE] Spawner not found.");
            return;
        }

        if (multiplierSpawnPoint == null)
        {
            Debug.LogWarning("[MULTIPLIER ZOMBIE] Multiplier spawn point not assigned.");
            return;
        }

        Debug.Log("[MULTIPLIER ZOMBIE] Spawning " + dayCount + " zombies at multiplier location");

        for (int i = 0; i < dayCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
            Spawner.Instance.SpawnSingleEnemyAtPosition(multiplierSpawnPoint.position + offset);
        }
    }
}