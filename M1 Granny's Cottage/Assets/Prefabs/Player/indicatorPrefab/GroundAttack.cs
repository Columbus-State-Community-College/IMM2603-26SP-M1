using UnityEngine;

public class GroundAttack : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab; // visual-only flat cylinder
    [SerializeField] private float maxRadius = 5f; // maximum AOE preview size

    [Header("Hitbox Settings")]
    [SerializeField] private GameObject hitboxPrefab; // AOE trigger prefab

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 2f;

    // Upgrade flag
    private bool canDealDamage = false;

    private GameObject activeIndicator; // currently spawned indicator
    private float maxChargeTime; // hover duration passed in
    private float currentChargeTime; // runtime charge timer
    private bool isCharging; // indicator active flag
    private CharacterController characterController;
    private GroundBelowPlayer _groundPosition; // the same position the camera follows
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        _groundPosition = GetComponent<GroundBelowPlayer>();
    }

    // Called by PlayerController when upgrade is applied
    public void EnableDamage()
    {
        canDealDamage = true;
    }

    // Called when player hover begins
    public void StartCharge(Vector3 playerPosition, float hoverMaxTime)
    {
        if (indicatorPrefab == null)
        {
            //Debug.LogWarning("[GROUND ATTACK] No indicator prefab assigned");
            return;
        }

        StopCharge(); // safety cleanup

        maxChargeTime = hoverMaxTime;
        currentChargeTime = 0f;
        isCharging = true;

        Vector3 spawnPos = _groundPosition.GroundPointTransform.position;//GetGroundPosition(playerPosition); // changr from groundPos

        activeIndicator = Instantiate(indicatorPrefab, spawnPos, Quaternion.identity);

        // Ensure indicator is visible on spawn
        activeIndicator.transform.localScale =
            new Vector3(0.5f, 0.02f, 0.5f);

        //Debug.Log("[GROUND ATTACK] Charge started — indicator spawned", activeIndicator);
    }

    // Called every frame while hovering
    public void UpdateCharge(Vector3 playerPosition)
    {
        if (!isCharging || activeIndicator == null)
            return;

        //Debug.Log("[GROUND ATTACK] UpdateCharge running", activeIndicator);

        currentChargeTime += Time.deltaTime;

        // Normalize progress
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);

        // Calculate current radius
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        activeIndicator.transform.position =
            _groundPosition.GroundPointTransform.position;//GetGroundPosition(playerPosition);
        // Scale visual (scale uses diameter, so radius * 2)

        activeIndicator.transform.localScale =
            new Vector3(radius * 2f, 0.02f, radius * 2f);

        if (currentChargeTime >= maxChargeTime)
        {
            //Debug.Log("[GROUND ATTACK] Max charge reached — auto-ending charge");
            ExecuteGroundAttack(activeIndicator.transform.position, radius);
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
    }

    private void ExecuteGroundAttack(Vector3 center, float radius)
    {
        if (hitboxPrefab == null)
            //Debug.LogWarning("[GROUND ATTACK] No hitbox prefab assigned");
            return;

        GameObject hitbox = Instantiate(hitboxPrefab, center, Quaternion.identity);

        SphereCollider sphere = hitbox.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            sphere.radius = radius; // match indicator radius
        }

        // Apply damage only if upgraded
        GroundAttackHitbox slamHitbox = hitbox.GetComponent<GroundAttackHitbox>();

        if (slamHitbox != null)
        {
            // NEW — pass stun duration from GroundAttack to the hitbox
            slamHitbox.SetStunDuration(stunDuration);

            if (canDealDamage)
            {
                HammerAttack hammer = GetComponent<HammerAttack>();

                if (hammer != null)
                {
                    float dynamicDamage = hammer.damage * 2f;
                    slamHitbox.SetDamageValues(dynamicDamage);
                }
            }
        }
    }

    //poewr up
    public void IncreaseStunDuration(float amount)
    {
        stunDuration += amount;

        Debug.Log("[POWERUP] Slam stun duration increased to: " + stunDuration);
    }  

    // Called by PlayerController when AOE size upgrade is applied
    public void IncreaseAOESize(float amount)
    {
        maxRadius += amount;

        Debug.Log("[POWERUP] Slam AOE size increased to: " + maxRadius);
    }
}