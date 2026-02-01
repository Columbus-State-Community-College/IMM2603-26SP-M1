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
    [SerializeField] private HammerAttack hammerAttack; // NEW

    [Header("Player Status")]
    public float playerHealth = 100f;
    [SerializeField] private bool isJumping = false;

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
        Move();
        Jump();
    }

    private void GatherInput()
    {
        Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, _verticalVelocity, moveInput.y);
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
            transform.forward * runSpeed * _moveInput.magnitude * Time.deltaTime;

        _characterController.Move(moveDirection);
    }

    private void ApplyGravity()
    {
        _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
    }

    private void Jump()
    {
        if (_playerInputActions.Player.Jump.WasPressedThisFrame())
        {
            isJumping = true;
            _playerCameraScript?.EnableJumpCamera(true);
        }

        if (_playerInputActions.Player.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            _playerCameraScript?.EnableJumpCamera(false);
        }

        if (isJumping)
        {
            if (transform.position.y < 7)
                _verticalVelocity = -_gravity * _gravityMultiplier * Time.deltaTime;
            else
                _verticalVelocity = 0;
        }
        else
        {
            if (transform.position.y > 1.8f)
                ApplyGravity();
            else
                _verticalVelocity = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
    }
}
