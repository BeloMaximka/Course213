using Assets.Game.Global;
using UnityEngine;

public class HudInertiaScript : MonoBehaviour
{
    [Tooltip("The camera the HUD should follow")]
    public Transform cameraTransform;

    GameObject camera;

    [Tooltip("Time (in seconds) it takes to reach ~63% of the difference")]
    public float smoothTime = 0.2f;

    public Transform headbobJoint;
    public RectTransform healthBar;

    // Internal state for SmoothDampAngle
    private float smoothX;
    private float smoothY;
    private float velX;
    private float velY;

    
    private float healthBarMaxWidth;

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        healthBarMaxWidth = healthBar.rect.width;
        PlayerState.HealthChanged += UpdateHealthBar;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, headbobJoint.position.y, transform.position.z);

        // Smooth each axis independently
        smoothX = Mathf.SmoothDampAngle(smoothX, camera.transform.rotation.eulerAngles.x, ref velX, smoothTime);
        smoothY = Mathf.SmoothDampAngle(smoothY, cameraTransform.eulerAngles.y, ref velY, smoothTime);

        // Apply the smoothed rotation to the HUD
        transform.rotation = Quaternion.Euler(smoothX, smoothY, 0f);
    }

    void UpdateHealthBar(int newHealth)
    {
        float ratio = (float)newHealth / PlayerState.MaxHealth;
        healthBar.sizeDelta = new Vector2(healthBarMaxWidth * ratio, healthBar.sizeDelta.y);
    }

    private void OnDestroy()
    {
        PlayerState.HealthChanged -= UpdateHealthBar;
    }
}