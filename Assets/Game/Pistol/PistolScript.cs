using Assets.Game.Global;
using Assets.Game.Pistol;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    GameObject camera;
    LayerMask enemyMask;
    Animator animator;
    public AudioSource shotSound;
    public float damage = 50;

    public PistolState State
    {
        set
        {
            animator.SetInteger("State", (int)value);
        }
        get => (PistolState)animator.GetInteger("State");
    }

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        enemyMask = LayerMask.GetMask("Enemy");
        animator = gameObject.GetComponent<Animator>();

        GameSettings.DifficultyChanged += UpdateDifficultyValues;
        UpdateDifficultyValues(GameSettings.Difficulty);
    }

    void UpdateDifficultyValues(DifficultyType difficulty)
    {
        damage = difficulty switch
        {
            DifficultyType.Easy => 100,
            _ => (float)50,
        };
    }

    void Update()
    {
        Vector3 updatedRotation = transform.rotation.eulerAngles;
        updatedRotation.x = camera.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(updatedRotation);

        if (!PlayerState.IsPaused && State == PistolState.Idle && Input.GetMouseButtonDown(0))
        {
            State = PistolState.Firing;

            shotSound.pitch = 0.75f + Random.value * 0.5f;
            shotSound.volume = GameSettings.EffectsVolume * 0.1f;
            shotSound.Play();

            Debug.DrawLine(camera.transform.position, camera.transform.position + transform.rotation * Vector3.forward * 1000f, Color.red, 1);
            if (Physics.Raycast(camera.transform.position, transform.rotation * Vector3.forward, out RaycastHit hit, 1000f, enemyMask))
            {
                hit.transform.gameObject.SendMessage("ApplyDamage", damage);
            }
        }
    }

    private void OnDestroy()
    {
        GameSettings.DifficultyChanged -= UpdateDifficultyValues;
    }
}
