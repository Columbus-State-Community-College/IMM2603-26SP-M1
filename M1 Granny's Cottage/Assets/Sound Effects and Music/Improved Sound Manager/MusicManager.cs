using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Audio Source (Music)")]
    public AudioSource musicSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    public bool muteMusic = false;

    [Header("Music Library")]
    public List<MusicItem> musicTracks = new List<MusicItem>();

    [System.Serializable]
    public class MusicItem
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

        UpdateVolume();
    }

    public void PlayMusic(string name)
    {
        MusicItem item = musicTracks.Find(m => m.name == name);
        if (item == null)
        {
            Debug.LogWarning("[MusicManager] Music not found: " + name);
            return;
        }

        musicSource.clip = item.clip;
        musicSource.loop = true;

        if (!muteMusic)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetVolume(float value)
    {
        musicVolume = value;
        UpdateVolume();
    }

    public void Mute(bool mute)
    {
        muteMusic = mute;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        musicSource.volume = muteMusic ? 0 : musicVolume;
    }

    public IEnumerator FadeInMusic(string name, float fadeTime = 1.5f)
    {
        PlayMusic(name);
        musicSource.volume = 0f;

        float t = 0f;
        while (t < fadeTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

}
