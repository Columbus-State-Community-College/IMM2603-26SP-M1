using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float healAmount = 25f;

    [Header("Optional Behavior Toggles")]
    [SerializeField] private bool onlyHealIfMissingHealth = true;
    [SerializeField] private bool requireKeyPress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Optional Visual Effects")]
    [SerializeField] private bool enableFloating = false;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.25f;

    [Header("Optional Audio / VFX")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupEffect;

    [Header("Optional Respawn")]
    [SerializeField] private bool enableRespawn = false;
    [SerializeField] private float respawnDelay = 10f;

    private Vector3 startPosition;
    private bool playerInRange = false;
    private PlayerHealth cachedPlayerHealth;
    private Renderer[] renderers;
    private Collider pickupCollider;

    private void Start()
    {
        startPosition = transform.position;
        renderers = GetComponentsInChildren<Renderer>();
        pickupCollider = GetComponent<Collider>();

        Debug.Log("[HEALTH PICKUP] Ready. Heal Amount: " + healAmount);
    }

    private void Update()
    {
        HandleFloating();

        if (requireKeyPress && playerInRange && Input.GetKeyDown(interactKey))
        {
            AttemptHeal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        cachedPlayerHealth = other.GetComponent<PlayerHealth>();
        if (cachedPlayerHealth == null)
        {
            Debug.LogWarning("[HEALTH PICKUP] PlayerHealth missing!");
            return;
        }

        playerInRange = true;

        if (!requireKeyPress)
        {
            AttemptHeal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        playerInRange = false;
        cachedPlayerHealth = null;
    }

    private void AttemptHeal()
    {
        if (cachedPlayerHealth == null) return;

        if (onlyHealIfMissingHealth &&
            cachedPlayerHealth.currentHealth >= cachedPlayerHealth.maxHealth)
        {
            Debug.Log("[HEALTH PICKUP] Player already at max health.");
            return;
        }

        cachedPlayerHealth.Heal(healAmount);
        Debug.Log("[HEALTH PICKUP] Player healed for: " + healAmount);

        PlayEffects();

        if (enableRespawn)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayEffects()
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }

    private void HandleFloating()
    {
        if (!enableFloating) return;

        float newY = startPosition.y +
                     Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        transform.Rotate(Vector3.up * 60f * Time.deltaTime);
    }

    private IEnumerator RespawnRoutine()
    {
        Debug.Log("[HEALTH PICKUP] Respawning in " + respawnDelay + " seconds");

        SetActiveState(false);

        yield return new WaitForSeconds(respawnDelay);

        SetActiveState(true);
    }

    private void SetActiveState(bool state)
    {
        pickupCollider.enabled = state;

        foreach (Renderer r in renderers)
        {
            r.enabled = state;
        }
    }
}
