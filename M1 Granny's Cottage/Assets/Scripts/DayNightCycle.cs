using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public enum TimeOfDay {  Day, Night }
    public TimeOfDay currentTime;

    [Header("Cycle Duration (seconds)")]
    public float dayDuration = 120f;
    public float nightDuration = 90f;

    float timer;

    public static event Action<TimeOfDay> OnTimeChanged;

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
