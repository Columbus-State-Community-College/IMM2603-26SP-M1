using System.Collections;
using System.Data;
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
    public EnemyRole role;
}

public enum EnemyRole
{
    Aggressor,
    Flanker,
    Ranged
}

public class Enemy : MonoBehaviour
{
    // NOTE: do not remove rigidbody from enemy, or else the triggers for colliders will not fire

    public Transform player;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float damage = 10f;
    public ScoreCounter scoreScript;
    public EnemySounds enemySoundScript;
    public int points = 100;

    [Header("Knockback (Tuning)")]
    [SerializeField] private float knockbackDistance = 4.5f;
    [SerializeField] private float knockbackDuration = 0.12f;

    private float knockbackMultiplier = 1f; // NEW - allows powerups to increase knockback

    [Header("Stun (Tuning)")]  
    [SerializeField] private bool canBeStunned = true;  
    private bool isStunned = false;  
    private Coroutine stunRoutine;  

    [Header("VFX")]
    public VisualEffect hitVFX;
    [SerializeField] private ParticleSystem dizzyParticles;

    private IObjectPool<Enemy> enemyPool;
    private bool isDead = false;

    private Coroutine knockbackRoutine;
    private bool isKnockedBack = false;

    [Header("Hit Flash")]
    private MeshRenderer[] meshRenderers;
    [SerializeField] private float flashDuration = 0.1f;

    private Material[] materials;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    // Enemy types
    [Header("Day Persona (Businessmen)")]
    [SerializeField] private GameObject dayMesh;
    [SerializeField] private float dayDamage = 10f;
    [SerializeField] private float daySpeed = 4f;
    [SerializeField] private EnemyRole dayRole = EnemyRole.Aggressor;

    [Header("Night Demon Types")]
    [SerializeField] private DemonType[] nightDemons;

    private DemonType activeDemonType;
    private DayNightCycle.TimeOfDay currentTime;

    [Header("AI")]
    public EnemyRole role;
    [SerializeField] private float surroundRadius = 3f;
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationStrength = 2f;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 10f;

    private float attackTimer;

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
        if (isDead || isKnockedBack || isStunned) return; // add || isStunned

        if (player ==  null) return;

        Vector3 targetPosition = player.position;

        switch (role)
        {
            case EnemyRole.Aggressor:
                targetPosition = player.position;
                break;

            case EnemyRole.Flanker:
                targetPosition = GetFlankPosition();
                break;

            case EnemyRole.Ranged:
                HandleRangedBehavior();
                return;
        }

