using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeZone : MonoBehaviour
{
    public GameObject OKButton;
    public GameObject NOButton;
    public GameObject WelcomeBackground;
    public GameObject HammerGlamourChoices;
    public Button HammerGlamourButton;
    public TMP_Text ChangeableText;
    public TMP_Text OKButtonText;
    private bool isFirstTime = true;
    private bool isHammerBaseDamage;


    public void OKButtonPress()
    {
        if(isFirstTime == true)
        {
            OKButton.gameObject.SetActive(false);
            OKButtonText.text = "YES!";
            WelcomeBackground.gameObject.SetActive(false);
            isFirstTime = false;
        }

        if (isHammerBaseDamage == true)
        {
            Debug.Log("Base Damage Glamour Applied");
            isHammerBaseDamage = false;
            HammerGlamourButton.interactable = true;
            SubMenuSuccessReturn();
        }
    }

    public void NOButtonPress()
    {
        if (isHammerBaseDamage == true)
        {
            OKButton.gameObject.SetActive(false);
            NOButton.gameObject.SetActive(false);
            isHammerBaseDamage = false;
            ChangeableText.text = "What Kind of Glamour Do You Want?";
            HammerGlamourChoices.gameObject.SetActive(true);
        }
    }

    public void HammerGlamour()
    {
        ChangeableText.text = "What Kind of Glamour Do You Want?";
        HammerGlamourChoices.gameObject.SetActive(true);
        HammerGlamourButton.interactable = false;
    }

    public void HammerBaseDamage()
    {
        HammerGlamourChoices.gameObject.SetActive(false);
        isHammerBaseDamage = true;
        SubMenuConfirm();
    }

    public void SubMenuSuccessReturn()
    {
        OKButton.gameObject.SetActive(false);
        NOButton.gameObject.SetActive(false);
        ChangeableText.text = "Welcome To the Upgrade Zone!";
    }

    public void SubMenuConfirm()
    {
        ChangeableText.text = "Apply Glamour?";
        OKButton.gameObject.SetActive(true);
        NOButton.gameObject.SetActive(true);
    }
}
