using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public ScoreCounter scoreScript;

    [Header("Health UI and Audio Elements")]

    public PlayerHitbox hitbox;
    public GameObject GameOverScreen;
    public GrannysSounds grannySoundScript;

    void Start()
    {
        currentHealth = maxHealth;

        if (hitbox != null)
        {
            hitbox.SetMaxHealth(maxHealth);
        }
        else
        {
            //Debug.LogWarning("[PLAYER] No PlayerHitbox assigned");
        }

        // Log starting health
        //Debug.Log("[PLAYER] Starting Health: " + currentHealth);
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("[PLAYER] Taking Damage: " + amount);

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hitbox != null)
        {
            hitbox.SetHealth(currentHealth);
        }

        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHPLost();
        }

        //Debug.Log("[PLAYER] Current Health After Damage: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Heal function for health pickups
    public void Heal(float amount)
    {
        //Debug.Log("[PLAYER] Attempting Heal: " + amount);

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hitbox != null)
        {
            hitbox.SetHealth(currentHealth);
        }
        //Debug.Log("[PLAYER] Current Health After Heal: " + currentHealth);

        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHeal();
        }
    }

    // Increase max health powerup support
    public void IncreaseMaxHealth(float amount)  
    {
        maxHealth += amount; // increase cap
        currentHealth += amount; // heal by same amount

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // safety

        if (hitbox != null) // update UI
        {
            hitbox.SetMaxHealth(maxHealth);
            hitbox.SetHealth(currentHealth);
        }

        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHeal();
        }
        //Debug.Log("[POWERUP] Extra Health applied. Max Health = " + maxHealth + " | Current Health = " + currentHealth); //power up log
    }

    void Die()
    {
        //Debug.Log("[PLAYER] Player Died");

        GameOverScreen.gameObject.SetActive(true);
        if (grannySoundScript != null)
        {
            grannySoundScript.PlayScream();
        }
        Time.timeScale = 0;
        GameObject playerObject = GameObject.Find("PlayerParent");
        if (playerObject)
        {
            ScoreCounter scoreScript = playerObject.GetComponent<ScoreCounter>();
            if (scoreScript != null)
            {
                scoreScript.GameOverProcess();
            }
        }
    }
}
