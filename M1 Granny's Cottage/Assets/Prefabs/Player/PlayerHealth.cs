using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public PlayerHitbox hitbox;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Debug.Log("Granny died");
    }
}
