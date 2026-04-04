using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeZone : MonoBehaviour, IDataPersistence
{
    public GameObject OKButton;
    public GameObject NOButton;
    public GameObject WelcomeBackground;
    public GameObject HammerGlamourChoices;
    public Button HammerGlamourButton;
    public Button HammerBaseDamageButton;
    public Button HammerBaseAttackSpeedButton;
    public Button HammerBaseKnockbackButton;
    public TMP_Text ChangeableText;
    public TMP_Text OKButtonText;
    public TMP_Text UpgradeAppliedText;
    public TMP_Text CoinCountText;
    private int pointBank;
    private bool isFirstTime = true;
    private bool isHammerBaseDamage;
    private bool isHammerBaseAttackSpeed;
    private bool isHammerBaseKnockback;

    void Start ()
    {
        CoinCountText.text = "\"Trespassing\" Fines Collected: $" + pointBank.ToString();
    }


    public void OKButtonPress()
    {
        if (isFirstTime == true)
        {
            OKButton.gameObject.SetActive(false);
            OKButtonText.text = "YES!";
            WelcomeBackground.gameObject.SetActive(false);
            isFirstTime = false;
        }

        if (isHammerBaseDamage == true)
        {
            UpgradeAppliedText.text = "Base Damage Glamour Applied!";
            isHammerBaseDamage = false;
            HammerGlamourButton.interactable = true;
            HammerBaseDamageButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isHammerBaseAttackSpeed == true)
        {
            UpgradeAppliedText.text = "Base Attack Speed Glamour Applied!";
            isHammerBaseAttackSpeed = false;
            HammerGlamourButton.interactable = true;
            HammerBaseAttackSpeedButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isHammerBaseKnockback == true)
        {
            UpgradeAppliedText.text = "Base Knockback Glamour Applied!";
            isHammerBaseKnockback = false;
            HammerGlamourButton.interactable = true;
            HammerBaseKnockbackButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }
    }

    public void NOButtonPress()
    {
        if (isHammerBaseDamage == true)
        {
            isHammerBaseDamage = false;
            HammerGlamourChoices.gameObject.SetActive(true);
        }

        if (isHammerBaseAttackSpeed == true)
        {
            isHammerBaseAttackSpeed = false;
            HammerGlamourChoices.gameObject.SetActive(true);
        }

        if (isHammerBaseKnockback == true)
        {
            isHammerBaseKnockback = false;
            HammerGlamourChoices.gameObject.SetActive(true);
        }

        ChangeableText.text = "What Kind of Glamour Do You Want?";
        OKButton.gameObject.SetActive(false);
        NOButton.gameObject.SetActive(false);
    }

    public void HammerGlamour()
    {
        ChangeableText.text = "What Kind of Glamour Do You Want?";
        HammerGlamourChoices.gameObject.SetActive(true);
        HammerGlamourButton.interactable = false;
    }

    public void HammerBaseDamage()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            isHammerBaseDamage = true;
            SubMenuConfirm();
        }

        else
        {
            NotEnoughCoins();
        }
    }

    public void HammerBaseAttackSpeed()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            isHammerBaseAttackSpeed = true;
            SubMenuConfirm();
        }

        else
        {
            NotEnoughCoins();
        }
    }

    public void HammerBaseKnockback()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            isHammerBaseKnockback = true;
            SubMenuConfirm();
        }

        else 
        {
            NotEnoughCoins();
        }
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

    public void NotEnoughCoins()
    {
        UpgradeAppliedText.text = "Not Enough $$$ to get the Glamour!";
    }

    public void CoinCount()
    {
        GlamourSaving();
        CoinCountText.text = "\"Trespassing\" Fines Collected: $" + pointBank.ToString();
        
    }

    public void LoadData(GameData data)
    {
        this.pointBank = data.pointBank;
    }

    public void SaveData(ref GameData data)
    {
        data.pointBank = this.pointBank;
    }

    // This is a helper function to save the gamestate of the pointBank and other base variables.
    private void GlamourSaving()
    {
        DataPersistenceManager.instance.SaveGame();
    }
}
