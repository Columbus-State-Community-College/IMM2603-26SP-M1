using UnityEngine;
using UnityEngine.UI;

public class PlayerHitbox : MonoBehaviour
{
    PlayerHealth playerHealth;

    public Slider slider;

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            playerHealth.TakeDamage(enemy.damage);
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
