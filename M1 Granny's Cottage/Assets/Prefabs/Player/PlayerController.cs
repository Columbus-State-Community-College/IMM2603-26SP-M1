using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System;

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

    [Header("Hover Settings")] // NEW
    [SerializeField] private float maxHoverTime = 2f; // NEW (upgrade modifies this)
    private float currentHoverTime; // NEW
    private bool isHovering; // NEW

    private float hoverLogTimer = 0f; // DEBUG
    private const float hoverLogInterval = 0.25f; // DEBUG

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

        currentHoverTime = maxHoverTime; // NEW
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
            isHovering = true; // NEW

            Debug.Log("[HOVER] Jump pressed — hover started"); // DEBUG

            _playerCameraScript?.EnableJumpCamera(true);
            audioSource.PlayOneShot(jumpingSound, volume);
        }

        // Jump released
        if (_playerInputActions.Player.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            isHovering = false; // NEW

            Debug.Log("[HOVER] Jump released — hover manually ended"); // DEBUG

            _playerCameraScript?.EnableJumpCamera(false);
        }

        // Hover logic with time limit
        if (isHovering && currentHoverTime > 0f) // NEW
        {
            // Maintain hover height but DO NOT pause timer
            if (transform.position.y < 7f)
            {
                _verticalVelocity = -_gravity * _gravityMultiplier * Time.deltaTime;
            }
            else
            {
                _verticalVelocity = 0;
            }

            // ALWAYS drain hover time while hovering
            currentHoverTime -= Time.deltaTime; // NEW

            // Throttled logging
            hoverLogTimer += Time.deltaTime; // DEBUG
            if (hoverLogTimer >= hoverLogInterval) // DEBUG
            {
                //Debug.Log($"[HOVER] Active — Time left: {currentHoverTime:F2} | Y: {transform.position.y:F2}");
                hoverLogTimer = 0f; // DEBUG
            }
        }
        else
        {
            if (isHovering)
            {
                Debug.Log("[HOVER] Hover time expired — auto-ending hover"); // DEBUG
            }

            isHovering = false; // NEW
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
                currentHoverTime = maxHoverTime; // NEW
                hoverLogTimer = 0f; // DEBUG
            }
        }
    }
}
