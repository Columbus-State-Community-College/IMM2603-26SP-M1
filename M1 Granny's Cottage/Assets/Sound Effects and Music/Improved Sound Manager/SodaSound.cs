using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SodaSound : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Sound Names (match SoundManager list)")]
    public string openCanSFX = "soda_open";
    public string drinkSFX = "soda_drink";
    public string dropCanSFX = "soda_drop";

    [Header("Local Volume Multiplier")]
    [Range(0f, 3f)] public float openVol = 1f;
    [Range(0f, 3f)] public float drinkVol = 1f;
    [Range(0f, 3f)] public float dropVol = 1f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private AudioClip GetClip(string soundName)
    {
        var item = SoundManager.instance.sfxClips.Find(s => s.name == soundName);
        if (item == null)
        {
            Debug.LogWarning("[SodaSound] Missing SFX: " + soundName);
            return null;
        }
        return item.clip;
    }

    private void PlayClip(string soundName, float vol)
    {
        AudioClip clip = GetClip(soundName);
        if (clip == null) return;

        float finalVol = SoundManager.instance.sfxVolume * vol;
        audioSource.PlayOneShot(clip, finalVol);
    }

    public void PlayOpen() => PlayClip(openCanSFX, openVol);
    public void PlayDrink() => PlayClip(drinkSFX, drinkVol);
    public void PlayDrop() => PlayClip(dropCanSFX, dropVol);
}
