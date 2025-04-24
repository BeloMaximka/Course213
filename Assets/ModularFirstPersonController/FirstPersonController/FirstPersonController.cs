using UnityEngine;
using UnityEngine.UI;
using Assets.Game.Global;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    private CharacterController cc;
    private Vector3 velocity;
    private float gravity = -9.81f;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public float maxJumpSlopeAngle = 45f;

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f; // unused with CharacterController

    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    private void Start()
    {
        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
            crosshairObject.gameObject.SetActive(false);

        sprintBarCG = GetComponentInChildren<CanvasGroup>();
        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenW = Screen.width, screenH = Screen.height;
            sprintBarWidth = screenW * sprintBarWidthPercent;
            sprintBarHeight = screenH * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
                sprintBarCG.alpha = 0;
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleCamera();
        HandleZoom();
        HandleSprintTimers();
        HandleJumpInput();
        HandleCrouchInput();
        MoveCharacter();
        if (enableHeadBob) HeadBob();
    }

    private void HandleCamera()
    {
        if (PlayerState.IsPaused) return;

        yaw += Input.GetAxis("Mouse X") * GameSettings.MouseSensitivityX;
        float deltaY = Input.GetAxis("Mouse Y") * GameSettings.MouseSensitivityY;
        pitch += invertCamera ? deltaY : -deltaY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = Vector3.up * yaw;
        playerCamera.transform.localEulerAngles = Vector3.right * pitch;
    }

    private void HandleZoom()
    {
        if (!enableZoom || isSprinting) return;

        if (!holdToZoom && Input.GetKeyDown(zoomKey))
            isZoomed = !isZoomed;
        if (holdToZoom)
        {
            if (Input.GetKeyDown(zoomKey)) isZoomed = true;
            if (Input.GetKeyUp(zoomKey)) isZoomed = false;
        }

        float targetFOV = isZoomed ? zoomFOV : fov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomStepTime * Time.deltaTime);
    }

    private void HandleSprintTimers()
    {
        if (!enableSprint) return;

        if (isSprinting)
        {
            isZoomed = false;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

            if (!unlimitedSprint)
            {
                sprintRemaining -= Time.deltaTime;
                if (sprintRemaining <= 0f)
                {
                    isSprintCooldown = true;
                    isSprinting = false;
                }
            }
        }
        else
        {
            if (!unlimitedSprint)
                sprintRemaining = Mathf.Clamp(sprintRemaining + Time.deltaTime, 0, sprintDuration);
        }

        if (isSprintCooldown)
        {
            sprintCooldown -= Time.deltaTime;
            if (sprintCooldown <= 0f)
                isSprintCooldown = false;
        }
        else
            sprintCooldown = sprintCooldownReset;

        if (useSprintBar && !unlimitedSprint)
        {
            float pct = sprintRemaining / sprintDuration;
            sprintBar.transform.localScale = new Vector3(pct, 1f, 1f);

            if (hideBarWhenFull)
                sprintBarCG.alpha = Mathf.Lerp(sprintBarCG.alpha, sprintRemaining >= sprintDuration ? 0 : 1, Time.deltaTime * 5f);
        }
    }

    private void HandleJumpInput()
    {
        if (enableJump && Input.GetKeyDown(jumpKey) && cc.isGrounded)
        {
            if (IsOnTooSteepSlope())
                return;

            velocity.y = jumpPower;
            if (isCrouched && !holdToCrouch)
                Crouch();
        }
    }

    private void HandleCrouchInput()
    {
        if (!enableCrouch) return;

        if (!holdToCrouch && Input.GetKeyDown(crouchKey))
            Crouch();
        if (holdToCrouch)
        {
            if (Input.GetKeyDown(crouchKey)) Crouch();
            if (Input.GetKeyUp(crouchKey)) Crouch();
        }
    }

    private void MoveCharacter()
    {
        if (!playerCanMove) return;

        // Determine input direction
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        bool hasInput = inputDir.sqrMagnitude > 0.01f;

        // Sprint logic
        float currentSpeed = walkSpeed;
        bool wantSprint = enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown;
        if (wantSprint)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                if (isCrouched) Crouch();
            }
            currentSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
        }

        // Walking state for head bob
        isWalking = hasInput && cc.isGrounded;

        // Horizontal move
        Vector3 move = transform.TransformDirection(inputDir.normalized) * currentSpeed;

        // Vertical (gravity + jump)
        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = -2f;  // keep controller grounded
        velocity.y += gravity * Time.deltaTime;

        // Combine and move
        Vector3 disp = move + Vector3.up * velocity.y;
        cc.Move(disp * Time.deltaTime);
    }

    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = originalScale;
            walkSpeed /= speedReduction;
            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;
            isCrouched = true;
        }
    }

    private bool IsOnTooSteepSlope()
    {
        float rayDistance = cc.height + 0.1f;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            float groundAngle = Vector3.Angle(hit.normal, Vector3.up);
            return groundAngle > maxJumpSlopeAngle;
        }
        return true;
    }


    private void HeadBob()
    {
        if (isWalking)
        {
            float speedFactor = bobSpeed;
            if (isSprinting) speedFactor += sprintSpeed;
            else if (isCrouched) speedFactor *= speedReduction;

            timer += Time.deltaTime * speedFactor;
            joint.localPosition = new Vector3(
                jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x,
                jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y,
                jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z
            );
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed);
        }
    }
}
