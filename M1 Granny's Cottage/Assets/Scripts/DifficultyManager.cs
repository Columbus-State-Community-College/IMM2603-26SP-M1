using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Enemy Multipliers")]
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
    public float enemySpeedMultiplier = 1f;

    [Header("Player Multipliers")]
    public float playerDamageMultiplier = 1f;
    public float playerCooldownMultiplier = 1f;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public Difficulty currentDifficulty = Difficulty.Normal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;

        switch (difficulty)
        {
            case Difficulty.Easy:
                enemyHealthMultiplier = 0.75f;
                enemyDamageMultiplier = 0.75f;
                enemySpeedMultiplier = 0.9f;
                break;
            case Difficulty.Normal:
                enemyHealthMultiplier = 1f;
                enemyDamageMultiplier = 1f;
                enemySpeedMultiplier = 1f;
                break;
            case Difficulty.Hard:
                enemyHealthMultiplier = 1.5f;
                enemyDamageMultiplier = 1.5f;
                enemySpeedMultiplier = 1.2f;
                break;
        }
    }
}
