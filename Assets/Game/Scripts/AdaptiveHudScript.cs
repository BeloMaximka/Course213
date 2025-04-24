using UnityEngine;

public class AdaptiveHudScript : MonoBehaviour
{
    public Camera targetCamera;

    private float initialHeight;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialHeight = rectTransform.rect.height;
    }

    void LateUpdate()
    {
        rectTransform.sizeDelta = new Vector2(initialHeight * targetCamera.aspect, initialHeight);
    }
}