using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public enum TimeOfDay {  Day, Night }
    public TimeOfDay currentTime;

    [Header("Cycle Duration (seconds)")]
    public float dayDuration = 120f;
    public float nightDuration = 90f;

    [Header("Sound Effect")]
    public EtcSounds etcSoundScript;

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
        }
        else
        {
            currentTime = TimeOfDay.Day;
            timer = dayDuration;
        }

        OnTimeChanged?.Invoke(currentTime);
    }
}
