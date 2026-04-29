using UnityEngine;
using System.Collections;

public class ScoreMultiplier : MonoBehaviour
{
    [Header("Multiplier Settings")]
    [SerializeField] private float multiplier = 1.5f;

    [Header("Timer Settings")]
    [SerializeField] private bool useDuration = false; // Toggle timer on/off
    [SerializeField] private float duration = 10f;     // How long multiplier lasts

    [Header("Day/Night Behavior")]
    [SerializeField] private bool despawnOnNight = true;

    [SerializeField] private GrannyGlow grannyGlow;

    private float currentMultiplier;

    private Renderer[] renderers;
    private Collider pickupCollider;

    private Coroutine durationRoutine;

    private void OnEnable()
    {
        DayNightCycle.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        DayNightCycle.OnTimeChanged -= HandleTimeChanged;
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        pickupCollider = GetComponent<Collider>();

        currentMultiplier = multiplier;

        Debug.Log("[SCORE MULTIPLIER] Ready with value: " + currentMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        ApplyMultiplier();
        
        ScoreMultiplierZombie zombieScript = FindFirstObjectByType<ScoreMultiplierZombie>();
        if (zombieScript != null)
        {
            zombieScript.OnScoreMultiplierPickedUp();
        }

        Debug.Log("[SCORE MULTIPLIER] Pickup triggered");

        SetActiveState(false);
    }

    private void ApplyMultiplier()
    {
        ScoreCounter scoreCounter = FindFirstObjectByType<ScoreCounter>();

        if (scoreCounter != null)
        {
            scoreCounter.SetScoreMultiplier(currentMultiplier);
            Debug.Log("[SCORE MULTIPLIER] Applied: " + currentMultiplier);

            // reset previous timer before starting a new one
            if (useDuration)
            {
                if (durationRoutine != null)
                {
                    StopCoroutine(durationRoutine);
                    durationRoutine = null;
                }

                durationRoutine = StartCoroutine(MultiplierDuration(scoreCounter));
            }
        }
        else
        {
            Debug.LogWarning("[SCORE MULTIPLIER] Could not find ScoreCounter.");
        }
    }

    private IEnumerator MultiplierDuration(ScoreCounter scoreCounter)
    {
        Debug.Log("[SCORE MULTIPLIER] Timer started: " + duration + " seconds");

        if (grannyGlow != null)
        {
            grannyGlow.EnableGlow();
        }

        yield return new WaitForSeconds(duration);

        scoreCounter.SetScoreMultiplier(1f);

        if (grannyGlow != null)
        {
            grannyGlow.DisableGlow();
        }


        Debug.Log("[SCORE MULTIPLIER] Multiplier expired, reset to 1x");

        // CLEANUP
        durationRoutine = null;
    }

    private void HandleTimeChanged(DayNightCycle.TimeOfDay time)
    {
        if (time == DayNightCycle.TimeOfDay.Day)
        {
            // Stop any active timer when a new day starts
            if (durationRoutine != null)
            {
                StopCoroutine(durationRoutine);
                durationRoutine = null;
            }

            // day → respawn
            currentMultiplier = multiplier;
            SetActiveState(true);

            Debug.Log("[SCORE MULTIPLIER] Respawned for new day: " + currentMultiplier);
        }
        else if (time == DayNightCycle.TimeOfDay.Night)
        {
            if (despawnOnNight && pickupCollider.enabled)
            {
                // Only disable if it hasn't been picked up yet
                SetActiveState(false);

                Debug.Log("[SCORE MULTIPLIER] Despawned because night started");
            }
        }
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