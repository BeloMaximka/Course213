using UnityEngine;

public class UICameraScript : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        Vector3 updatedRotation = transform.rotation.eulerAngles;
        updatedRotation.x = mainCamera.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(updatedRotation);
    }
}
