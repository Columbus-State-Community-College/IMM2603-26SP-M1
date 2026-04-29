using UnityEngine;

public class GrannyGlow : MonoBehaviour
{
    [SerializeField] private Renderer grannyRenderer;
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowIntensity = 2f;

    private Material[] mats;
    private Color[] baseEmissions;

    void Start()
    {
        mats = grannyRenderer.materials;

        baseEmissions = new Color[mats.Length];

        for (int i = 0; i < mats.Length; i++)
        {
            baseEmissions[i] = mats[i].GetColor("_EmissionColor");
        }

        DisableGlow();
    }

    public void EnableGlow()
    {
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].EnableKeyword("_EMISSION");
            mats[i].SetColor("_EmissionColor", glowColor * glowIntensity);
        }
    }

    public void DisableGlow()
    {
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_EmissionColor", baseEmissions[i]);
            mats[i].DisableKeyword("_EMISSION");
        }
    }
}
