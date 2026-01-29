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
        "Speed Boost", "Double Jump", "Dash", "Extra Health", "Area Smash", "Quick Swing"
    };

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

        popupPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ApplyPowerup(string powerup)
    {
        // Will add to these if game is chosen
        switch (powerup)
        {
            case "Speed Boost":
                break;
            case "Double Jump":
                break;
            case "Dash":
                break;
            case "Extra Health":
                break;
            case "Area Smash":
                break;
            case "Quick Swing":
                break;
        }
    }
}
