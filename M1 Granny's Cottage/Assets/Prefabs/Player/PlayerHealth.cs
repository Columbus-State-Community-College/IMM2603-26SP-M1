using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

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

        // NEW Log starting health
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

    // NEW Heal function for health pickups
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

    //NEW Increase max health powerup support
    public void IncreaseMaxHealth(float amount) //NEW
    {
        maxHealth += amount; //NEW increase cap
        currentHealth += amount; //NEW heal by same amount

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //NEW safety

        if (hitbox != null) //NEW update UI
        {
            hitbox.SetMaxHealth(maxHealth);
        hitbox.SetHealth(currentHealth);
        }

        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHeal();
        }
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
    }
}
