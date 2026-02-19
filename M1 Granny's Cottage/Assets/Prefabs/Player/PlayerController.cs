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
    [SerializeField] private bool isJumping = false;
    private GroundPosition _groundPosition;
    public bool isGrounded = true;

    [Header("Hover Settings")] // HOVER (time-limited hover / slam charge)
    [SerializeField] private float maxHoverTime = 2f; // HOVER (upgrade-modifiable hover duration)
    private float currentHoverTime; // HOVER (runtime hover timer)
    private bool isHovering; // HOVER (tracks active hover state)

    private float hoverLogTimer = 0f; // HOVER (throttles hover debug logging)
    private const float hoverLogInterval = 0.25f; // HOVER (log interval)

    [Header("Knockback Settings")] // KNOCKBACK (player hit reaction)
    [SerializeField] private float knockbackDistance = 3.5f; // KNOCKBACK (push distance)
    [SerializeField] private float knockbackDuration = 0.15f; // KNOCKBACK (push duration)
    private bool isKnockedBack = false; // KNOCKBACK (locks player control)
    private Coroutine knockbackRoutine; // KNOCKBACK (knockback coroutine handle)

    [Header("Ground Attack")] // GROUND ATTACK
    [SerializeField] private GroundAttack groundAttack; // GROUND ATTACK

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
        // this manages
        

        if (_playerCamera != null)
            _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();

        currentHoverTime = maxHoverTime; // HOVER (initialize hover timer)
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
        if (isKnockedBack) return; // KNOCKBACK (disable control during knockback)

        ApplyRotation();
        Jump();
        ApplyMovement();
        UpdateAnimation();

    }


    // Attack callback (Pure New Input System)
    private void OnAttack(InputAction.CallbackContext context)
    {
        // Prevent hammer attack while hovering or not grounded
        if (!isGrounded || isHovering)
        {
            Debug.Log("[COMBAT] Hammer attack blocked — player airborne or hovering");
            return;
        }

        if (hammerAttack != null)
        {
            hammerAttack.StartAttack();
            //audioSource.PlayOneShot(hammerSwingSound, volume);
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

    private void Jump()
    {
        // Jump pressed
        if (_playerInputActions.Player.Jump.WasPressedThisFrame())
        {
            isJumping = true;
            isHovering = true; // HOVER (start hover)

            groundAttack?.StartCharge(transform.position, maxHoverTime); // GROUND ATTACK

            
            audioSource.PlayOneShot(jumpingSound, volume);
        }

        // Jump released
        if (_playerInputActions.Player.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            isHovering = false; // HOVER (manual cancel)

            groundAttack?.StopCharge(); // GROUND ATTACK

            
        }

        // Hover logic with time limit
        if (isHovering && currentHoverTime > 0f) // HOVER
        {
            if (transform.position.y < 7f)
            {
                _verticalVelocity = -_gravity * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                _verticalVelocity = 0;
            }

            currentHoverTime -= Time.deltaTime; // HOVER

            groundAttack?.UpdateCharge(transform.position); // GROUND ATTACK

            // slowed logging
            hoverLogTimer += Time.deltaTime; // DEBUG
            if (hoverLogTimer >= hoverLogInterval) // DEBUG
            {
                //Debug.Log($"[HOVER] Active — Time left: {currentHoverTime:F2}");
                hoverLogTimer = 0f; // DEBUG
            }
        }
        else
        {
            if (isHovering)
            {
                Debug.Log("[HOVER] Hover time expired — auto-ending hover"); // DEBUG
            }

            isHovering = false; // HOVER
            groundAttack?.StopCharge(); // GROUND ATTACK
        }

        // Gravity when not hovering
        if (!isHovering)
        {
            if (transform.position.y > 1.8f)
            {
                ApplyGravity();
            }
            else
            {
                _verticalVelocity = 0;
                currentHoverTime = maxHoverTime; // HOVER
                hoverLogTimer = 0f; // HOVER

                groundAttack?.StopCharge(); // GROUND ATTACK (safety)
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

    // PLAYER KNOCKBACK
    public void TakeHit(Vector3 hitSourcePosition, float damage) // KNOCKBACK (called by enemy)
    {
        if (isKnockedBack)
        {
            //Debug.Log("[KNOCKBACK] Hit ignored — already in knockback"); // DEBUG
            return;
        }

        //Debug.Log("[KNOCKBACK] Player hit — knockback started"); // DEBUG

        Vector3 direction = transform.position - hitSourcePosition; // KNOCKBACK
        direction.y = 0f;
        direction.Normalize();

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine); // KNOCKBACK

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction)); // KNOCKBACK
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction) // KNOCKBACK
    {
        isKnockedBack = true; // KNOCKBACK

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction * knockbackDistance; // KNOCKBACK

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 nextPos = Vector3.Lerp(start, target, elapsed / knockbackDuration);
            _characterController.Move(nextPos - transform.position);
            yield return null;
        }

        isKnockedBack = false; // KNOCKBACK
        knockbackRoutine = null; // KNOCKBACK

        //Debug.Log("[KNOCKBACK] Knockback ended — control restored"); // DEBUG
    }
}
