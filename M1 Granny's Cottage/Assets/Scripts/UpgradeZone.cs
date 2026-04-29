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
    public Button BaseMaxJumpCooldownDurButton;
    public Button BaseJumpStunDurButton;
    public Button BaseJumpMaxAOEButton;
    public GameObject StatusGlamourChoices;
    public Button StatusGlamourButton;
    public Button BaseHealthButton;
    public Button DayDurationButton;
    public Button NightDurationButton;
    public Button MovementSpeedButton;
    public Button ScoreMultiplierButton;
    public GameObject HammerGlamourDescriptions;
    public GameObject JumpGlamourDescriptions;
    public GameObject StatusGlamourDescriptions;
    public TMP_Text ChangeableText;
    public TMP_Text OKButtonText;
    public TMP_Text UpgradeAppliedText;
    public TMP_Text CoinCountText;
    public AudioClip GlamourGetSound;
    public AudioSource soundEffectSource;
    private int pointBank;
    private bool isFirstTime = true;
    private bool isHammerBaseDamage;
    private bool isHammerBaseAttackSpeed;
    private bool isHammerBaseKnockback;
    private bool isJumpBaseMaxCooldownDur;
    private bool isJumpBaseStunDur;
    private bool isJumpBaseMaxAOE;
    private bool isStatusBaseHealth;
    private bool isStatusBaseDayDur;
    private bool isStatusBaseNightDur;
    private bool isStatusBaseMoveSpeed;
    private bool isStatusBaseScoreMultiplier;

    void Start ()
    {
        CoinCountText.text = "Total Trespassing Fines Collected: $" + pointBank.ToString();

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
            float increase = 20f;
            UpgradeAppliedText.text = "Base Damage Glamour Applied!";
            isHammerBaseDamage = false;
            HammerGlamourButton.interactable = true;
            HammerBaseDamageButton.interactable = false;
            pointBank -= 10000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.hammerSwingDamage += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isHammerBaseAttackSpeed == true)
        {
            float decrease = 0.2f;
            UpgradeAppliedText.text = "Base Attack Speed Glamour Applied!";
            isHammerBaseAttackSpeed = false;
            HammerGlamourButton.interactable = true;
            HammerBaseAttackSpeedButton.interactable = false;
            pointBank -= 5000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.hammerSwingAttackCooldown -= decrease;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isHammerBaseKnockback == true)
        {
            float increase = 2f;
            UpgradeAppliedText.text = "Base Knockback Glamour Applied!";
            isHammerBaseKnockback = false;
            HammerGlamourButton.interactable = true;
            HammerBaseKnockbackButton.interactable = false;
            pointBank -= 10000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.hammerSwingKnockbackMultiplier += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isJumpBaseMaxCooldownDur == true)
        {
            float decrease = 2f;
            UpgradeAppliedText.text = "Max Jump Duration Glamour Applied!";
            isJumpBaseMaxCooldownDur = false;
            JumpGlamourButton.interactable = true;
            BaseMaxJumpCooldownDurButton.interactable = false;
            pointBank -= 12000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.jumpSlamCooldown -= decrease;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isJumpBaseStunDur == true)
        {
            float increase = 2f;
            UpgradeAppliedText.text = "Jump Stun Duration Glamour Applied!";
            isJumpBaseStunDur = false;
            JumpGlamourButton.interactable = true;
            BaseJumpStunDurButton.interactable = false;
            pointBank -= 15000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.jumpSlamStunDuration += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isJumpBaseMaxAOE == true)
        {
            float increase = 1f;
            UpgradeAppliedText.text = "Jump AOE Glamour Applied!";
            isJumpBaseMaxAOE = false;
            JumpGlamourButton.interactable = true;
            BaseJumpMaxAOEButton.interactable = false;
            pointBank -= 20000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.jumpSlamMaxRadius += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isStatusBaseScoreMultiplier == true)
        {
            UpgradeAppliedText.text = "Fines Multiplier Glamour Applied!";
            isStatusBaseScoreMultiplier = false;
            StatusGlamourButton.interactable = true;
            ScoreMultiplierButton.interactable = false;
            pointBank -= 100000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.scoreMultiplier += 0.25f;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isStatusBaseHealth == true)
        {
            float increase = 25f;
            UpgradeAppliedText.text = "Base Health Glamour Applied!";
            isStatusBaseHealth = false;
            StatusGlamourButton.interactable = true;
            BaseHealthButton.interactable = false;
            pointBank -= 10000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.maxHealth += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isStatusBaseDayDur == true)
        {
            float increase = 20f;
            UpgradeAppliedText.text = "Day Duration Glamour Applied!";
            isStatusBaseDayDur = false;
            StatusGlamourButton.interactable = true;
            DayDurationButton.interactable = false;
            pointBank -= 15000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.dayDuration += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isStatusBaseNightDur == true)
        {
            float increase = 20f;
            UpgradeAppliedText.text = "Night Duration Glamour Applied!";
            isStatusBaseNightDur = false;
            StatusGlamourButton.interactable = true;
            NightDurationButton.interactable = false;
            pointBank -= 15000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.nightDuration += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
        }

        if (isStatusBaseMoveSpeed == true)
        {
            float increase = 2f;
            UpgradeAppliedText.text = "Movement Speed Glamour Applied!";
            isStatusBaseMoveSpeed = false;
            StatusGlamourButton.interactable = true;
            MovementSpeedButton.interactable = false;
            pointBank -= 50000;
            GameData data = DataPersistenceManager.instance.GameData;
            data.runSpeed += increase;
            SubMenuSuccessReturn();
            CoinCount();
            GlamourGet();
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

    void OpenGlamourMenu(GameObject menu, Button button, GameObject descriptions)
    {
        HammerGlamourChoices.SetActive(false);
        JumpGlamourChoices.SetActive(false);
        StatusGlamourChoices.SetActive(false);

        HammerGlamourButton.interactable = true;
        JumpGlamourButton.interactable = true;
        StatusGlamourButton.interactable = true;

        HammerGlamourDescriptions.SetActive(false);
        JumpGlamourDescriptions.SetActive(false);
        StatusGlamourDescriptions.SetActive(false);

        menu.SetActive(true);

        button.interactable = false;

        descriptions.SetActive(true);

        ChangeableText.text = "What Kind of Glamour Do You Want?";
    }

    public void HammerGlamour()
    {
        OpenGlamourMenu(HammerGlamourChoices, HammerGlamourButton, HammerGlamourDescriptions);
    }

    public void HammerBaseDamage()
    {
        int requiredcoins = 10000;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            HammerGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 5000;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            HammerGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 10000;

        if (pointBank >= requiredcoins){
            HammerGlamourChoices.gameObject.SetActive(false);
            HammerGlamourDescriptions.SetActive(false);
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
        OpenGlamourMenu(JumpGlamourChoices, JumpGlamourButton, JumpGlamourDescriptions);
    }

    public void JumpBaseMaxDur()
    {
        int requiredcoins = 12000;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            JumpGlamourDescriptions.SetActive(false);
            isJumpBaseMaxCooldownDur = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void JumpBaseStunDur()
    {
        int requiredcoins = 15000;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            JumpGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 20000;

        if (pointBank >= requiredcoins)
        {
            JumpGlamourChoices.gameObject.SetActive(false);
            JumpGlamourDescriptions.SetActive(false);
            isJumpBaseMaxAOE = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void StatusGlamour()
    {
        OpenGlamourMenu(StatusGlamourChoices, StatusGlamourButton, StatusGlamourDescriptions);
    }

    public void StatusHealth()
    {
        int requiredcoins = 10000;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            StatusGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 15000;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            StatusGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 15000;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            StatusGlamourDescriptions.SetActive(false);
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
        int requiredcoins = 50000;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            StatusGlamourDescriptions.SetActive(false);
            isStatusBaseMoveSpeed = true;
            SubMenuConfirm();
        }
        else
        {
            NotEnoughCoins();
        }
    }

    public void StatusScoreMultiplier()
    {
        int requiredcoins = 100000;

        if (pointBank >= requiredcoins)
        {
            StatusGlamourChoices.gameObject.SetActive(false);
            StatusGlamourDescriptions.SetActive(false);
            isStatusBaseScoreMultiplier = true;
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
        ChangeableText.text = "Welcome To the Glamour Shop!";
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

    public void GlamourGet()
    {
        soundEffectSource.PlayOneShot(GlamourGetSound);
    }

    public void CoinCount()
    {
        GlamourSaving();
        CoinCountText.text = "Trespassing Fines Collected: $" + pointBank.ToString();
        
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
