using UnityEngine;

public class GroundAttack : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab; // visual-only flat cylinder
    [SerializeField] private float maxRadius = 5f; // maximum AOE preview size
    [SerializeField] private float groundY = 0f; // ground height to place indicator

    private GameObject activeIndicator; // currently spawned indicator
    private float maxChargeTime; // hover duration passed in
    private float currentChargeTime; // runtime charge timer
    private bool isCharging; // indicator active flag

    // Called when player hover begins
    public void StartCharge(Vector3 playerPosition, float hoverMaxTime)
    {
        if (indicatorPrefab == null)
        {
            Debug.LogWarning("[GROUND ATTACK] No indicator prefab assigned");
            return;
        }

        StopCharge(); // safety cleanup

        maxChargeTime = hoverMaxTime;
        currentChargeTime = 0f;
        isCharging = true;

        Vector3 spawnPos = new Vector3(playerPosition.x, groundY, playerPosition.z);
        activeIndicator = Instantiate(indicatorPrefab, spawnPos, Quaternion.identity);

        // Ensure indicator is visible on spawn
        activeIndicator.transform.localScale =
            new Vector3(0.5f, 0.02f, 0.5f);

        Debug.Log("[GROUND ATTACK] Charge started — indicator spawned", activeIndicator);
    }

    // Called every frame while hovering
    public void UpdateCharge(Vector3 playerPosition)
    {
        if (!isCharging || activeIndicator == null)
            return;

        // Debug proof that UpdateCharge is running
        Debug.Log("[GROUND ATTACK] UpdateCharge running", activeIndicator);

        currentChargeTime += Time.deltaTime;

        // Normalize charge progress based on hover duration
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);

        // Expand indicator over time
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        // Keep indicator positioned under player
        activeIndicator.transform.position =
            new Vector3(playerPosition.x, groundY, playerPosition.z);

        activeIndicator.transform.localScale =
            new Vector3(radius * 2f, 0.02f, radius * 2f);

        // Auto-stop if hover timer completes
        if (currentChargeTime >= maxChargeTime)
        {
            Debug.Log("[GROUND ATTACK] Max charge reached — auto-ending charge");
            StopCharge();
        }
    }

    // Called when hover ends or is cancelled
    public void StopCharge()
    {
        if (!isCharging)
            return;

        isCharging = false;

        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
            activeIndicator = null;
        }

        Debug.Log("[GROUND ATTACK] Charge ended — indicator removed");
    }
}
