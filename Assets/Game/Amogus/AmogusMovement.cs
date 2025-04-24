using Assets.Game.Amogus;
using Assets.Game.Global;
using UnityEngine;

public class AmogusMovement : MonoBehaviour
{
    public bool playerCanMove = true;
    public float walkSpeed = 3f;
    public float maxVelocityChange = 10f;
    private bool isGrounded = false;
    private bool isDead = false;
    private int health = 100;

    private Rigidbody rb;
    private GameObject player;
    private Transform modelWrap;

    [SerializeField]
    private Animator animator;


    public bool IsDead
    {
        set
        {
            isDead = value;
            animator.SetTrigger("Death");

        }
        get => isDead;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        modelWrap = transform.Find("ModelWrap");
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        CheckGround();

        if (isGrounded && Random.Range(1, 240) == 1 )
        {
            rb.AddForce(0f, 5f, 0f, ForceMode.Impulse);
            isGrounded = false;
            UpdateMoveState(AmogusMoveStates.Jumping);
        }
    }

    void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }

        Vector3 targetVelocity = DirectionWithSpreadOutBehaviour();
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0;
        modelWrap.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, -90, 0);

        if (isGrounded)
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

    public void ApplyDamage(int damage)
    {
        if (IsDead) return;

        UpdateMoveState(AmogusMoveStates.Damaged);
        health -= damage;
        if (health <= 0)
        {
            IsDead = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsDead) return;

        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerState.RecievedDamage = 5;
        }
    }

    private Vector3 DirectionWithSpreadOutBehaviour()
    {
        Vector3 direction = player.transform.position - transform.position;
        if(Vector3.Distance(player.transform.position, transform.position) < 15f)
        {
            return direction.normalized;
        }

        foreach (var entity in GameEntities.GetCollection(EntityType.Enemy))
        {
            if (entity == gameObject)
            {
                continue;
            }

            // Further - lesser magnitude
            float distanceFactor = 3f / Vector3.Distance(transform.position, entity.transform.position);
            direction += (transform.position - entity.transform.position) * distanceFactor;
        }
        Debug.DrawLine(transform.position, transform.position + direction.normalized * 2f, Color.magenta);
        return direction.normalized;
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
        }
    }

    private void UpdateMoveState(AmogusMoveStates state)
    {
        animator.SetInteger("MoveState", (int)state);
    }
}
