using UnityEngine;

public class SoundVolumeManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource soundEffectAudioSource;

    public void MusicVolumeDown()
    {
        musicAudioSource.volume -=0.1f;
    }

    public void MusicVolumeUp()
    {
        musicAudioSource.volume +=0.1f;
    }

    public void SoundVolumeDown()
    {
        soundEffectAudioSource.volume -=0.1f;
    }

    public void SoundVolumeUp()
    {
        soundEffectAudioSource.volume +=0.1f;
    }
}
