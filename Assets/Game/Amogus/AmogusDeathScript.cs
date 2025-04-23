using UnityEngine;

public class AmogusDeathScript : MonoBehaviour
{

    [SerializeField]
    private GameObject enemy;

    public void OnDeath()
    {
        if(enemy != null)
        {
            Destroy(enemy);
        }
    }
}
