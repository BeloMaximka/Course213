using Assets.Game.Global;
using UnityEngine;

public class AmogusDeathScript : MonoBehaviour
{

    [SerializeField]
    private GameObject enemy;

    public void OnDeath()
    {
        if(enemy != null)
        {
            GameEntities.Destroy(EntityType.Enemy, enemy);
        }
    }
}
