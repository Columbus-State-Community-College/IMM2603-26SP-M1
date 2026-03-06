using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundVolumeManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource grannyAudioSource;
    public AudioSource enemyAudioSource;
    public AudioSource etcAudioSource;
    public TMP_Text musicVolumeText;
    public EtcSounds etcSoundScript;
    public Button musicVolDownButton;
    public Button musicVolUpButton;
    public Button soundVolDownButton;
    public Button soundVolUpButton;

    void Start()
    {
        musicVolumeText.text = "Music Volume: " + musicAudioSource.volume.ToString();
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
        grannyAudioSource.volume -=0.1f;
        enemyAudioSource.volume -=0.1f;
        etcAudioSource.volume -=0.1f;

        if(etcSoundScript != null)
        {
            etcSoundScript.PlaySwitch();
        }

        if(grannyAudioSource.volume == 0f && enemyAudioSource.volume == 0f && etcAudioSource.volume == 0f){
            soundVolDownButton.interactable = false;
        }

        if(grannyAudioSource.volume <= 1f && enemyAudioSource.volume <= 1f && etcAudioSource.volume <= 1f){
            soundVolUpButton.interactable = true;
        }
    }

    public void SoundVolumeUp()
    {
        grannyAudioSource.volume +=0.1f;
        enemyAudioSource.volume +=0.1f;
        etcAudioSource.volume +=0.1f;
        
        if(etcSoundScript != null)
        {
            etcSoundScript.PlaySwitch();
        }

        if(grannyAudioSource.volume >= 0f && enemyAudioSource.volume >= 0f && etcAudioSource.volume >= 0f){
            soundVolDownButton.interactable = true;
        }

        if (grannyAudioSource.volume == 1 && enemyAudioSource.volume == 1 && etcAudioSource.volume == 1){
            soundVolUpButton.interactable = false;
        }
    }
}
