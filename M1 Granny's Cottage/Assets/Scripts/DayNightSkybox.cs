using UnityEngine;

public class DayNightSkybox : MonoBehaviour
{
    public Material daySkybox;
    public Material nightSkybox;

    // Changes skybox based on the time

    void OnEnable()
    {
        DayNightCycle.OnTimeChanged += ChangeSkybox;
    }

    private void OnDisable()
    {
        DayNightCycle.OnTimeChanged -= ChangeSkybox;
    }

    void ChangeSkybox(DayNightCycle.TimeOfDay time)
    {
        if (time == DayNightCycle.TimeOfDay.Day)
            RenderSettings.skybox = daySkybox;
        else
            RenderSettings.skybox = nightSkybox;

        DynamicGI.UpdateEnvironment();
    }
}
