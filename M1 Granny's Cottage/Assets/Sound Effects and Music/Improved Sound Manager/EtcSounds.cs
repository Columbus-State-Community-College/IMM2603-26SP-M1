using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EtcSounds : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names (must match SoundManager list)")]
    public string upgradeSFX;
    public string switchSFX;

    [Header("Local Volume Settings (can exceed normal volume)")]
    [Range(0f, 5f)]
    public float upgradeVolume = 1f;

    [Range(0f, 5f)]
    public float switchVolume = 1f;

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

    public void PlayUpgrade()
    {
        PlayLocalClip(upgradeSFX, upgradeVolume);
    }

    public void PlaySwitch()
    {
        PlayLocalClip(switchSFX, switchVolume);
    }
}
