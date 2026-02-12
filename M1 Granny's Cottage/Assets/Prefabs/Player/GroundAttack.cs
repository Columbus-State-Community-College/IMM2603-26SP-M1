using UnityEngine;

public class GroundAttack : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab;   // Visual preview
    [SerializeField] private float maxRadius = 5f;         // Max AOE size
    [SerializeField] private float raycastHeight = 20f;    // Height above player to cast from
    [SerializeField] private float raycastDistance = 50f;  // How far downward to check

    [Header("Hitbox Settings")]
    [SerializeField] private GameObject hitboxPrefab;      // AOE trigger prefab

    [Header("Debug")]
    [SerializeField] private bool enableLogs = true;

    private GameObject activeIndicator;
    private float maxChargeTime;
    private float currentChargeTime;
    private bool isCharging;

    // Called when hover begins
    public void StartCharge(Vector3 playerPosition, float hoverMaxTime)
    {
        if (indicatorPrefab == null)
        {
            Debug.LogWarning("GroundAttack: No indicator prefab assigned.");
            return;
        }

        StopCharge(); // Safety cleanup

        maxChargeTime = hoverMaxTime;
        currentChargeTime = 0f;
        isCharging = true;

        Vector3 groundPos = GetGroundPosition(playerPosition);

        activeIndicator = Instantiate(indicatorPrefab, groundPos, Quaternion.identity);

        // Start small
        activeIndicator.transform.localScale =
            new Vector3(1f, 0.02f, 1f);

        if (enableLogs)
            Debug.Log("GroundAttack: Charge started.");
    }

    // Called every frame while hovering
    public void UpdateCharge(Vector3 playerPosition)
    {
        if (!isCharging || activeIndicator == null)
            return;

        currentChargeTime += Time.deltaTime;

        // Normalize progress
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);

        // Calculate current radius
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        // Update ground position
        Vector3 groundPos = GetGroundPosition(playerPosition);
        activeIndicator.transform.position = groundPos;

        // Scale visual (scale uses diameter, so radius * 2)
        activeIndicator.transform.localScale =
            new Vector3(radius * 2f, 0.02f, radius * 2f);

        if (enableLogs)
            Debug.Log("GroundAttack: Charging... Radius = " + radius);

        // Auto execute if fully charged
        if (currentChargeTime >= maxChargeTime)
        {
            ExecuteGroundAttack(groundPos, radius);
            StopCharge();
        }
    }

    // Called when hover ends
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

        if (enableLogs)
            Debug.Log("GroundAttack: Charge ended.");
    }

    // Spawns the AOE hitbox
    private void ExecuteGroundAttack(Vector3 center, float radius)
    {
        if (hitboxPrefab == null)
        {
            Debug.LogWarning("GroundAttack: No hitbox prefab assigned.");
            return;
        }

        GameObject hitbox = Instantiate(hitboxPrefab, center, Quaternion.identity);

        SphereCollider sphere = hitbox.GetComponent<SphereCollider>();

        if (sphere != null)
        {
            sphere.radius = radius; // Must match indicator radius
        }

        if (enableLogs)
        {
            Debug.Log("GroundAttack: AOE executed.");
            Debug.Log("GroundAttack: Final Radius = " + radius);
        }
    }

    // Raycast downward ignoring Player layer
    private Vector3 GetGroundPosition(Vector3 playerPosition)
    {
        Vector3 rayStart = new Vector3(
            playerPosition.x,
            playerPosition.y + raycastHeight,
            playerPosition.z
        );

        // Ignore Player layer
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, layerMask))
        {
            return hit.point;
        }

        // Fallback if ray misses
        return playerPosition;
    }
}
