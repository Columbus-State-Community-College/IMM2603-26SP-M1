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

    [Header("Hover Settings")] // NEW – HOVER (time-limited hover / slam charge)
    [SerializeField] private float maxHoverTime = 2f; // NEW – HOVER (upgrade-modifiable hover duration)
    private float currentHoverTime; // NEW – HOVER (runtime hover timer)
    private bool isHovering; // NEW – HOVER (tracks active hover state)

    private float hoverLogTimer = 0f; // NEW – HOVER (throttles hover debug logging)
    private const float hoverLogInterval = 0.25f; // NEW – HOVER (log interval)

    [Header("Knockback Settings")] // NEW – KNOCKBACK (player hit reaction)
    [SerializeField] private float knockbackDistance = 3.5f; // NEW – KNOCKBACK (push distance)
    [SerializeField] private float knockbackDuration = 0.15f; // NEW – KNOCKBACK (push duration)
    private bool isKnockedBack = false; // NEW – KNOCKBACK (locks player control)
    private Coroutine knockbackRoutine; // NEW – KNOCKBACK (knockback coroutine handle)

    [Header("Player Action Sounds")]
    public AudioSource audioSource;
    public AudioClip jumpingSound;
    public AudioClip hammerSwingSound;
    public float volume = 0.5f;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (_playerCamera != null)
            _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();

        currentHoverTime = maxHoverTime; // NEW – HOVER (initialize hover timer)
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
        if (isKnockedBack) return; // NEW – KNOCKBACK (disable control during knockback)

        GatherInput();

        Look();
        Move();
        Jump();
    }

    private void GatherInput()
    {
        Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, _verticalVelocity, moveInput.y);
    }

    // Attack callback (Pure New Input System)
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (hammerAttack != null)
        {
            hammerAttack.StartAttack();
            audioSource.PlayOneShot(hammerSwingSound, volume);
        }
    }

    private void Look()
    {
        if (_moveInput == Vector3.zero) return;

        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 lookInput = isometricMatrix.MultiplyPoint3x4(_moveInput);

        Quaternion rotation = Quaternion.LookRotation(lookInput, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void Move()
    {
        Vector3 moveDirection =
            transform.forward * runSpeed * _moveInput.magnitude * Time.deltaTime;

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
            isHovering = true; // NEW – HOVER (start hover)

            Debug.Log("[HOVER] Jump pressed — hover started"); // DEBUG

            _playerCameraScript?.EnableJumpCamera(true);
            audioSource.PlayOneShot(jumpingSound, volume);
        }

        // Jump released
        if (_playerInputActions.Player.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            isHovering = false; // NEW – HOVER (manual cancel)

            Debug.Log("[HOVER] Jump released — hover manually ended"); // DEBUG

            _playerCameraScript?.EnableJumpCamera(false);
        }

        // Hover logic with time limit
        if (isHovering && currentHoverTime > 0f) // NEW – HOVER
        {
            if (transform.position.y < 7f)
            {
                _verticalVelocity = -_gravity * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                _verticalVelocity = 0;
            }

            currentHoverTime -= Time.deltaTime; // NEW – HOVER

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
            
            isHovering = false; // NEW – HOVER
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
                currentHoverTime = maxHoverTime; // NEW – HOVER
                hoverLogTimer = 0f; // NEW – HOVER
            }
        }
    }

    // PLAYER KNOCKBACK
    public void TakeHit(Vector3 hitSourcePosition, float damage) // NEW – KNOCKBACK (called by enemy)
    {
        if (isKnockedBack)
        {
            Debug.Log("[KNOCKBACK] Hit ignored — already in knockback"); // DEBUG
            return;
        }

        Debug.Log("[KNOCKBACK] Player hit — knockback started"); // DEBUG

        Vector3 direction = transform.position - hitSourcePosition; // NEW – KNOCKBACK
        direction.y = 0f;
        direction.Normalize();

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine); // NEW – KNOCKBACK

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction)); // NEW – KNOCKBACK
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction) // NEW – KNOCKBACK
    {
        isKnockedBack = true; // NEW – KNOCKBACK

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction * knockbackDistance; // NEW – KNOCKBACK

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 nextPos = Vector3.Lerp(start, target, elapsed / knockbackDuration);
            _characterController.Move(nextPos - transform.position);
            yield return null;
        }

        isKnockedBack = false; // NEW – KNOCKBACK
        knockbackRoutine = null; // NEW – KNOCKBACK

        Debug.Log("[KNOCKBACK] Knockback ended — control restored"); // DEBUG
    }
}
