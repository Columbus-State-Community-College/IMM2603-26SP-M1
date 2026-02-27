using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;     // ONE source for all normal SFX

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool muteSFX = false;

    [Header("Sound Library")]
    public List<SoundItem> sfxClips = new List<SoundItem>();

    [System.Serializable]
    public class SoundItem
    {
        public string name;
        public AudioClip clip;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // CRITICAL: allow SFX to play during pause
        if (sfxSource != null)
            sfxSource.ignoreListenerPause = true;

        UpdateVolume();
    }

    public void PlaySFX(string name)
    {
        if (muteSFX || sfxSource == null) return;

        AudioClip clip = GetClipInternal(name);
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // Used by GameOutcomeSound
    public AudioClip GetClipInternal(string name)
    {
        SoundItem item = sfxClips.Find(s => s.name == name);

        if (item == null)
        {
            Debug.LogWarning("[SoundManager] Missing SFX: " + name);
            return null;
        }

        return item.clip;
    }

    public void SetVolume(float value)
    {
        sfxVolume = value;
        UpdateVolume();
    }

    public void Mute(bool mute)
    {
        muteSFX = mute;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        if (sfxSource != null)
            sfxSource.volume = muteSFX ? 0 : sfxVolume;
    }
}