        targetPosition += GetSeparationOffset();
        navMeshAgent.SetDestination(targetPosition);
    }

    public void ApplyDifficultyMultipliers()
    {
        if (DifficultyManager.Instance == null) return;

        maxHealth *= DifficultyManager.Instance.enemyHealthMultiplier;
        damage *= DifficultyManager.Instance.enemyDamageMultiplier;
        navMeshAgent.speed *= DifficultyManager.Instance.enemySpeedMultiplier;

        Debug.Log("[ENEMY] Difficulty applied:");
        Debug.Log("Health: " + maxHealth);
        Debug.Log("Damage: " + damage);
        Debug.Log("Speed: " + navMeshAgent.speed);
    }

    // make sure enemies don't clump together
    private Vector3 GetSeparationOffset()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);

        Vector3 pushAway = Vector3.zero;
        int count = 0;

        foreach (var col in nearby)
        {
            if (col.gameObject == gameObject) continue;
            if (!col.CompareTag("Enemy")) continue;

            Vector3 diff = transform.position - col.transform.position;
            pushAway += diff.normalized;
            count++;
        }

        if (count > 0)
            pushAway /= count;

        return pushAway * separationStrength;
    }

    // gets player position info to creep up on player
    private Vector3 GetFlankPosition()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 perpendicular = Vector3.Cross(dir, Vector3.up);

        return player.position + perpendicular * surroundRadius;
    }

    // Controls switching the enemies from businessmen to demons
    private void OnEnable()
    {
        DayNightCycle.OnTimeChanged += OnTimeChanged;

        if (DayNightCycle.Current != null)
        {
            currentTime = DayNightCycle.Current.currentTime;
        }

        GetMaterials();
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

    private void GetMaterials()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        var mats = new System.Collections.Generic.List<Material>();
        var colors = new System.Collections.Generic.List<Color>();

        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                mats.Add(mat);

                if (mat.HasProperty("_BaseColor"))
                    colors.Add(mat.GetColor("_BaseColor"));
                else
                    colors.Add(mat.color);
            }
        }

        materials = mats.ToArray();
        originalColors = colors.ToArray();
    }

    // REMOVED - Player damage + knockback now handled in PlayerHitbox
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                //Debug.Log("[ENEMY] Player hit — calling TakeHit"); // DEBUG
                playerController.TakeHit(transform.position, damage);
            }
        }
    }
    */
    // ENEMY TAKES DAMAGE (FROM PLAYER)
    // Called by HammerAttack

    public void FlashRed()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    // loops through and adds granny materials to a list in order to chnage color
    private IEnumerator FlashRoutine()
    {
        if (materials == null) yield break;

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_BaseColor"))
                materials[i].SetColor("_BaseColor", Color.red);
            else
                materials[i].color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_BaseColor"))
                materials[i].SetColor("_BaseColor", originalColors[i]);
            else
                materials[i].color = originalColors[i];
        }
    }

    public void HandleHit(float damage, Vector3 attackerPosition, Vector3 hitPoint)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("[ENEMY] Took damage: " + damage + " | Remaining Health: " + currentHealth); //power up log
        FlashRed();

        if (knockbackDistance > 0f && knockbackDuration > 0f)
        {
            Vector3 dir = (transform.position - hitPoint);
            dir.y = 0f;
            dir.Normalize();

            // Apply multiplier from hammer powerups
            float finalKnockback = knockbackDistance * knockbackMultiplier;
            Debug.Log("[ENEMY] Knockback applied. Multiplier: " + knockbackMultiplier + " | Distance: " + finalKnockback); //power up log

            StartKnockback(dir, finalKnockback, knockbackDuration);
        }

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

    // instantiates projectile, identifies target and direction
    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 dir = (target - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();

        if (p != null)
            p.Initialize(dir, GetComponent<Collider>());
    }

    //behavior for ranged enemy, looks at player, shoots
    private void HandleRangedBehavior()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        if (distance <= attackRange)
        {
            navMeshAgent.isStopped = true;

            if (attackTimer <= 0f)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);
        }
    }

    // Allows hammer powerups to modify enemy knockback strength
    public void SetKnockbackMultiplier(float multiplier)
    {
        knockbackMultiplier = multiplier;
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

    // Knockback for ground attack
    public void ApplyGroundKnockback(Vector3 sourcePosition, float distance, float duration)
    {
        if (isDead) return;

        Vector3 direction = transform.position - sourcePosition;
        direction.y = 0f;
        direction.Normalize();

        StartKnockback(direction, distance, duration);
    }

    public void ApplyStun(float duration)  
    {
        Debug.Log("[ENEMY] Stunned for: " + duration); //power up log
        if (!canBeStunned || isDead || isStunned)
            return;

        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunCoroutine(duration));
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    private IEnumerator StunCoroutine(float duration)  
    {
        isStunned = true;

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();

        if (dizzyParticles != null) 
            dizzyParticles.Play();

        yield return new WaitForSeconds(duration);

        if (dizzyParticles != null) 
            dizzyParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        navMeshAgent.isStopped = false;
        isStunned = false;
        stunRoutine = null;
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

        damage = dayDamage * DifficultyManager.Instance.enemyDamageMultiplier;
        navMeshAgent.speed = daySpeed * DifficultyManager.Instance.enemySpeedMultiplier;
        role = dayRole;

        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();


        GetMaterials();
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
            role = activeDemonType.role;
        }

        GetMaterials();
        ApplyDifficultyMultipliers();
    }

    private void Die()
    {
        GameObject enemySoundObject = GameObject.Find("EnemySoundManager");
        if (enemySoundObject)
        {
            EnemySounds enemySoundScript = enemySoundObject.GetComponent<EnemySounds>();
            if (enemySoundScript != null)
            {
                enemySoundScript.PlayMonsterScream();
            }
        }

        GameObject playerObject = GameObject.Find("PlayerParent");
        if (playerObject)
        {
            ScoreCounter scoreScript = playerObject.GetComponent<ScoreCounter>();
            if (scoreScript != null)
            {
                Debug.Log("NOOOOOOO");
                scoreScript.addPoints(points);
            }
        }
        isDead = true;
        //Debug.Log("[ENEMY] DIED");

        if (Spawner.Instance != null)
            Spawner.Instance.aliveEnemies--;

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }

        if (stunRoutine != null) StopCoroutine(stunRoutine);  
        isStunned = false;  

        isKnockedBack = false;

        if (enemyPool != null)
        {
            enemyPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
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

        if (stunRoutine != null) StopCoroutine(stunRoutine);  
        isStunned = false;  

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