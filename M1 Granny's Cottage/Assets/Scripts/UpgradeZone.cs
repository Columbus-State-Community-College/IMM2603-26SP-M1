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
    public GameObject JumpGlamourChoices;
    public Button JumpGlamourButton;
    public Button BaseMaxJumpDurButton;
    public Button BaseJumpStunDurButton;
    public Button BaseJumpMaxAOEDurButton;
    public GameObject StatusGlamourChoices;
    public Button StatusGlamourButton;
    public Button ScoreMultiplierButton;
    public Button BaseHealthButton;
    public Button DayDurationButton;
    public Button NightDurationButton;
    public Button MovementSpeedButton;
    public TMP_Text ChangeableText;
    public TMP_Text OKButtonText;
    public TMP_Text UpgradeAppliedText;
    public TMP_Text CoinCountText;
    private int pointBank;
    private bool isFirstTime = true;
    private bool isHammerBaseDamage;
    private bool isHammerBaseAttackSpeed;
    private bool isHammerBaseKnockback;
    private bool isJumpBaseMaxDur;
    private bool isJumpBaseStunDur;
    private bool isJumpBaseMaxAOEDur;
    private bool isStatusBaseScoreMultiplier;
    private bool isStatusBaseHealth;
    private bool isStatusBaseDayDur;
    private bool isStatusBaseNightDur;
    private bool isStatusBaseMoveSpeed;

    void Start ()
    {
        CoinCountText.text = "\"Trespassing\" Fines Collected: $" + pointBank.ToString();

        if (isFirstTime == true )
        {
            HammerGlamourButton.gameObject.SetActive(false);
            JumpGlamourButton.gameObject.SetActive(false);
            StatusGlamourButton.gameObject.SetActive(false);
        }

        
    }


    public void OKButtonPress()
    {
        if (isFirstTime == true)
        {
            OKButton.gameObject.SetActive(false);
            OKButtonText.text = "YES!";
            WelcomeBackground.gameObject.SetActive(false);
            isFirstTime = false;

            HammerGlamourButton.gameObject.SetActive(true);
            JumpGlamourButton.gameObject.SetActive(true);
            StatusGlamourButton.gameObject.SetActive(true);
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

        if (isJumpBaseMaxDur == true)
        {
            UpgradeAppliedText.text = "Max Jump Duration Glamour Applied!";
            isJumpBaseMaxDur = false;
            JumpGlamourButton.interactable = true;
            BaseMaxJumpDurButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isJumpBaseStunDur == true)
        {
            UpgradeAppliedText.text = "Jump Stun Duration Glamour Applied!";
            isJumpBaseStunDur = false;
            JumpGlamourButton.interactable = true;
            BaseJumpStunDurButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isJumpBaseMaxAOEDur == true)
        {
            UpgradeAppliedText.text = "Jump AOE Glamour Applied!";
            isJumpBaseMaxAOEDur = false;
            JumpGlamourButton.interactable = true;
            BaseJumpMaxAOEDurButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isStatusBaseScoreMultiplier == true)
        {
            UpgradeAppliedText.text = "Score Multiplier Glamour Applied!";
            isStatusBaseScoreMultiplier = false;
            StatusGlamourButton.interactable = true;
            ScoreMultiplierButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isStatusBaseHealth == true)
        {
            UpgradeAppliedText.text = "Base Health Glamour Applied!";
            isStatusBaseHealth = false;
            StatusGlamourButton.interactable = true;
            BaseHealthButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isStatusBaseDayDur == true)
        {
            UpgradeAppliedText.text = "Day Duration Glamour Applied!";
            isStatusBaseDayDur = false;
            StatusGlamourButton.interactable = true;
            DayDurationButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isStatusBaseNightDur == true)
        {
            UpgradeAppliedText.text = "Night Duration Glamour Applied!";
            isStatusBaseNightDur = false;
            StatusGlamourButton.interactable = true;
            NightDurationButton.interactable = false;
            pointBank -= 100;
            SubMenuSuccessReturn();
            CoinCount();
        }

        if (isStatusBaseMoveSpeed == true)
        {
            UpgradeAppliedText.text = "Movement Speed Glamour Applied!";
            isStatusBaseMoveSpeed = false;
            StatusGlamourButton.interactable = true;
            MovementSpeedButton.interactable = false;
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

    void OpenGlamourMenu(GameObject menu, Button button)
    {
        HammerGlamourChoices.SetActive(false);
        JumpGlamourChoices.SetActive(false);
        StatusGlamourChoices.SetActive(false);

        HammerGlamourButton.interactable = true;
        JumpGlamourButton.interactable = true;
        StatusGlamourButton.interactable = true;

        menu.SetActive(true);

        button.interactable = false;

        ChangeableText.text = "What Kind of Glamour Do You Want?";
    }

    public void HammerGlamour()
    {
        OpenGlamourMenu(HammerGlamourChoices, HammerGlamourButton);
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

    public void JumpGlamour()
    {
        OpenGlamourMenu(JumpGlamourChoices, JumpGlamourButton);
    }

    public void JumpBaseMaxDur()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            isJumpBaseMaxDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void JumpBaseStunDur()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            isJumpBaseStunDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void JumpBaseMaxAOEDur()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            isJumpBaseMaxAOEDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void StatusGlamour()
    {
        OpenGlamourMenu(StatusGlamourChoices, StatusGlamourButton);
    }

    public void StatusScoreMultiplier()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            isStatusBaseScoreMultiplier = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void StatusHealth()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            isStatusBaseHealth = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }
    public void StatusDayDuration()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            isStatusBaseDayDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }
    public void StatusNightDuration()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            isStatusBaseNightDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }
    public void StatusMovementSpeed()
    {
        int requiredcoins = 100;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            isStatusBaseMoveSpeed = true;
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
