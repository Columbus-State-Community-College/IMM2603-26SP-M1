using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour
{
    public float damage = 10f;
    public float attackDuration = 0.2f;

    private bool attackActive = false;
    private Collider hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false; // OFF by default
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("[HAMMER] Attack started");

        attackActive = true;
        hitbox.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        hitbox.enabled = false;
        attackActive = false;

        Debug.Log("[HAMMER] Attack ended");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!attackActive) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        Debug.Log("[HAMMER] Enemy hit");

        enemy.HandleHit(damage, transform.position, other.ClosestPoint(transform.position));
    }
}
