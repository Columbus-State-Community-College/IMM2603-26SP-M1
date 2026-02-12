using UnityEngine;

public class GroundAttackHitbox : MonoBehaviour
{
    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 2f;

    [Header("Damage Settings")]
    [SerializeField] private bool applyDamage = false;
    [SerializeField] private float damageAmount = 25f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 0.3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.ApplyStun(stunDuration);

            if (applyDamage)
            {
                enemy.HandleHit(damageAmount, transform.position, transform.position);
            }

            Debug.Log("GroundAttackHitbox: Enemy affected.");
        }
    }
}
