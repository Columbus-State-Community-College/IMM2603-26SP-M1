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

    if (hitbox != null) // NEW
    {
        hitbox.SetMaxHealth(maxHealth); // NEW
    }
    else
    {
        Debug.LogWarning("[PLAYER] No PlayerHitbox assigned"); // NEW
    }
}


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        hitbox.SetHealth(currentHealth);
        audioSource.PlayOneShot(HPLostSound, volume = 0.4f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameOverScreen.gameObject.SetActive(true);
        audioSource.PlayOneShot(ScreamSound, volume);
        Time.timeScale = 0;
    }
}
