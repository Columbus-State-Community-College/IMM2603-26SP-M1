using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.VFX;

// Class to hold the info for each different demon type
[System.Serializable]
public class DemonType
{
    public string name;
    public GameObject mesh;
    public float damage;
    public float moveSpeed;
}

public class Enemy : MonoBehaviour
{
    // NOTE: do not remove rigidbody from enemy, or else the triggers for colliders will not fire

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

    // Enemy types
    [Header("Day Persona (Businessmen)")]
    [SerializeField] private GameObject dayMesh;
    [SerializeField] private float dayDamage = 10f;
    [SerializeField] private float daySpeed = 4f;

    [Header("Night Demon Types")]
    [SerializeField] private DemonType[] nightDemons;

    private DemonType activeDemonType;
    private DayNightCycle.TimeOfDay currentTime;



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

    // Controls switching the enemies from businessmen to demons
    private void OnEnable()
    {
        DayNightCycle.OnTimeChanged += OnTimeChanged;

        currentTime = DayNightCycle.Current.currentTime;
    }

    private void OnDisable()
    {
        DayNightCycle.OnTimeChanged -= OnTimeChanged;
    }

    // When time is changed (references day/night cycle), track
    // time and call functions that control the switch
    private void OnTimeChanged(DayNightCycle.TimeOfDay time)
    {
        currentTime = time;

        if (!gameObject.activeInHierarchy || isDead)
            return;

        if (currentTime == DayNightCycle.TimeOfDay.Night)
        {
            ChooseDemonType();
            ApplyDemonType();
        }
        else
        {
            ApplyDayPersona();
        }
    }

    // PLAYER DAMAGE + KNOCKBACK
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("[ENEMY] Player hit — calling TakeHit"); // DEBUG
                playerController.TakeHit(transform.position, damage);
            }
        }
    }

    // ENEMY TAKES DAMAGE (FROM PLAYER)

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

        // Knockback
        if (knockbackDistance > 0f && knockbackDuration > 0f)
        {
            //Debug.Log("[ENEMY] Knockback SHOULD trigger");

            // use hit point for accurate knockback direction
            Vector3 dir = (transform.position - hitPoint);
            dir.y = 0f;
            dir.Normalize();

            StartKnockback(dir, knockbackDistance, knockbackDuration);
        }

        // Death handling AFTER knockback
        if (currentHealth <= 0f)
        {
            if (isKnockedBack)
            {
                StartCoroutine(DieAfterKnockback());
            }
            else
            {
                Die();
            }
        }
    }

    private IEnumerator DieAfterKnockback()
    {
        //Debug.Log("[ENEMY] Waiting for knockback to finish before dying");
        while (isKnockedBack)
            yield return null;

        Die();
    }

    private void StartKnockback(Vector3 direction, float distance, float duration)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        //Debug.Log("[ENEMY] Knockback START");
        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction, distance, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float distance, float duration)
    {
        isKnockedBack = true;

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();

        Vector3 start = transform.position;
        Vector3 target = start + direction * distance;

        // snap target to NavMesh
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

        //Debug.Log("[ENEMY] Knockback END");
    }

    // Chooses random type for each enemy
    private void ChooseDemonType()
    {
        if (nightDemons == null || nightDemons.Length == 0)
            return;

        activeDemonType = nightDemons[Random.Range(0, nightDemons.Length)];
    }

    // Applies businessman persona, more changes can be made down the line
    // As of now, just mesh and damage
    private void ApplyDayPersona()
    {
        foreach (var demon in nightDemons)
        {
            if (demon.mesh != null)
                demon.mesh.SetActive(false);
        }

        if (dayMesh != null)
            dayMesh.SetActive(true);

        damage = dayDamage;
        navMeshAgent.speed = daySpeed;
    }

    // "Switches" the mesh and damage to the demon type it is assigned
    private void ApplyDemonType()
    {
        if (dayMesh != null)
            dayMesh.SetActive(false);

        foreach (var demon in nightDemons)
        {
            if (demon.mesh != null)
                demon.mesh.SetActive(false);
        }

        if (activeDemonType != null && activeDemonType.mesh != null)
        {
            activeDemonType.mesh.SetActive(true);
            damage = activeDemonType.damage;
            navMeshAgent.speed = activeDemonType.moveSpeed;

        }
    }

    private void Die()
    {
        isDead = true;
        //Debug.Log("[ENEMY] DIED");

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

        // Forces enemies to have the right mesh on spawn

        if (currentTime == DayNightCycle.TimeOfDay.Night)
        {
            ChooseDemonType();
            ApplyDemonType();
        }
        else
        {
            ApplyDayPersona();
        }
    }
}
