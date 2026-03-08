using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class HammerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackDuration = 0.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackMultiplier = 1f; // Multiplier applied to enemy knockback
     
    [Header("Cooldown Settings")]  
    [SerializeField] private float attackCooldown = 0.8f;  
    private bool canAttack = true;  

    [Header("Particle VFX")]
    [SerializeField] private ParticleSystem slashParticles;
    [SerializeField] private ParticleSystem hitParticlesPrefab;
    [SerializeField] private float hitParticlesLifetime = 1f;

    [Header("Audio")]
    public GrannysSounds grannySoundScript;

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
        if (attackActive || !canAttack) return; // include cooldown check

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
        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHammerSwing();
        }
    }

    public void PlayHitSound()
    {
        if (grannySoundScript != null)
        {
            grannySoundScript.PlayHammerHit();
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false; // start cooldown lock

        animator.ResetTrigger("isSwinging");
        animator.SetTrigger("isSwinging");

        // fixes taking input before animation is finished
        yield return new WaitForSeconds(attackDuration);

        yield return new WaitForSeconds(attackCooldown); // cooldown delay

        canAttack = true; // reset cooldown
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Hammer hit: " + other.name);
        if (!attackActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;
        //Debug.Log("[HAMMER] Hit enemy with damage: " + damage); //power up log

        Vector3 contactPoint = other.ClosestPoint(hitbox.bounds.center);
        PlayHitSound();

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

        //Debug.Log("Enemy hit: " + enemy.name);
        // Apply knockback multiplier from powerup
    enemy.SetKnockbackMultiplier(knockbackMultiplier);

    enemy.HandleHit(
        damage,
        transform.position,
        contactPoint
);
    }

    // Powerup support
    public void ReduceCooldown(float multiplier)
    {
        attackCooldown *= multiplier;  

        // Optional safety clamp so cooldown never becomes 0
        attackCooldown = Mathf.Max(0.1f, attackCooldown);

        //Debug.Log("[POWERUP] Hammer attackCooldown: " + attackCooldown); //power up log
    }

    // Powerup support
    public void IncreaseDamage(float amount)
    {
        damage += amount;

        Debug.Log("[POWERUP] Hammer damage increased to: " + damage);// power up log
    }

    // Powerup support
    public void IncreaseKnockback()
    {
        knockbackMultiplier = 2f;
    }
}