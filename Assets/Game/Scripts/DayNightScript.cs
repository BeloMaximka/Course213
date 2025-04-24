using Assets.Game;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightScript : MonoBehaviour
{
    [SerializeField]
    private float dayDuration = 10.0f;
    private float dayTime = 12f;
    private float rotationAngle;
    private float dawnTime = 4.0f;
    private float noonTime = 7.0f;
    private float duskTime = 17.0f;
    private float nightTime = 20.0f;
    private float maxSkyboxExposure = 1.3f;

    [SerializeField]
    private Light sun;
    [SerializeField]
    private Light moon;
    private Material skyBox;
    bool isDay;

    private Material waterMat;
    public GameObject water;
    private float waterBaseBrightness;

    void Start()
    {
        waterMat = water.GetComponent<Renderer>().material;
        waterBaseBrightness = waterMat.GetFloat("_AlbedoIntensity");
        rotationAngle = -360.0f / dayDuration;
        dayTime = 12.0f - transform.eulerAngles.z / 360.0f * 24.0f;
        while (dayTime >= 24)
        {
            dayTime -= 24f;
        }
        while (nightTime >= 24)
        {
            dayTime += 24f;
        }
        skyBox = RenderSettings.skybox;
    }

    void Update()
    {

        dayTime += 24f * Time.deltaTime / dayDuration;
        dayTime %= 24f;

        float coef;
        if(dayTime >= dawnTime && dayTime < nightTime)
        {
            float t = (dayTime - dawnTime) / (duskTime - dawnTime);
            coef = Mathf.Clamp01(Mathf.Sin(t * Mathf.PI));

            sun.intensity = coef;
            waterMat.SetFloat("_AlbedoIntensity", coef * waterBaseBrightness + 0.2f);

            if (RenderSettings.sun != sun)
            {
                RenderSettings.sun = sun;
                moon.intensity = 0;
            }
        }
        else
        {
            float arg = dayTime < dawnTime ? dayTime : dayTime - 24.0f;
            coef = 0.3f * Mathf.Cos(arg * Mathf.PI / (dawnTime - (-dawnTime)));

            moon.intensity = coef;
            waterMat.SetFloat("_AlbedoIntensity", coef * waterBaseBrightness + 0.2f);
            if (RenderSettings.sun != moon)
            {
                RenderSettings.sun = moon;
                sun.intensity = 0;
            }
        }
        UpdateDayState(coef);
        RenderSettings.ambientIntensity = coef;
        skyBox.SetFloat("_Exposure", coef * maxSkyboxExposure);

        this.transform.Rotate(0, 0, rotationAngle * Time.deltaTime);
    }

    private void UpdateDayState(float coef)
    {
        if(coef < 0.5f && isDay)
        {
            isDay = false;
            GameEvents.OnNightBegin();
        }
        else if (coef > 0.5f && !isDay)
        {
            isDay = true;
            GameEvents.OnDayBegin();
        }
    }
}
