using Assets.Game.Global;
using UnityEngine;

public class FirstAidScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private bool isCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected || PlayerState.Health >= PlayerState.MaxHealth) return;

        animator.SetTrigger("OnCollected");
        isCollected = true;
        PlayerState.Health += 25;
    }
}
