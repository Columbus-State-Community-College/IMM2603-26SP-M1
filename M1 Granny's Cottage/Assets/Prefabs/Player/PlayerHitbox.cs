using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitbox : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerController playerController; //NEW

    public Slider slider;

    [Header("Hit Flash")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    private Material[] materials;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    [Header("Damage Cooldown")] //NEW
    [SerializeField] private float damageCooldown = 0.3f; //NEW
    private float damageCooldownTimer = 0f; //NEW

    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        playerController = GetComponentInParent<PlayerController>(); //NEW

        if (meshRenderer != null)
        {
            materials = meshRenderer.materials;
            originalColors = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_BaseColor"))
                    originalColors[i] = materials[i].GetColor("_BaseColor");
                else
                    originalColors[i] = materials[i].color;
            }
        }
    }

    private void Update() //NEW
    {
        if (damageCooldownTimer > 0f)
            damageCooldownTimer -= Time.deltaTime;
    }

    //NEW CHANGED from OnTriggerStay to OnTriggerEnter
    private void OnTriggerEnter(Collider other) //NEW
    {
        if (damageCooldownTimer > 0f) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            damageCooldownTimer = damageCooldown;

            playerHealth.TakeDamage(enemy.damage);

            if (playerController != null)
                playerController.TakeHit(enemy.transform.position, enemy.damage);

            FlashRed();
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    // function to call the flash routine
    public void FlashRed()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    // loops through and adds granny materials to a list in order to chnage color
    private IEnumerator FlashRoutine()
    {
        if (materials == null) yield break;

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_BaseColor"))
                materials[i].SetColor("_BaseColor", Color.red);
            else
                materials[i].color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_BaseColor"))
                materials[i].SetColor("_BaseColor", originalColors[i]);
            else
                materials[i].color = originalColors[i];
        }
    }
}