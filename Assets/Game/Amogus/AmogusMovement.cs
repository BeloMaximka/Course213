using Assets.Game.Amogus;
using UnityEngine;

public class AmogusMovement : MonoBehaviour
{
    public bool playerCanMove = true;
    public float walkSpeed = 3f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isGrounded = false;

    private Rigidbody rb;

    [SerializeField]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGround();

        if (isGrounded && Input.GetKeyDown(KeyCode.Space) )
        {
            rb.AddForce(0f, 5f, 0f, ForceMode.Impulse);
            isGrounded = false;
            UpdateMoveState(AmogusMoveStates.Jumping);
        }
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(isGrounded)
        {
            if (targetVelocity.x != 0 || targetVelocity.z != 0)
            {
                UpdateMoveState(AmogusMoveStates.Walking);
            }
            else
            {
                UpdateMoveState(AmogusMoveStates.Idle);
            }
        }

        targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void CheckGround()
    {
        Vector3 origin = new(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit _, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            UpdateMoveState(AmogusMoveStates.Jumping);
        }
    }

    private void UpdateMoveState(AmogusMoveStates state)
    {
        animator.SetInteger("MoveState", (int)state);
    }
}
