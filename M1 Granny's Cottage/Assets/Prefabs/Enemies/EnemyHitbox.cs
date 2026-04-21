using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Enemy enemy;

    private float lastHitTime;
    public float hitCooldown = 0.5f;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(enemy.damage);
            lastHitTime = Time.time;
        }
    }
    //private Enemy enemy;

    //private void Awake()
    //{
    //    enemy = GetComponentInParent<Enemy>();
    //}

    //// Simply calls player health script to decrement health
    //private void OnTriggerEnter(Collider other)
    //{
    //    PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

    //    if (player != null)
    //    {
    //        player.TakeDamage(enemy.damage);
    //    }

    //}
}
