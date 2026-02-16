using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class HammerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackDuration = 0.5f;

    [Header("Particle VFX")]
    [SerializeField] private ParticleSystem slashParticles;
    [SerializeField] private ParticleSystem hitParticlesPrefab;
    [SerializeField] private float hitParticlesLifetime = 1f;

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
        if (attackActive) return;

        //Debug.Log("[HAMMER] Attack START");
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        attackActive = true;
        hitbox.enabled = true;

        if (trail != null) trail.enabled = true;

        if (slashParticles != null)
        {
            slashParticles.Play();
        }

        animator.ResetTrigger("isSwinging");
        animator.SetTrigger("isSwinging");

        yield return new WaitForSeconds(attackDuration);

        if (trail != null) trail.enabled = false;

        hitbox.enabled = false;
        attackActive = false;

        //Debug.Log("[HAMMER] Attack END");
    }

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Hammer hit: " + other.name);
        if (!attackActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        Vector3 contactPoint = other.ClosestPoint(hitbox.bounds.center);

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
}
