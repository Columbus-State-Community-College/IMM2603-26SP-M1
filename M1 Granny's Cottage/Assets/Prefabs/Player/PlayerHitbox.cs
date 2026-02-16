using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitbox : MonoBehaviour
{
    PlayerHealth playerHealth;

    public Slider slider;

    [Header("Hit Flash")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    private Material[] materials;
    private Color[] originalColors;
    private Coroutine flashRoutine;


    private void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();

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

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            playerHealth.TakeDamage(enemy.damage);
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

    public void FlashRed()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

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
