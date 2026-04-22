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

    [Tooltip("Time of Day Song Number")]
    public int daytimeSong;
    public int nighttimeSong;

    [Tooltip("Health Script Reader")]
    public PlayerHealth grannysHealthScript;

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
            // MusicManager.instance.PlayMusic(musicTrackName);
            DayTimeMusic();
    }

    private void Update()
    {
        if (grannysHealthScript.atGameOver == true)
        {
            // Debug.Log("Oh Noes!");
            grannysHealthScript.atGameOver = false;
            StopMusic();
        }
    }

    public void NightTimeMusic()
    {
        // Debug.Log("It's nighttime! Time to Jam!");
        if (DifficultyManager.Instance == null) return;
        nighttimeSong = DifficultyManager.Instance.nighttimeTrack;

        if (nighttimeSong == 1)
        {
            Debug.Log("This is the song that is played on the easy difficulty during the night.");
            MusicManager.instance.PlayMusic(musicTrackName = "POL-halloween-rnr-short");
        }

        if (nighttimeSong == 2)
        {
            Debug.Log("This is the song that is played on the normal difficulty during the night.");
            MusicManager.instance.PlayMusic(musicTrackName = "vampires_piano");
        }

        if (nighttimeSong == 3)
        {
            Debug.Log("This is the song that is played on the hard difficulty during the night.");
            MusicManager.instance.PlayMusic(musicTrackName = "Monolith");
        }
    }

    public void DayTimeMusic()
    {
        if (DifficultyManager.Instance == null) return;
        daytimeSong = DifficultyManager.Instance.daytimeTrack;

        if (daytimeSong == 1)
        {
            Debug.Log("This is the song that is played on the easy difficulty during the day.");
            MusicManager.instance.PlayMusic(musicTrackName = "Relaxing Interlude");
        }

        if (daytimeSong == 2)
        {
            Debug.Log("This is the song that is played on the normal difficulty during the day.");
            MusicManager.instance.PlayMusic(musicTrackName = "happy_plains_loop");
        }

        if (daytimeSong == 3)
        {
            Debug.Log("This is the song that is played on the hard difficulty during the day.");
            MusicManager.instance.PlayMusic(musicTrackName = "lvl_3_the_grassland");
        }
    }

    public void StopMusic()
    {
        MusicManager.instance.StopMusic();
    }
}
