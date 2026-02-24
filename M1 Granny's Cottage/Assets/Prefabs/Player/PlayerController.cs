using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float turnSpeed = 1080f;

    [Header("Gravity")]
    private float _gravity = -9.81f;
    [SerializeField] private float _gravityMultiplier = 36.0f;
    [SerializeField] private float _verticalVelocity;

    private InputSystem_Actions _playerInputActions;
    private Vector3 _moveInput;
    private CharacterController _characterController;

    [Header("Camera")]
    [SerializeField] public CinemachineCamera _playerCamera;
    [SerializeField] private PlayerCameraScript _playerCameraScript;

    [Header("Combat")]
    [SerializeField] public HammerAttack hammerAttack;

    [Header("Player Status")]
    private GroundPosition _groundPosition;
    public bool isGrounded = true;

    [Header("Jump Settings")] // Jump (time-limited hover / slam charge)
    [SerializeField] private float maxJumpTime = 2f; // JUMP (upgrade-modifiable jump duration)
    private float currentJumpTime; // JUMP (runtime jump timer)

    private enum JumpState
    {
        READY_TO_JUMP,
        ASCENDING,
        HOVERING,
        FALLING,
        JUMP_ON_COOLDOWWN
    }
    private JumpState _currentJumpState = JumpState.READY_TO_JUMP;
    private bool isJumpAscending = false; // JUMP (tracks jump ascending state)
    private bool isJumpHovering = false; // JUMP (tracks jump hovering state)
    private bool isJumpFalling = false; // JUMP (tracks jump falling state)

    private Coroutine _jumpAbilityRiseCoroutine;
    private Coroutine _jumpAbilityHoverCoroutine;
    private Coroutine _jumpAbilityFallSlamCoroutine;

    private float hoverLogTimer = 0f; // HOVER (throttles hover debug logging)
    private const float hoverLogInterval = 0.25f; // HOVER (log interval)

    [Header("Knockback Settings")] // KNOCKBACK (player hit reaction)
    [SerializeField] private float knockbackDistance = 3.5f; // KNOCKBACK (push distance)
    [SerializeField] private float knockbackDuration = 0.15f; // KNOCKBACK (push duration)
    private bool isKnockedBack = false; // KNOCKBACK (locks player control)
    private Coroutine knockbackRoutine; // KNOCKBACK (knockback coroutine handle)

    [Header("Ground Attack")] // GROUND ATTACK
    [SerializeField] private GroundAttack groundAttack; // GROUND ATTACK

    //NEW
    [Header("Ground Attack Cooldown")] //NEW
    [SerializeField] private float slamCooldownDuration = 7f; //NEW (adjustable in Inspector)
    private float slamCooldownTimer = 0f; //NEW
    private bool slamOnCooldown = false; //NEW
    private bool slamStarted = false; //NEW (tracks if a slam was actually initiated)

    //NEW
    [SerializeField] private bool showSlamCooldownDebug = true; //NEW (toggle in Inspector)
    private float slamLogTimer = 0f; //NEW (throttle timer)
    private const float slamLogInterval = 1f; //NEW (log once per second)

    [Header("Player Action Sounds")]
    public AudioSource audioSource;
    public AudioClip jumpingSound;
    public AudioClip hammerSwingSound;
    public float volume = 0.5f;

    [Header("Animation")]
    [SerializeField] public Animator animator;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _playerInputActions.UI.Disable();
        _characterController = GetComponent<CharacterController>();
        _groundPosition = GetComponent<GroundPosition>();
    }

    private void Start()
    {
        if (_playerCamera != null)
            _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();

        currentJumpTime = maxJumpTime; // HOVER (initialize hover timer)
        //Debug.Log($"[HOVER] Initialized hover time: {currentHoverTime}"); // DEBUG
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();

        // attack input hookup
        _playerInputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Attack.performed -= OnAttack;
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {
        isGrounded = _groundPosition.groundedState;
        //Debug.Log(isGrounded ? "GROUNDED" : "NOT GROUNDED");

        JumpSlamCooldownManager();

        if (isKnockedBack) return; // KNOCKBACK (disable control during knockback)

        ApplyRotation();
        //Jump();
        ApplyMovement();
        UpdateAnimation();
    }

    // Attack callback (Pure New Input System)
    private void OnAttack(InputAction.CallbackContext context)
    {
        // Prevent hammer attack while hovering or not grounded
        if (!isGrounded || isJumpHovering)
        {
            Debug.Log("[COMBAT] Hammer attack blocked — player airborne or hovering");
            return;
        }

        if (hammerAttack != null)
        {
            hammerAttack.StartAttack();
        // Prevent hammer attack while hovering or not grounded
        }
    }

    // Turns the player towards the horizontal direction they are moving in.
    private void ApplyRotation()
    {
        if (_moveInput == Vector3.zero) return;

        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 lookInput = isometricMatrix.MultiplyPoint3x4(_moveInput);

        Quaternion rotation = Quaternion.LookRotation(lookInput, Vector3.up);

        // IMPORTANT: This line locks the playerParent rotation to the horizontal plane only. 
        rotation.Set(0.0f, rotation.y, 0.0f, rotation.w);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            turnSpeed * Time.deltaTime
        );
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        Vector2 moveInput = callbackContext.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, _verticalVelocity, moveInput.y);
    }

    private void ApplyMovement()
    {
        // These two lines prepare the jump velocity to be added to the player's movement velocity
        Vector3 jumpVelocity = new Vector3(0.0f, _verticalVelocity, 0.0f);
        jumpVelocity.Normalize();

        Vector3 moveDirection =
            transform.forward * runSpeed * _moveInput.magnitude * Time.deltaTime;

        // this line adds the normalized jump velocity to the player's movement
        moveDirection.y = jumpVelocity.y;
        _characterController.Move(moveDirection);
    }

    private void ApplyGravity()
    {
        _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
    }

    // KNOCKBACK (restored only — no changes to your system)
    public void TakeHit(Vector3 hitSourcePosition, float damage)
    {
        if (isKnockedBack)
            return;

        Vector3 direction = transform.position - hitSourcePosition;
        direction.y = 0f;
        direction.Normalize();

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction)
    {
        isKnockedBack = true;

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction * knockbackDistance;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 nextPos = Vector3.Lerp(start, target, elapsed / knockbackDuration);
            _characterController.Move(nextPos - transform.position);
            yield return null;
        }

        isKnockedBack = false;
        knockbackRoutine = null;
    }

    public void OnJumpInputEvent(InputAction.CallbackContext callbackContext)
    {

        if (callbackContext.action.WasPressedThisFrame() && JumpingPermitted())
        {
            _jumpAbilityRiseCoroutine = StartCoroutine(JumpAbilityRise());
        }


        if (callbackContext.action.WasReleasedThisFrame() && !isGrounded && !isJumpFalling && !JumpingPermitted())
        {        
            _jumpAbilityFallSlamCoroutine = StartCoroutine(JumpAbilityFallSlam());
        }

    }

    // This function checks whether the player can start the jump ability
    private bool JumpingPermitted()
    {
        bool canPlayerStartJumping;
        canPlayerStartJumping = 
        (isGrounded && 
        !slamOnCooldown &&
        _jumpAbilityRiseCoroutine == null &&
        _jumpAbilityHoverCoroutine == null &&
        _jumpAbilityFallSlamCoroutine == null) ? true : false;
        
        return canPlayerStartJumping;
    }

    //private bool 
    
    // this coroutine handles the start of the jump and rising jump state
    private IEnumerator JumpAbilityRise()
    {
        float jumpRiseDuration = 0.5f;
        Debug.Log("Jump Rise Coroutine begun.");
        //Debug.Break();
        //_currentJumpState = JumpState.ASCENDING;
        isJumpAscending = true;
        slamStarted = true; //NEW

        groundAttack?.StartCharge(transform.position, maxJumpTime); // GROUND ATTACK
        audioSource.PlayOneShot(jumpingSound, volume);


        //while (transform.position.y <= (_groundPosition.GroundPointTransform.position.y + 5.0f))
        {
            
            Mathf.SmoothDamp(transform.position.y,  (_groundPosition.GroundPointTransform.position.y + 5.0f), ref _verticalVelocity, jumpRiseDuration, runSpeed);
            
            currentJumpTime -= Time.deltaTime; // HOVER
            groundAttack?.UpdateCharge(transform.position); // GROUND ATTACK
            yield return new WaitForSeconds(jumpRiseDuration);// + 0.1f);
        }
        _verticalVelocity = 0.0f;
        //_currentJumpState = JumpState.HOVERING;
        isJumpAscending = false;
        // starts the floating coroutine
        Debug.Log("Jump Rise Coroutine finished.");
        _jumpAbilityRiseCoroutine = null;
        _jumpAbilityHoverCoroutine = StartCoroutine(JumpAbilityHover());
        yield return _jumpAbilityHoverCoroutine;
        

    }

    // this coroutine handles the hovering jump state
    private IEnumerator JumpAbilityHover()
    {
        Debug.Log("Jump Hover Coroutine begun.");
        isJumpHovering = true;
        while (currentJumpTime > 0.0f)
        {
            currentJumpTime -= Time.deltaTime; // HOVER
            groundAttack?.UpdateCharge(transform.position); // GROUND ATTACK
            yield return null;
        }

        Debug.Log("Jump Hover Coroutine finished.");
        _jumpAbilityHoverCoroutine = null;
        _jumpAbilityFallSlamCoroutine = StartCoroutine(JumpAbilityFallSlam());
        yield return _jumpAbilityFallSlamCoroutine;
        

    }

    // This coroutine handles the fall and slam state of the jump ability.
    private IEnumerator JumpAbilityFallSlam()
    {
        float jumpFallDuration = 0.3f;
        Debug.Log("Jump Fall Slam Coroutine begun.");
        // this code block ensures the fall state occurs cleanly
        {
            if (_jumpAbilityRiseCoroutine != null) StopCoroutine(_jumpAbilityRiseCoroutine);
            if (_jumpAbilityHoverCoroutine != null) StopCoroutine(_jumpAbilityHoverCoroutine);
            isJumpAscending = false;
            isJumpHovering = false;
            groundAttack?.StopCharge(); // GROUND ATTACK
        }

        isJumpFalling = true;

        //while (!isGrounded)
        {
            Mathf.SmoothDamp(transform.position.y, _groundPosition.GroundPointTransform.position.y, ref _verticalVelocity, jumpFallDuration);
            yield return new WaitForSeconds(jumpFallDuration);
        }

        _verticalVelocity = 0.0f;
        isJumpFalling = false;
        currentJumpTime = maxJumpTime; // HOVER
        //NEW
        if (slamStarted && !slamOnCooldown) //NEW
        {
            slamOnCooldown = true; //NEW
            slamCooldownTimer = slamCooldownDuration; //NEW
            slamStarted = false; //NEW
            slamLogTimer = 0f; //NEW

            /* debug
            if (showSlamCooldownDebug) //NEW
                Debug.Log($"[SLAM] Cooldown started ({slamCooldownDuration:F1}s)"); //NEW
            */
        }

        Debug.Log("Jump Fall Slam Coroutine Finished.");
        StopCoroutine(_jumpAbilityFallSlamCoroutine);
        _jumpAbilityFallSlamCoroutine = null;
        yield break;

        
    }

    // This function manages the cooldown for the jump slam
    private void JumpSlamCooldownManager()
    {
        //NEW
        if (slamOnCooldown) //NEW
        {
            slamCooldownTimer -= Time.deltaTime; //NEW

            //NEW
            if (showSlamCooldownDebug) //NEW
            {
                slamLogTimer += Time.deltaTime; //NEW
                
                if (slamLogTimer >= slamLogInterval) //NEW
                {
                    Debug.Log($"[SLAM] Cooldown remaining: {slamCooldownTimer:F1}s"); //NEW
                    slamLogTimer = 0f; //NEW
                }
            }

            if (slamCooldownTimer <= 0f) //NEW
            {
                slamOnCooldown = false; //NEW
                slamCooldownTimer = 0f; //NEW
                slamLogTimer = 0f; //NEW

                
                //NEW
                if (showSlamCooldownDebug)
                {
                    //NEW
                    Debug.Log("[SLAM] Cooldown finished"); //NEW
                } 
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        Vector3 horizontalMove = new Vector3(_moveInput.x, 0f, _moveInput.z);
        float speed = horizontalMove.magnitude;

        animator.SetFloat("Speed", speed);
    }

    //NEW POWERUP METHODS

    public void ApplySpeedBoost() //NEW
    {
        runSpeed += 3f;
    }

    public void ApplyDashUpgrade() //NEW
    {
        // placeholder for dash implementation
    }

    public void ApplyExtraHealth() //NEW
    {
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
            health.IncreaseMaxHealth(25f);
    }

    public void ApplyGroundSmashUpgrade() //NEW
    {
        slamCooldownDuration *= 0.5f;
    }

    public void ApplyQuickSwing() //NEW
    {
        if (hammerAttack != null)
            hammerAttack.ReduceCooldown(0.5f);
    }
}