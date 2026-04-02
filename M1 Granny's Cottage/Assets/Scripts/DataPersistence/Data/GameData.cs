using TMPro;
using UnityEngine;

[System.Serializable]
public class GameData
{
    /*
        <!> When making game data, use the following naming and writing pattern. Load 
        saved values as needed in local scripts. Also indicate if the data 
        focus is a Glamour candidate. Gift upgrades should be handled locally during 
        active runs.

        // [Comment explaining data focus] : [Glamour candidate]
        private const [type] d_[variableName]  // default value on new game
        public [type] [variableName]  // saved base value

        <!> Once this is done, set the saved value to the default value in the constructor
    */

    // High Score / Sunk Cost Data
    private const int d_highScore = 0;
    public int highScore;

    // Glamour Bank Data
    private const int d_pointBank = 0;
    public int pointBank;

    // Player Run Speed : Glamour
    private const float d_runSpeed = 10f;
    public float runSpeed;

    // Player Max Jump Time : Glamour
    private const float d_maxJumpTime = 2f;
    public float maxJumpTime;
    
    // Player Jump Slam Cooldown Duration : Glamour
    private const float d_jumpSlamCooldown = 7f;
    public float jumpSlamCooldown;

    // Player Jump Slam Stun Duration : Glamour
    private const float d_jumpSlamStunDuration = 2f;
    public float jumpSlamStunDuration;

    // Player Jump Slam Max Radius : Glamour
    private const float d_jumpSlamMaxRadius = 5f;
    public float jumpSlamMaxRadius;

    // Player Hammer Swing Damage : Glamour
    private const float d_hammerSwingDamage = 10f;
    public float hammerSwingDamage;

    // Player Hammer Swing Attack Duration : Glamour
    private const float d_hammerSwingAttackDuration = 0.5f;
    public float hammerSwingAttackDuration;

    // Player Hammer Swing Knockback Multiplier : Glamour
    private const float d_hammerSwingKnockbackMultiplier = 1f;
    public float hammerSwingKnockbackMultiplier;

    // Player Hammer Swing Attack Cooldown : Glamour
    private const float d_hammerSwingAttackCooldown = 0.8f;
    public float hammerSwingAttackCooldown;

    // Player Max Health : Glamour
    private const float d_maxHealth = 100f;
    public float maxHealth;


    // Game music volume
    private const float d_musicVolume = 0.5f;
    public float musicVolume;

    // Granny sound volume
    private const float d_grannyVolume = 0.5f;
    public float grannyVolume;

    // Enemy sound volume
    private const float d_enemyVolume = 0.5f;
    public float enemyVolume;

    // ETC sound volume
    private const float d_etcVolume = 0.5f;
    public float etcVolume;
    

    /* 
        This constructor initializes the New Game game data state. The saved 
        values are initialized with the default values.
    */
    public GameData()
    {
        this.highScore = d_highScore;
        this.pointBank = d_pointBank;
        this.runSpeed = d_runSpeed;
        this.maxJumpTime = d_maxJumpTime;
        this.jumpSlamCooldown = d_jumpSlamCooldown;
        this.jumpSlamStunDuration = d_jumpSlamStunDuration;
        this.jumpSlamMaxRadius = d_jumpSlamMaxRadius;
        this.hammerSwingDamage = d_hammerSwingDamage;
        this.hammerSwingAttackDuration = d_hammerSwingAttackDuration;
        this.hammerSwingKnockbackMultiplier = d_hammerSwingKnockbackMultiplier;
        this.hammerSwingAttackCooldown = d_hammerSwingAttackCooldown;
        this.maxHealth = d_maxHealth;
        this.musicVolume = d_musicVolume;
        this.grannyVolume = d_grannyVolume;
        this.enemyVolume = d_enemyVolume;
        this.etcVolume = d_etcVolume;
    }

}
