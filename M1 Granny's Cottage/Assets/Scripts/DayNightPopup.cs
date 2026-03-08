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

    void ShowPopup(DayNightCycle.TimeOfDay time)
    {
        Time.timeScale = 0f;

        popupPanel.SetActive(true);
        popupText.text = time == DayNightCycle.TimeOfDay.Day ? "DAYTIME" : "NIGHTFALL";

        List<string> choices = allPowerups.OrderBy(x => Random.value).Take(powerupButtons.Length).ToList();

        for (int i = 0; i < powerupButtons.Length; i++)
        {
            int index = i;
            string powerup = choices[i];

            buttonTexts[i].text = powerup;
            powerupButtons[i].gameObject.SetActive(true);

            powerupButtons[i].onClick.RemoveAllListeners();
            powerupButtons[i].onClick.AddListener(() => SelectPowerup(powerup));
        }
    }

    void SelectPowerup(string powerup)
    {
        Debug.Log("Player chose: " + powerup);

        ApplyPowerup(powerup);
        if (etcSoundScript != null)
        {
            etcSoundScript.PlayUpgrade();
        }

        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ApplyPowerup(string powerup)
    {
        // Will add to these if game is chosen
        if (playerController == null) return;  

        switch (powerup)
        {
            case "Speed Boost":
                playerController.ApplySpeedBoost();  
                break;

            case "Extra Health":
                playerController.ApplyExtraHealth();  
                break;

            case "Quick Swing":
                playerController.ApplyQuickSwing();  
                break;

            case "Empowered Slam":
                playerController.ApplyEmpoweredSlam();
                break;

            case "Hammer Damage":
                playerController.ApplyHammerDamage();
                break;

            case "Hammer Knockback":
                playerController.ApplyHammerKnockback();
                break;

            case "Jump Duration":
                playerController.ApplyJumpDurationUpgrade();
                break;

            case "Jump Stun Duration":
                playerController.ApplyJumpStunUpgrade();
                break;

            case "Jump AOE Size":
                playerController.ApplyJumpAOEUpgrade();
                break;
        }
        
    }
}