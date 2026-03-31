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

    // Player Jump Slam Max Radius : Glamour
    private const float d_jumpSlamMaxRadius = 5f;
    public float jumpSlamMaxRadius;

    // Game volume
    private const float d_volume = 0.5f;
    public float volume;
    

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
        this.volume = d_volume;
        this.jumpSlamMaxRadius = d_jumpSlamMaxRadius;

    }

}
