using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class HammerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackDuration = 0.5f;

    //NEW
    [Header("Cooldown Settings")] //NEW
    [SerializeField] private float attackCooldown = 0.8f; //NEW
    private bool canAttack = true; //NEW

    [Header("Particle VFX")]
    [SerializeField] private ParticleSystem slashParticles;
    [SerializeField] private ParticleSystem hitParticlesPrefab;
    [SerializeField] private float hitParticlesLifetime = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private AudioClip hammerSwingSound;
    [SerializeField] private AudioClip hammerHitSound;
    [SerializeField] private AudioClip hammerHitEnemySound;
    [SerializeField] private float volume;

    [Header("Other Components")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer trail;

    private bool attackActive = false;
    private Collider hitbox;

    private void Awake()
    {
        hitbox = GetComponent<Collider>();

        if (hitbox == null)
        {
            //Debug.LogError("[HAMMER] No collider found on Hammer!");
            return;
        }

        hitbox.enabled = false;
    }

    // this ensures to hook in the hammerAttack to the PlayerController
    void OnEnable()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerController.hammerAttack = this;
    }

    // Called by PlayerController
    public void StartAttack()
    {
        if (attackActive || !canAttack) return; //NEW include cooldown check

        //Debug.Log("[HAMMER] Attack START");
        StartCoroutine(AttackRoutine());
    }

    // the next 3 functions are called from anmation events on the swing
    public void EnableHitbox()
    {
        hitbox.enabled = true;

        if (trail != null)
            trail.enabled = true;

        if (slashParticles != null)
            slashParticles.Play();

        attackActive = true;
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;

        if (trail != null)
            trail.enabled = false;

        attackActive = false;
    }

    public void PlaySwooshSound()
    {
        playerController.audioSource.PlayOneShot(hammerSwingSound, volume);
    }

    public void PlayHitSound()
    {
        playerController.audioSource.PlayOneShot(hammerHitSound, volume);
    }

    public void PlayEnemyHitSound()
    {
        enemyAudioSource.PlayOneShot(hammerHitEnemySound);
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false; //NEW start cooldown lock

        animator.ResetTrigger("isSwinging");
        animator.SetTrigger("isSwinging");

        // fixes taking input before animation is finished
        yield return new WaitForSeconds(attackDuration);

        yield return new WaitForSeconds(attackCooldown); //NEW cooldown delay

        canAttack = true; //NEW reset cooldown
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Hammer hit: " + other.name);
        if (!attackActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        Vector3 contactPoint = other.ClosestPoint(hitbox.bounds.center);
        PlayHitSound();
        PlayEnemyHitSound();

        if (hitParticlesPrefab != null)
        {
            ParticleSystem hitVFX = Instantiate(
                hitParticlesPrefab,
                contactPoint,
                Quaternion.identity
            );

            hitVFX.Play();
            Destroy(hitVFX.gameObject, hitParticlesLifetime);
        }

        //Debug.Log("[HAMMER] Enemy HIT");
        //Debug.Log("Enemy hit: " + enemy.name);
        enemy.HandleHit(
            damage,
            transform.position,
            contactPoint
        );
    }

    //NEW Powerup support
    public void ReduceCooldown(float multiplier) //NEW
    {
        attackCooldown *= multiplier; //NEW

        // Optional safety clamp so cooldown never becomes 0
        attackCooldown = Mathf.Max(0.1f, attackCooldown); //NEW
    }
}