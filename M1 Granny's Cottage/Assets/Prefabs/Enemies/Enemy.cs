using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    [SerializeField] float maxHealth = 30f;
    float currentHealth;

    public float damage = 10f;

    [Header("Knockback (Tuning)")]
    [SerializeField] private float knockbackDistance = 1.5f;
    [SerializeField] private float knockbackDuration = 0.12f;

    private IObjectPool<Enemy> enemyPool;
    private bool isDead = false;

    // Knockback state
    private Coroutine knockbackRoutine;
    private bool isKnockedBack = false;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        enemyPool = pool;
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;
        if (isKnockedBack) return;

        if (player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    // New signature used by HammerAttack
    public void HandleHit(float damage, Vector3 attackerPosition, Vector3 hitPoint)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Knockback (tuned in inspector)
        if (knockbackDistance > 0f && knockbackDuration > 0f)
        {
            Vector3 dir = (transform.position - attackerPosition);
            dir.y = 0f;

            // Fallback if attackerPosition is basically the same point
            if (dir.sqrMagnitude < 0.0001f)
                dir = -transform.forward;

            dir.Normalize();
            StartKnockback(dir, knockbackDistance, knockbackDuration);
        }

        if (currentHealth <= 0f)
            Die();
    }

    // Keep old signature in case something else still calls it
    public void HandleHit(float damage)
    {
        HandleHit(damage, transform.position - transform.forward, transform.position);
    }

    private void StartKnockback(Vector3 direction, float distance, float duration)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction, distance, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float distance, float duration)
    {
        isKnockedBack = true;

        // Stop NavMesh steering so it doesn’t instantly counter-move
        bool agentWasEnabled = navMeshAgent != null && navMeshAgent.enabled;
        if (agentWasEnabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        Vector3 start = transform.position;
        Vector3 target = start + direction * distance;

        // Optional: try to keep the target on the NavMesh to reduce wall/edge weirdness
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            target = hit.position;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            transform.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }

        if (agentWasEnabled)
            navMeshAgent.isStopped = false;

        isKnockedBack = false;
        knockbackRoutine = null;
    }

    void Die()
    {
        isDead = true;

        if (Spawner.Instance != null)
            Spawner.Instance.aliveEnemies--;

        // Stop knockback if we die mid-knockback
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
        isKnockedBack = false;

        if (enemyPool != null)
            enemyPool.Release(this);
        else
            gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        isDead = false;
        currentHealth = maxHealth;

        // Reset knockback state for pooled enemies
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
        isKnockedBack = false;
    }
}
