using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float damage = 10f;

    [Header("Knockback (Tuning)")]
    [SerializeField] private float knockbackDistance = 4.5f;
    [SerializeField] private float knockbackDuration = 0.12f;

    [Header("VFX")]
    public VisualEffect hitVFX;

    private IObjectPool<Enemy> enemyPool;
    private bool isDead = false;

    private Coroutine knockbackRoutine;
    private bool isKnockedBack = false;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        enemyPool = pool;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead || isKnockedBack) return;

        if (player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    // Called by HammerAttack
    public void HandleHit(float damage, Vector3 attackerPosition, Vector3 hitPoint)
    {
        if (isDead) return;

        //Debug.Log("[ENEMY] Took HIT");

        VisualEffect vfx = Instantiate(hitVFX, hitPoint, Quaternion.identity);
        vfx.Play();
        Destroy(vfx.gameObject, 2f);
        
        currentHealth -= damage;

        //Debug.Log($"[ENEMY] Damage Applied: {damage} | HP Now: {currentHealth}");

        // --- Knockback ---
        if (knockbackDistance > 0f && knockbackDuration > 0f)
        {
            //Debug.Log("[ENEMY] Knockback SHOULD trigger");

            // NEW: use hit point for accurate knockback direction
            Vector3 dir = (transform.position - hitPoint); // NEW
            dir.y = 0f;                                   // NEW
            dir.Normalize();                              // NEW

            StartKnockback(dir, knockbackDistance, knockbackDuration);
        }

        // --- Death handling AFTER knockback ---
        if (currentHealth <= 0f)
        {
            if (isKnockedBack) // NEW
            {
                StartCoroutine(DieAfterKnockback()); // NEW
            }
            else
            {
                Die();
            }
        }
}


    private IEnumerator DieAfterKnockback() // NEW
    {
        //Debug.Log("[ENEMY] Waiting for knockback to finish before dying"); // NEW

        while (isKnockedBack)
            yield return null;

        Die();
    }


    private void StartKnockback(Vector3 direction, float distance, float duration)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        //Debug.Log("[ENEMY] Knockback START"); // NEW
        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction, distance, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float distance, float duration)
    {
        isKnockedBack = true;

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();

        Vector3 start = transform.position;
        Vector3 target = start + direction * distance;

        // NEW: snap target to NavMesh
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            target = hit.position;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        navMeshAgent.isStopped = false;
        isKnockedBack = false;
        knockbackRoutine = null;

        //Debug.Log("[ENEMY] Knockback END"); // NEW
    }

    private void Die()
    {
        isDead = true;
        //Debug.Log("[ENEMY] DIED"); // NEW

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
