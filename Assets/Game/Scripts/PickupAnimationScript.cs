using UnityEngine;

public class PickupAnimationScript : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToDestroy;

    private void OnAnimationEnd()
    {
        Destroy(objectToDestroy);
    }
}
