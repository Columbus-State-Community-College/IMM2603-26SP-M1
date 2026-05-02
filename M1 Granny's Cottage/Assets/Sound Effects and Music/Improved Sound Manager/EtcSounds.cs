using UnityEngine;
using System.IO;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EtcSounds : MonoBehaviour
{
    private AudioSource audioSource;
    private int daytimeSoundTimer;
    public bool isDayTime = true;
    private bool coroutineStart = false;
    private IEnumerator dayCoroutine;

    [Header("SFX Names (must match SoundManager list)")]
    public string upgradeSFX;
    public string switchSFX;
    public string recordScratchSFX;
    public string musicVolumeChangeSFX;
    public string outdoorDaytimeSFX;
    public string outdoorNighttimeSFX;

    [Header("Local Volume Settings (can exceed normal volume)")]
    [Range(0f, 5f)]
    public float upgradeVolume = 1f;

    [Range(0f, 5f)]
    public float switchVolume = 1f;

    [Range(0f, 5f)]
    public float recordScratchVolume = 1f;

    [Range(0f, 1f)]
    public float musicVolumeChangeVolume = 1f;

    [Range(0f, 1f)]
    public float outdoorDaytimeVolume = 1f;

    [Range(0f, 1f)]
    public float outdoorNighttimeVolume = 1f;
    

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
            Debug.LogWarning("[Etc Sounds] Missing clip: " + clipName);
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

    public void PlayRecordScratch()
    {
        PlayLocalClip(recordScratchSFX, recordScratchVolume);
    }

    public void PlayMusicVolumeChange()
    {
        PlayLocalClip(musicVolumeChangeSFX, musicVolumeChangeVolume);
    }

    public void PlayOutsideSounds()
    {
        daytimeSoundTimer = 0;
        if (isDayTime == true)
        {
            PlayLocalClip(outdoorDaytimeSFX, outdoorDaytimeVolume);

            if (coroutineStart == false)
            {
                coroutineStart = true;
                dayCoroutine = InstanceTimer(1.0f);
                StartCoroutine(dayCoroutine);
            }
        }

        if (isDayTime == false)
        {
            StopCoroutine(dayCoroutine);
            PlayLocalClip(outdoorNighttimeSFX, outdoorNighttimeVolume);
            coroutineStart = false;
        }
    }

    IEnumerator InstanceTimer(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            if (isDayTime == true)
            {
                //print("Daytime Time: " + daytimeSoundTimer);
                daytimeSoundTimer += 1;
                DaytimeSoundChecker();
            }
        }
    }

    public void DaytimeSoundChecker()
    {
        if (daytimeSoundTimer == 60)
        {
            PlayOutsideSounds();
        }
    }
}
