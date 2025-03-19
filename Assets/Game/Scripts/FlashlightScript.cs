using Assets.Game;
using UnityEngine;

public class FlashlightScirpt : MonoBehaviour
{
    [SerializeField]
    GameObject playerCamera;

    [SerializeField]
    GameObject playerCameraController;

    Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
        GameEvents.DayBegin += DisableFlashlight;
        GameEvents.NightBegin += EnableFlashlight;
    }

    void Update()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
    }

    private void OnDestroy()
    {
        GameEvents.DayBegin -= DisableFlashlight;
        GameEvents.NightBegin -= EnableFlashlight;
    }

    private void EnableFlashlight()
    {
        flashlight.intensity = 1;
    }

    private void DisableFlashlight()
    {
        flashlight.intensity = 0;
    }
}
