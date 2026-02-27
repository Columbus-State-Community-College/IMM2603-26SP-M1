using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundVolumeManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource soundEffectAudioSource;
    public TMP_Text musicVolumeText;
    public TMP_Text soundEffectVolumeText;
    public EtcSounds etcSoundScript;
    public Button musicVolDownButton;
    public Button musicVolUpButton;
    public Button soundVolDownButton;
    public Button soundVolUpButton;

    void Start()
    {
        musicVolumeText.text = "Music Volume: " + musicAudioSource.volume.ToString();
        soundEffectVolumeText.text = "SFX Volume: " + soundEffectAudioSource.volume.ToString();
    }

    public void MusicVolumeDown()
    {
        musicAudioSource.volume -=0.1f;
        musicVolumeText.text = "Music Volume: " + musicAudioSource.volume.ToString();

        if(musicAudioSource.volume == 0f){
            musicVolDownButton.interactable = false;
        }

        if(musicAudioSource.volume <= 1f){
            musicVolUpButton.interactable = true;
        }
    }

    public void MusicVolumeUp()
    {
        musicAudioSource.volume +=0.1f;
        musicVolumeText.text = "Music Volume: " + musicAudioSource.volume.ToString();

        if(musicAudioSource.volume >= 0f){
            musicVolDownButton.interactable = true;
        }

        if (musicAudioSource.volume == 1){
            musicVolUpButton.interactable = false;
        }
    }

    public void SoundVolumeDown()
    {
        soundEffectAudioSource.volume -=0.1f;
        soundEffectVolumeText.text = "SFX Volume: " + soundEffectAudioSource.volume.ToString();

        if(etcSoundScript != null)
        {
            etcSoundScript.PlaySwitch();
        }

        if(soundEffectAudioSource.volume == 0f){
            soundVolDownButton.interactable = false;
        }

        if(soundEffectAudioSource.volume <= 1f){
            soundVolUpButton.interactable = true;
        }
    }

    public void SoundVolumeUp()
    {
        soundEffectAudioSource.volume +=0.1f;
        soundEffectVolumeText.text = "SFX Volume: " + soundEffectAudioSource.volume.ToString();
        
        if(etcSoundScript != null)
        {
            etcSoundScript.PlaySwitch();
        }

        if(soundEffectAudioSource.volume >= 0f){
            soundVolDownButton.interactable = true;
        }

        if (soundEffectAudioSource.volume == 1){
            soundVolUpButton.interactable = false;
        }
    }
}
