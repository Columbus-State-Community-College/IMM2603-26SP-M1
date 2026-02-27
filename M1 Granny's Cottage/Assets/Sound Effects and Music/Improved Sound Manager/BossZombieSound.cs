using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossZombieSound : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Hit sound (non-looping)
    public void PlayHitSound()
    {
        SoundManager.instance.PlaySFX("Zombie1"); 
    }

    // Boss idle moan loop
    public void PlayMoanLoop()
    {
        AudioClip clip = GetClip("Zombie3");
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = SoundManager.instance.sfxVolume;
        audioSource.Play();
    }

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
            Debug.LogWarning("[BossZombieSound] Missing SFX: " + name);
            return null;
        }
        return item.clip;
    }
}
