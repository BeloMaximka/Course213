using UnityEngine;

public class PistolScript : MonoBehaviour
{
    GameObject camera;
    LayerMask enemyMask;
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        enemyMask = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        Vector3 updatedRotation = transform.rotation.eulerAngles;
        updatedRotation.x = camera.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(updatedRotation);

        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(camera.transform.position, transform.rotation * Vector3.forward, out RaycastHit hit, 1000f, enemyMask))
            {
                hit.transform.gameObject.SendMessage("ApplyDamage", 50);
            }
        }
    }
}
