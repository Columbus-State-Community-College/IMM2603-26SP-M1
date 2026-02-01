using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float attackDuration = 0.2f;

    [SerializeField] private PlayerController playerController;

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

        //Debug.Log("[HAMMER] Attack START"); // NEW
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        attackActive = true;
        hitbox.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        hitbox.enabled = false;
        attackActive = false;

        //Debug.Log("[HAMMER] Attack END"); // NEW
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        //Debug.Log("[HAMMER] Enemy HIT"); // NEW

        enemy.HandleHit(
            damage,
            transform.position,
            other.ClosestPoint(transform.position)
        );
    }
}
