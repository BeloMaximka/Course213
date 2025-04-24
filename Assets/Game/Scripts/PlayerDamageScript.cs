using Assets.Game.Global;
using UnityEngine;

public class PlayerDamageScript : MonoBehaviour
{
    [Tooltip("Damage interval in seconds")]
    public float damageInterval;

    void Start()
    {
        InvokeRepeating(nameof(HandleDamageDealt), 0f, damageInterval);
    }

    #pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    void HandleDamageDealt()
    #pragma warning restore S2325 // Used in InvokeRepeating
    {
        if (PlayerState.RecievedDamage == 0)
        {
            return;
        }

        PlayerState.Health -= PlayerState.RecievedDamage;
        PlayerState.ResetRecievedDamage();
    }
}
