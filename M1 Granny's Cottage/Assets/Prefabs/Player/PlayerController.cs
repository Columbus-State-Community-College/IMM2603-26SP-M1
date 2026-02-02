using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem; // NEW
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float turnSpeed = 1080f;

    [Header("Gravity")]
    private float _gravity = -30f; // NEW was -9.81f
    [SerializeField] private float _gravityMultiplier = 1.0f; // NEW was 36.0f
    [SerializeField] private float _verticalVelocity;

    private InputSystem_Actions _playerInputActions;
    private Vector3 _moveInput;
    private CharacterController _characterController;

    [Header("Camera")]
    [SerializeField] public CinemachineCamera _playerCamera;
    [SerializeField] private PlayerCameraScript _playerCameraScript;

    [Header("Combat")]
    [SerializeField] public HammerAttack hammerAttack; // NEW

    [Header("Player Status")]
    public float playerHealth = 100f;
    [SerializeField] private bool isJumping = false;

    [Header("Jumping")] // NEW
    [SerializeField] private float jumpHeight = 5f;// NEW
    private bool isGrounded;// NEW
    private bool lastGroundedState;// NEW
    private bool jumpRequested; // NEW

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (_playerCamera != null)
            _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();

        // NEW: attack input hookup
        _playerInputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Attack.performed -= OnAttack; // NEW
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {
        GatherInput();

        Look();
        Jump();          // NEW moved before gravity
        ApplyGravity();  // NEW
        Move();

        isGrounded = _characterController.isGrounded; // NEW moved AFTER Move
        jumpRequested = false; // NEW clear jump protection AFTER Move

        // NEW slow down for debug logging
        if (isGrounded != lastGroundedState)
        {
            Debug.Log($"Grounded changed → {isGrounded}");
            lastGroundedState = isGrounded;
        }
    }

    private void GatherInput()
    {
        Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, 0f, moveInput.y); // NEW change to _verticalVelocity to 0f
    }

    // NEW Attack callback (Pure New Input System)
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (hammerAttack != null)
        {
            hammerAttack.StartAttack();
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
            (transform.forward * runSpeed * _moveInput.magnitude)
            + Vector3.up * _verticalVelocity; // NEW change Time.deltaTime to + Vector3.up * _verticalVelocity

        _characterController.Move(moveDirection * Time.deltaTime); // NEW add * Time.deltaTime
    }

    private void ApplyGravity()
    {
        if (isGrounded && _verticalVelocity < 0f && !jumpRequested) // NEW protect jump frame
        {
            _verticalVelocity = -5f; // NEW
        }
        else
        {
            _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime; // NEW change = to +=
        }
    }

    private void Jump()
    {
        if (_playerInputActions.Player.Jump.WasPressedThisFrame() && isGrounded) // NEW add  && isGrounded
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * _gravity); // NEW
            jumpRequested = true; // NEW
            isJumping = true;
            _playerCameraScript?.EnableJumpCamera(true);
        }

        if (isGrounded && isJumping) // NEW
        {
            isJumping = false;
            _playerCameraScript?.EnableJumpCamera(false);
        }
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
    }
}
