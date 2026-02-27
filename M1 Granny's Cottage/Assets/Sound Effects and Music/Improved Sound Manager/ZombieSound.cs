using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZombieSound : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Called when zombie is hit (non-looping)
    public void PlayHitSound()
    {
        SoundManager.instance.PlaySFX("Zombie1"); // plays one-shot hit sound
    }

    // Called when zombie starts idling or patrolling
    public void PlayMoanLoop()
    {
        AudioClip clip = GetClip("Zombie3");
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = SoundManager.instance.sfxVolume;
        audioSource.Play();
    }

    // Stops the looping sound (for death or despawn)
    public void StopMoan()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private AudioClip GetClip(string name)
    {
        var item = SoundManager.instance.sfxClips.Find(s => s.name == name);
        if (item == null)
        {
            Debug.LogWarning("[ZombieSound] Missing SFX: " + name);
            return null;
        }
        return item.clip;
    }
}
