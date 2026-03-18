using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float healAmount = 25f; // Amount of health restored to the player

    [Header("Optional Behavior Toggles")]
    [SerializeField] private bool onlyHealIfMissingHealth = true; // Prevent pickup if player is already at max health
    [SerializeField] private bool requireKeyPress = false; // If true, player must press a key to pick up
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Key used for manual pickup

    [Header("Optional Visual Effects")]
    [SerializeField] private bool enableFloating = false; // Enables floating animation
    [SerializeField] private float floatSpeed = 2f; // Speed of floating motion
    [SerializeField] private float floatHeight = 0.25f; // Height of floating motion

    [Header("Idle Particle Effect")]
    [SerializeField] private ParticleSystem idleEffect; // Constant looping particle effect while pickup exists

    [Header("Optional Audio / VFX")]
    [SerializeField] private AudioClip pickupSound; // Sound played when pickup is collected
    [SerializeField] private ParticleSystem pickupEffect; // Particle effect played when collected

    [Header("Optional Respawn")]
    [SerializeField] private bool enableRespawn = false; // If true, pickup will respawn instead of being destroyed
    [SerializeField] private float respawnDelay = 10f; // Time before the pickup respawns

    [Header("Ground Indicator")]
    [SerializeField] private GameObject groundIndicatorPrefab; // Prefab used for the ground location indicator
    [SerializeField] private float indicatorGroundY = 0f; // Height where the indicator sits (usually ground level)

    [SerializeField] private float indicatorRadius = 1f; // Radius of the ground indicator circle (controls overall size)
    [SerializeField] private float indicatorThickness = 0.02f; // Keeps the cylinder very thin so it appears like a 2D circle

    private GameObject activeIndicator; // Instance of the spawned indicator

    private Vector3 startPosition; // Original position used for floating animation
    private bool playerInRange = false; // Tracks if player is inside trigger
    private PlayerHealth cachedPlayerHealth; // Cached PlayerHealth reference for healing
    private Renderer[] renderers; // All renderers on the pickup (for enabling/disabling)
    private Collider pickupCollider; // Pickup collider reference

    private void Start()
    {
        startPosition = transform.position; // Store original position
        renderers = GetComponentsInChildren<Renderer>(); // Get all visual renderers
        pickupCollider = GetComponent<Collider>(); // Get collider used for pickup detection

        SpawnIndicator(); // Create the ground indicator if one is assigned

        if (idleEffect != null)
        {
            idleEffect.Play();
        }

        //Debug.Log("[HEALTH PICKUP] Ready. Heal Amount: " + healAmount);
    }

    private void Update()
    {
        HandleFloating(); // Apply floating animation if enabled

        UpdateIndicatorPosition(); // Keep indicator aligned with pickup X/Z position

        // If interaction requires a key press, check for input
        if (requireKeyPress && playerInRange && Input.GetKeyDown(interactKey))
        {
            AttemptHeal();
        }
    }

    // Called when something enters the pickup trigger
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return; // Ignore non-player objects

        cachedPlayerHealth = other.GetComponent<PlayerHealth>();
        if (cachedPlayerHealth == null)
        {
            Debug.LogWarning("[HEALTH PICKUP] PlayerHealth missing!");
            return;
        }

        playerInRange = true;

        // Automatically heal if key press is not required
        if (!requireKeyPress)
        {
            AttemptHeal();
        }
    }

    // Called when player leaves the trigger
    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        playerInRange = false;
        cachedPlayerHealth = null;
    }

    // Attempts to heal the player
    private void AttemptHeal()
    {
        if (cachedPlayerHealth == null) return;

        // Prevent pickup if player is already at full health
        if (onlyHealIfMissingHealth &&
            cachedPlayerHealth.currentHealth >= cachedPlayerHealth.maxHealth)
        {
            //Debug.Log("[HEALTH PICKUP] Player already at max health.");
            return;
        }

        // Heal the player
        cachedPlayerHealth.Heal(healAmount);
        //Debug.Log("[HEALTH PICKUP] Player healed for: " + healAmount);

        PlayEffects(); // Play audio and particle effects

        // Decide whether to respawn or destroy pickup
        if (enableRespawn)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Plays optional particle and sound effects
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

    // Handles optional floating animation
    private void HandleFloating()
    {
        if (!enableFloating) return;

        float newY = startPosition.y +
                     Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Move object up and down
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // Slowly rotate pickup
        transform.Rotate(Vector3.up * 60f * Time.deltaTime);
    }

    // Handles respawning after delay
    private IEnumerator RespawnRoutine()
    {
        //Debug.Log("[HEALTH PICKUP] Respawning in " + respawnDelay + " seconds");

        SetActiveState(false); // Hide pickup

        yield return new WaitForSeconds(respawnDelay);

        SetActiveState(true); // Re-enable pickup
    }

    // Enables or disables the pickup visuals and collider
    private void SetActiveState(bool state)
    {
        pickupCollider.enabled = state;

        foreach (Renderer r in renderers)
        {
            r.enabled = state;
        }

        if (idleEffect != null)
        {
            if (state)
            {
                idleEffect.Play();
            }
            else
            {
                idleEffect.Stop();
            }
        }

        // Also toggle the ground indicator
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(state);
        }
    }

    // Spawns the ground indicator under the pickup
    private void SpawnIndicator()
    {
        if (groundIndicatorPrefab == null) return; // Do nothing if no indicator prefab is assigned

        // Create position directly under the pickup using the configured ground height
        Vector3 pos = new Vector3(
            transform.position.x,
            indicatorGroundY,
            transform.position.z
        );

        // Instantiate the indicator prefab
        activeIndicator = Instantiate(
            groundIndicatorPrefab,
            pos,
            Quaternion.identity
        );

        activeIndicator.transform.localScale = new Vector3(
            indicatorRadius,
            indicatorThickness,
            indicatorRadius
        );
    }

    // Keeps the indicator positioned under the pickup
    private void UpdateIndicatorPosition()
    {
        if (activeIndicator == null) return;

        activeIndicator.transform.position = new Vector3(
            transform.position.x,
            indicatorGroundY,
            transform.position.z
        );
    }
}