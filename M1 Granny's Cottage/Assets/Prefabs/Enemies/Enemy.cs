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

    // New Used by HammerAttack
    public void HandleHit(float damage, Vector3 attackerPosition, Vector3 hitPoint)
{
    if (isDead) return;

    Debug.Log("[ENEMY] Took damage");

    currentHealth -= damage;

    // New: confirm knockback trigger
    if (knockbackDistance > 0f && knockbackDuration > 0f)
    {
        Debug.Log("[ENEMY] Knockback triggered");

        Vector3 dir = (transform.position - attackerPosition);
        dir.y = 0f;
        dir.Normalize();

        StartKnockback(dir, knockbackDistance, knockbackDuration);
    }

    if (currentHealth <= 0f)
        Die();
}

    // Old overload kept for safety
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

        bool agentWasEnabled = navMeshAgent != null && navMeshAgent.enabled;
        if (agentWasEnabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        Vector3 start = transform.position;
        Vector3 target = start + direction * distance;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            target = hit.position;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        if (agentWasEnabled)
            navMeshAgent.isStopped = false;

        isKnockedBack = false;
        knockbackRoutine = null;

        // New log CONFIRM knockback finished
        Debug.Log("[ENEMY] Knockback END");
    }

    void Die()
    {
        isDead = true;

        // New log enemy died
        Debug.Log("[ENEMY] DIED");

        if (Spawner.Instance != null)
            Spawner.Instance.aliveEnemies--;

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

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
        isKnockedBack = false;
    }
}
