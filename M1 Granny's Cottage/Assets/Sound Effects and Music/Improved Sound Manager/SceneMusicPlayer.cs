using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [Header("Scene Music Settings")]
    [Tooltip("Name of the music track to play when this scene starts. Must match the name in MusicManager's list.")]
    public string musicTrackName = "ominous Music 1";

    [Tooltip("Play automatically when the scene starts.")]
    public bool playOnStart = true;

    [Tooltip("If true, fade in instead of instantly starting.")]
    public bool fadeIn = false;

    [Tooltip("Fade-in duration in seconds.")]
    public float fadeTime = 1.5f;

    private void Start()
    {
        if (!playOnStart) return;
        if (MusicManager.instance == null)
        {
            Debug.LogWarning("[SceneMusicPlayer] No MusicManager found in scene!");
            return;
        }

        if (fadeIn)
            StartCoroutine(MusicManager.instance.FadeInMusic(musicTrackName, fadeTime));
        else
            MusicManager.instance.PlayMusic(musicTrackName);
    }
}
