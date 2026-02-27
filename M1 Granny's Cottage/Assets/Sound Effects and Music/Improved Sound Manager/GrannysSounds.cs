using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GrannysSounds : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names (must match SoundManager list)")]
    public string jumpSFX;
    public string hammerSwingSFX;
    public string hammerHitSFX;
    public string healSFX;
    public string hpLostSFX;
    public string screamSFX;

    [Header("Local Volume Settings (can exceed normal volume)")]
    [Range(0f, 5f)]
    public float jumpVolume = 1f;

    [Range(0f, 5f)]
    public float hammerSwingVolume = 1f;

    [Range(0f, 5f)]
    public float hammerHitVolume = 1f;

    [Range(0f, 5f)]
    public float healVolume = 1f;

    [Range(0f, 5f)]
    public float hpLostVolume = 1f;

    [Range(0f, 5f)]
    public float screamVolume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private AudioClip GetClip(string clipName)
    {
        var item = SoundManager.instance.sfxClips.Find(s => s.name == clipName);
        if (item == null)
        {
            Debug.LogWarning("[Grannys Sounds] Missing clip: " + clipName);
            return null;
        }
        return item.clip;
    }

    private void PlayLocalClip(string clipName, float localVolume)
    {
        AudioClip clip = GetClip(clipName);
        if (clip == null) return;

        float finalVolume = SoundManager.instance.sfxVolume * localVolume;

        // allow up to 300% volume without clipping too hard
        finalVolume = Mathf.Clamp(finalVolume, 0f, 3f);

        audioSource.PlayOneShot(clip, finalVolume);
    }

    public void PlayJump()
    {
        PlayLocalClip(jumpSFX, jumpVolume);
    }

    public void PlayHammerSwing()
    {
        PlayLocalClip(hammerSwingSFX, hammerSwingVolume);
    }

    public void PlayHammerHit()
    {
        PlayLocalClip(hammerHitSFX, hammerHitVolume);
    }

    public void PlayHeal()
    {
        PlayLocalClip(healSFX, healVolume);
    }

    public void PlayHPLost()
    {
        PlayLocalClip(hpLostSFX, hpLostVolume);
    }

    public void PlayScream()
    {
        PlayLocalClip(screamSFX, screamVolume);
    }
}
