using UnityEngine;

public class HammerAttack : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        // Closest hit point on the collider we struck (useful for debugging / VFX later)
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        enemy.HandleHit(damage, transform.position, hitPoint);
    }
}
