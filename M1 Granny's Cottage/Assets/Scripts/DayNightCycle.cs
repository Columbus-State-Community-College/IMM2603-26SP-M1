using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour, IDataPersistence
{
    public enum TimeOfDay {  Day, Night }
    public TimeOfDay currentTime;

    [Header("Default Values: Cycle Duration (seconds)")]
    public float dayDuration = 120f;
    public float nightDuration = 90f;

    [Header("Sound Effect")]
    public EtcSounds etcSoundScript;

    [Header("Day and Night Music Changer")]
    public SceneMusicPlayer sceneMusicScript;

    float timer;

    public static event Action<TimeOfDay> OnTimeChanged;

    public static DayNightCycle Current { get; private set; }

    void Awake()
    {
        Current = this;
    }

    // Calculates and switches the time of day

    void Start()
    {
        currentTime = TimeOfDay.Day;
        timer = dayDuration;
        OnTimeChanged?.Invoke(currentTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0) 
        {
            if (etcSoundScript != null)
            {
                etcSoundScript.PlaySwitch();
            }
            
            SwitchTime();
        }
    }

    void SwitchTime()
    {
        if (currentTime == TimeOfDay.Day)
        {
            currentTime = TimeOfDay.Night;
            timer = nightDuration;
            sceneMusicScript.NightTimeMusic();
        }
        else
        {
            currentTime = TimeOfDay.Day;
            timer = dayDuration;
            etcSoundScript.PlayRecordScratch();
            sceneMusicScript.DayTimeMusic();
        }

        OnTimeChanged?.Invoke(currentTime);
    }

    public void LoadData(GameData data)
    {
        dayDuration = data.dayDuration;
        nightDuration = data.nightDuration;
    }

    public void SaveData(ref GameData data)
    {
        data.dayDuration = dayDuration;
        data.nightDuration = nightDuration;
    }
}
