using Assets.Game.Pistol;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    GameObject camera;
    LayerMask enemyMask;
    Animator animator;

    private PistolState state;
    public PistolState State
    {
        set
        {
            state = value;
            animator.SetInteger("State", (int)value);
        }
        get => state;
    }

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        enemyMask = LayerMask.GetMask("Enemy");
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 updatedRotation = transform.rotation.eulerAngles;
        updatedRotation.x = camera.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(updatedRotation);

        if(State == PistolState.Idle && Input.GetMouseButtonDown(0))
        {
            State = PistolState.Firing;
            if (Physics.Raycast(camera.transform.position, transform.rotation * Vector3.forward, out RaycastHit hit, 1000f, enemyMask))
            {
                hit.transform.gameObject.SendMessage("ApplyDamage", 50);
            }
        }
    }
}
