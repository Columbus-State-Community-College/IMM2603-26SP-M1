using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector3 direction;

    private Collider owner;

    // initializes projectile
    public void Initialize(Vector3 dir, Collider enemyCollider)
    {
        direction = dir.normalized;
        owner = enemyCollider;

        transform.forward = direction;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    // when player is hit decrement health (ignore enemy collider as well)
    private void OnTriggerEnter(Collider other)
    {
        if (other == owner) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }
    }
}
