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


    public AudioSource deathSound;

    public float spreadFactor = 1f;
    private int damage = 5;

    public bool IsDead
    {
        set
        {
            isDead = value;
            deathSound.Play();
            animator.SetTrigger("Death");
        }
        get => isDead;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        modelWrap = transform.Find("ModelWrap");
        deathSound.volume = GameSettings.EffectsVolume;
        GameSettings.EffectsVolumeChanged += OnVolumeChange;
        GameSettings.DifficultyChanged += UpdateDifficultyValues;
        UpdateDifficultyValues(GameSettings.Difficulty);
    }

    void Update()
    {
        if (IsDead || Mathf.Approximately(Time.timeScale, 0f))
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

    void UpdateDifficultyValues(DifficultyType difficulty)
    {
        switch (difficulty)
        {
            case DifficultyType.Easy:
                walkSpeed = 6;
                damage = 5;
                break;
            case DifficultyType.Medium:
                walkSpeed = 11;
                damage = 10;
                break;
            case DifficultyType.Hard:
                walkSpeed = 16;
                damage = 15;
                break;
        }
    }

    private void OnVolumeChange(float volume)
    {
        deathSound.volume = volume;
    }

    private void OnDestroy()
    {
        GameSettings.EffectsVolumeChanged -= OnVolumeChange;
        GameSettings.DifficultyChanged -= UpdateDifficultyValues;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DamagePlayer(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        DamagePlayer(collision);
    }

    private void DamagePlayer(Collision collision)
    {
        if (IsDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerState.RecievedDamage = damage;
        }
    }

    private Vector3 DirectionWithSpreadOutBehaviour()
    {
        Vector3 direction = player.transform.position - transform.position;
        if(Vector3.Distance(player.transform.position, transform.position) < 20f)
        {
            return direction.normalized;
        }

        foreach (var entity in GameEntities.GetCollection(EntityType.Enemy))
        {
            if (entity.MainObject == gameObject)
            {
                continue;
            }

            float dst = Vector3.Distance(transform.position, entity.MainObject.transform.position);
            // Further - lesser magnitude
            float distanceFactor = spreadFactor / (dst * dst);
            direction += (transform.position - entity.MainObject.transform.position) * distanceFactor;
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
