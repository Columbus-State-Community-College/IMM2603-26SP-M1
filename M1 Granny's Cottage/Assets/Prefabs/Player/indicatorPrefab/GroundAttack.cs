using UnityEngine;

public class GroundAttack : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab; // visual-only flat cylinder
    [SerializeField] private float maxRadius = 5f; // maximum AOE preview size

    // NEW
    [SerializeField] private float raycastHeight = 20f; // NEW height above player to cast from
    [SerializeField] private float raycastDistance = 50f; // NEW how far downward to check

    
    [Header("Hitbox Settings")]
    [SerializeField] private GameObject hitboxPrefab; // AOE trigger prefab

    private GameObject activeIndicator; // currently spawned indicator
    private float maxChargeTime; // hover duration passed in
    private float currentChargeTime; // runtime charge timer
    private bool isCharging; // indicator active flag

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

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

        Vector3 spawnPos = GetGroundPosition(playerPosition); // NEW changr from groundPos

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

        Debug.Log("[GROUND ATTACK] UpdateCharge running", activeIndicator);

        currentChargeTime += Time.deltaTime;

        // Normalize progress
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);

        // Calculate current radius
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        activeIndicator.transform.position = GetGroundPosition(playerPosition); // NEW
        // Scale visual (scale uses diameter, so radius * 2)
        activeIndicator.transform.localScale =
            new Vector3(radius * 2f, 0.02f, radius * 2f);

        if (currentChargeTime >= maxChargeTime)
        {
            Debug.Log("[GROUND ATTACK] Max charge reached — auto-ending charge");

            ExecuteGroundAttack(activeIndicator.transform.position, radius); // NEW
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

    // NEW Raycast downward ignoring Player layer
    private Vector3 GetGroundPosition(Vector3 playerPosition)
    {
        Vector3 rayStart = new Vector3(
            playerPosition.x,
            playerPosition.y + raycastHeight,
            playerPosition.z
        );

        // NEW Ignore Player layer
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, layerMask))
        {
            return hit.point;
        }

        // Fallback if ray misses
        return playerPosition;
    }

    // NEW
    private void ExecuteGroundAttack(Vector3 center, float radius)
    {
        if (hitboxPrefab == null)
        {
            Debug.LogWarning("[GROUND ATTACK] No hitbox prefab assigned");
            return;
        }

        GameObject hitbox = Instantiate(hitboxPrefab, center, Quaternion.identity);

        SphereCollider sphere = hitbox.GetComponent<SphereCollider>();

        if (sphere != null)
        {
            sphere.radius = radius; // match indicator radius
        }

        Debug.Log("[GROUND ATTACK] AOE executed with radius: " + radius);
    }
}
