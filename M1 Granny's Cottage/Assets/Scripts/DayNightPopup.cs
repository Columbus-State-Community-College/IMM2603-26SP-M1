using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayNightPopup : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    [Header("Powerup Buttons")]
    public Button[] powerupButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Settings")]
    public List<string> allPowerups = new List<string>
    {
        "Hammer Damage",
        "Speed Boost",
        "Extra Health",
        "Quick Swing",
        "Empowered Slam",
        "Hammer Knockback",
        "Smash Cooldown",
        "Jump Duration",
        "Jump Stun Duration",
        "Jump AOE Size",
        
    };

    [Header("Select Sound Effect")]
    public EtcSounds etcSoundScript;

     
    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;  

    private void OnEnable()
    {
        DayNightCycle.OnTimeChanged += ShowPopup;

    }

    private void OnDisable()
    {
        DayNightCycle.OnTimeChanged -= ShowPopup;
    }

    // NEW 
    private Dictionary<string, int> powerupCounts = new Dictionary<string, int>();

    void ShowPopup(DayNightCycle.TimeOfDay time)
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        popupPanel.SetActive(true);
        popupText.text = time == DayNightCycle.TimeOfDay.Day ? "DAYTIME" : "NIGHTFALL";

        List<string> availablePowerups = allPowerups
            .Where(CanPowerupAppear)
            .OrderBy(x => Random.value)
            .ToList();

        List<string> choices = availablePowerups
            .Take(powerupButtons.Length)
            .ToList();

        for (int i = 0; i < powerupButtons.Length; i++)
        {
            string powerup = choices[i];

            int count = GetPowerupCount(powerup); // NEW

            if (count > 0)
            {
                buttonTexts[i].text = powerup + " (" + count + ")"; // NEW
            }

            else
            {
                buttonTexts[i].text = powerup;
            }

            powerupButtons[i].gameObject.SetActive(true);

            powerupButtons[i].onClick.RemoveAllListeners();
            powerupButtons[i].onClick.AddListener(() => SelectPowerup(powerup));
        }
    }

    // NEW
    private bool CanPowerupAppear(string powerup)
    {
        int count = GetPowerupCount(powerup);

        switch (powerup)
        {
            case "Empowered Slam":
            case "Hammer Knockback":
            case "Quick Swing":
            case "Smash Cooldown":
                return count == 0; // one-time only

            default:
                return true; // can repeat (uses diminishing returns)
        }
}

    void SelectPowerup(string powerup)
    {
        RecordPowerupChoice(powerup);

        int pickCount = GetPowerupCount(powerup);

        Debug.Log("[POWERUP] Player chose: " + powerup + " | Times chosen: " + pickCount);

        ApplyPowerup(powerup, pickCount);

        if (etcSoundScript != null)
        {
            etcSoundScript.PlayUpgrade();
        }

        popupPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ApplyPowerup(string powerup, int pickCount)
    {
        // Will add to these if game is chosen
        if (playerController == null) return;  

        switch (powerup)
        {
            case "Speed Boost":
                playerController.ApplySpeedBoost(pickCount);  
                break;

            case "Extra Health":
                playerController.ApplyExtraHealth(pickCount);  
                break;

            case "Quick Swing":
                playerController.ApplyQuickSwing(pickCount);  
                break;

            case "Empowered Slam":
                playerController.ApplyEmpoweredSlam(pickCount);
                break;

            case "Hammer Damage":
                playerController.ApplyHammerDamage(pickCount);
                break;

            case "Hammer Knockback":
                playerController.ApplyHammerKnockback(pickCount);
                break;

            case "Jump Duration":
                playerController.ApplyJumpDurationUpgrade(pickCount);
                break;

            case "Jump Stun Duration":
                playerController.ApplyJumpStunUpgrade(pickCount);
                break;

            case "Jump AOE Size":
                playerController.ApplyJumpAOEUpgrade(pickCount);
                break;

            case "Smash Cooldown":
                playerController.ApplyGroundSmashUpgrade(pickCount);
                break;
        }
        
    }

    //NEW
    private int GetPowerupCount(string powerup)
    {
        if (powerupCounts.ContainsKey(powerup))
            return powerupCounts[powerup];

        return 0;
    }

    //NEW
    private void RecordPowerupChoice(string powerup)
    {
        if (!powerupCounts.ContainsKey(powerup))
            powerupCounts[powerup] = 0;

        powerupCounts[powerup]++;
    }
}