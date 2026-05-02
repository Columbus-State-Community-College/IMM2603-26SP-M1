using UnityEngine;

public class GroundAttack : MonoBehaviour, IDataPersistence
{
    [Header("Indicator Settings")]
    [SerializeField] private GameObject indicatorPrefab; // NOW slam ring
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

    // NEW — store original prefab scale so scaling behaves correctly
    private Vector3 indicatorBaseScale;

    private float radiusLogTimer = 0f;
    private const float radiusLogInterval = 1f;

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

    // START CHARGE
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

        Vector3 spawnPos = _groundPosition.GroundPointTransform.position;
        spawnPos.y += 0.01f; // NEW - prevent clipping

        // NEW - force correct rotation
        activeIndicator = Instantiate(
            indicatorPrefab,
            spawnPos,
            Quaternion.Euler(90f, 0f, 0f)
        );

        // NEW — store base scale of prefab
        indicatorBaseScale = activeIndicator.transform.localScale;
    }

    // NEW UPDATE CHARGE
    // Called every frame while hovering
    public void UpdateCharge(Vector3 playerPosition)
    {
        if (!isCharging || activeIndicator == null)
            return;

        //Debug.Log("[GROUND ATTACK] UpdateCharge running", activeIndicator);

        currentChargeTime += Time.deltaTime;

        // Normalize progress
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);
        t = Mathf.Pow(t, 2f);

        // Calculate current radius
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        radiusLogTimer += Time.deltaTime;

        if (radiusLogTimer >= radiusLogInterval)
        {
            Debug.Log("[GROUND ATTACK DEBUG] Current Radius: " + radius);
            radiusLogTimer = 0f;
        }

        Vector3 pos = _groundPosition.GroundPointTransform.position;
        pos.y += 0.01f; // NEW
        activeIndicator.transform.position = pos;

        // NEW — scale relative to prefab base size
        activeIndicator.transform.localScale = new Vector3(
            indicatorBaseScale.x * (radius * 2f),
            indicatorBaseScale.y * (radius * 2f),
            indicatorBaseScale.z
        );

        if (currentChargeTime >= maxChargeTime)
        {
            //Debug.Log("[GROUND ATTACK] Max charge reached — auto-ending charge");
            ExecuteGroundAttack(activeIndicator.transform.position, radius);
            StopCharge();
        }
    }

    // STOP CHARGE
    public void StopCharge()
    {
        if (!isCharging)
            return;

        isCharging = false;

        // calculate current radius based on charge progress
        float t = Mathf.Clamp01(currentChargeTime / maxChargeTime);
        float radius = Mathf.Lerp(0.5f, maxRadius, t);

        if (activeIndicator != null)
        {
            Vector3 slamPosition = activeIndicator.transform.position;

            // execute slam when player releases early
            ExecuteGroundAttack(slamPosition, radius);
            Debug.Log("[GROUND SLAM] Slam executed. Radius: " + radius);

            Destroy(activeIndicator);
            activeIndicator = null;
        }
    }

    // EXECUTE ATTACK
    private void ExecuteGroundAttack(Vector3 center, float radius)
    {
        if (hitboxPrefab == null)
            //Debug.LogWarning("[GROUND ATTACK] No hitbox prefab assigned");
            return;

        // EXISTING hitbox spawn
        GameObject hitbox = Instantiate(hitboxPrefab, center, Quaternion.identity);

        SphereCollider sphere = hitbox.GetComponent<SphereCollider>();
        if (sphere != null)
        {
               sphere.radius = radius;
        }

        GroundAttackHitbox slamHitbox = hitbox.GetComponent<GroundAttackHitbox>();

        if (slamHitbox != null)
        {
            // pass stun duration from GroundAttack to the hitbox
            slamHitbox.SetStunDuration(stunDuration);

            if (canDealDamage)
            {
                float slamDamage = 20f; // fixed empowered slam damage
                slamHitbox.SetDamageValues(slamDamage);

                //Debug.Log("[GROUND SLAM] Empowered slam damage applied: " + slamDamage); //power up log
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

    public void LoadData(GameData data)
    {
        stunDuration = data.jumpSlamStunDuration;
        maxRadius = data.jumpSlamMaxRadius;
    }

    public void SaveData(ref GameData data)
    {
        //data.jumpSlamStunDuration = stunDuration;
        //data.jumpSlamMaxRadius = maxRadius;
    }
}