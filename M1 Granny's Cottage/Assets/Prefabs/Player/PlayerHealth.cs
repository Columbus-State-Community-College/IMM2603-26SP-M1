using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Health UI and Audio Elements")]

    public PlayerHitbox hitbox;
    public GameObject GameOverScreen;
    public AudioSource audioSource;
    public AudioClip ScreamSound;
    public AudioClip HPLostSound;
    private float volume = 1f;

    void Start()
    {
        currentHealth = maxHealth;

        if (hitbox != null)
        {
            hitbox.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("[PLAYER] No PlayerHitbox assigned");
        }

        // NEW Log starting health
        Debug.Log("[PLAYER] Starting Health: " + currentHealth);
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

        audioSource.PlayOneShot(HPLostSound, volume = 0.4f);

        Debug.Log("[PLAYER] Current Health After Damage: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // NEW Heal function for health pickups
    public void Heal(float amount)
    {
        Debug.Log("[PLAYER] Attempting Heal: " + amount);

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hitbox != null)
        {
            hitbox.SetHealth(currentHealth);
        }

        Debug.Log("[PLAYER] Current Health After Heal: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("[PLAYER] Player Died");

        GameOverScreen.gameObject.SetActive(true);
        audioSource.PlayOneShot(ScreamSound, volume);
        Time.timeScale = 0;
    }
}
