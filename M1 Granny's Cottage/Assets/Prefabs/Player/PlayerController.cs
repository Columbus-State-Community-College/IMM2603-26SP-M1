using UnityEngine;
using System;
using Unity.Cinemachine;
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

    [SerializeField] public CinemachineCamera _playerCamera;
    [SerializeField] private PlayerCameraScript _playerCameraScript;
    

    [Header("Player Status")]
    public float playerHealth = 100f;
    [SerializeField] private bool isJumping = false;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();
        
    }

    // for initialization based on info from other scripts
    private void Start()
    {
        _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();
    }

    private void OnEnable()
    {
        // enables Player action map in Input System
        _playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        // disables Player action map in Input System
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {
        GatherInput();

        Look();
        Move();
        Jump();
    }

    // causes the player to turn towards and face the direction _moveInput indicates
    private void Look()
    {
        // if no input, do nothing
        if (_moveInput == Vector3.zero) return;

        // This code block corrects our inputs to the isometric camera angle
        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0,45,0));
        
        Vector3 lookInput = _moveInput; 
        Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(lookInput);
        
        // turns player towards input direction
        Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        
    }

    private void GatherInput()
    {
        // gathers movement input
        Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, _verticalVelocity, moveInput.y);    

    }

   // Moves the player in the direction _moveInput indicates
    private void Move()
    {
        Vector3 moveDirection = transform.forward * runSpeed * _moveInput.magnitude * Time.deltaTime;
        _characterController.Move(moveDirection);
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
    }

    private void ApplyGravity()
    {
        _verticalVelocity = _gravity * _gravityMultiplier * Time.deltaTime;
    }

    private void Jump()
    {
        // gathers jump input, edits camera deadzone while midair
        if (_playerInputActions.Player.Jump.WasPressedThisFrame())
        {
            isJumping = true;
            _playerCameraScript.EnableJumpCamera(isJumping);
        }
        if (_playerInputActions.Player.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            _playerCameraScript.EnableJumpCamera(isJumping);
        }

        if (isJumping)
        {
            // if below a certain height, allow to jump up
            if (transform.position.y < 7)
            {
                _verticalVelocity = -_gravity * _gravityMultiplier * Time.deltaTime;
            }
            // if at or above certain height while jumping, stay there
            else if (transform.position.y >= 7)
            {
                _verticalVelocity = 0;
            }
        }
        // if not jumping, fall unless on the ground
        else
        {
            if (transform.position.y > 1.8)
            {
                ApplyGravity();
            }
            else
            {
                _verticalVelocity = 0;
            }
        }  
    }
}
