using Assets.Game.Global;
using UnityEngine;

public class PickupAnimationScript : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToDestroy;

    public EntityType entityType;

    private void OnAnimationEnd()
    {
        GameEntities.Destroy(entityType, objectToDestroy);
    }
}
