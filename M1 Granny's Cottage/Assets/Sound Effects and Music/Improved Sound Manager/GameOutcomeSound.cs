using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameOutcomeSound : MonoBehaviour
{
    private bool hasPlayed = false;
    private AudioSource outcomeSource;

    [Header("SFX Keys (SoundManager Library Names)")]
    public string winSFX;
    public string loseSFX;

    [Header("Local Volume Multiplier")]
    [Range(0f, 3f)] public float outcomeVolume = 1f;

    private void Awake()
    {
        outcomeSource = GetComponent<AudioSource>();

        outcomeSource.playOnAwake = false;
        outcomeSource.loop = false;
        outcomeSource.spatialBlend = 0f;
        outcomeSource.ignoreListenerPause = true;

        if (SoundManager.instance != null && SoundManager.instance.sfxSource != null)
        {
            outcomeSource.outputAudioMixerGroup =
                SoundManager.instance.sfxSource.outputAudioMixerGroup;
        }
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.K))
    {
        Debug.Log("[GameOutcomeSound] Manual siren test");
        StartCoroutine(PlayOutcomeDelayed(loseSFX));
    }
}


    public void PlayWin()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        StartCoroutine(PlayOutcomeDelayed(winSFX));
    }

    public void PlayLose()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        StartCoroutine(PlayOutcomeDelayed(loseSFX));
    }

    private IEnumerator PlayOutcomeDelayed(string sfxName)
    {
        yield return new WaitForSecondsRealtime(0.05f);

        if (SoundManager.instance == null)
        {
            Debug.LogWarning("[GameOutcomeSound] SoundManager missing");
            yield break;
        }

        AudioClip clip = SoundManager.instance.GetClipInternal(sfxName);
        if (clip == null)
        {
            Debug.LogWarning("[GameOutcomeSound] Missing outcome SFX: " + sfxName);
            yield break;
        }

        float finalVolume = outcomeVolume * SoundManager.instance.sfxVolume;
        outcomeSource.PlayOneShot(clip, finalVolume);
    }
}
