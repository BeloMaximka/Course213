using UnityEngine;

public class FirstAidScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger("OnCollected");
    }
}
