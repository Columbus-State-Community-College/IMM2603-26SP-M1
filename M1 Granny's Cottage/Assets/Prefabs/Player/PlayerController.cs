using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour,  IDataPersistence
{
    [Header("Player Movement")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float _verticalVelocity;

    [Header("Player Status")]
    private GroundBelowPlayer _groundPosition;
    public bool isGrounded = true;
    private InputSystem_Actions _playerInputActions;
    private Vector3 _moveInput;
    private CharacterController _characterController;

    [Header("Jump Settings")] // Jump (time-limited hover / slam charge)
    [SerializeField] private float maxJumpTime = 2f; // JUMP (upgrade-modifiable jump duration)
    private float currentJumpTime; // JUMP (runtime jump timer)

    [Header("Invincibility Frames")]
    [SerializeField] private float slamInvincibilityDuration = 1f;
    private bool isInvincible = false;

    public enum JumpState
    {
        READY_TO_JUMP,
        ASCENT_START,
        ASCENDING,
        HOVERING,
        FALLING,
        SLAM,
        JUMP_ON_COOLDOWN
    }
    public JumpState _currentJumpState = JumpState.READY_TO_JUMP;

    private Coroutine _jumpAbilityRiseCoroutine;
    private Coroutine _jumpAbilityHoverCoroutine;
    private Coroutine _jumpAbilityFallSlamCoroutine;

    [Header("Camera")]
    [SerializeField] public CinemachineCamera _playerCamera;
    [SerializeField] private PlayerCameraScript _playerCameraScript;
    [SerializeField] private CinemachineImpulseSource _jumpImpulseSource;

    [Header("Combat")]
    [SerializeField] public HammerAttack hammerAttack;

    [Header("Knockback Settings")] // KNOCKBACK (player hit reaction)
    [SerializeField] private float knockbackDistance = 3.5f; // KNOCKBACK (push distance)
    [SerializeField] private float knockbackDuration = 0.15f; // KNOCKBACK (push duration)
    private bool isKnockedBack = false; // KNOCKBACK (locks player control)
    private Coroutine knockbackRoutine; // KNOCKBACK (knockback coroutine handle)

    [Header("Ground Attack")] // GROUND ATTACK
    [SerializeField] private GroundAttack groundAttack; // GROUND ATTACK
    public ParticleSystem groundAttackParticles;

    [Header("Ground Attack Cooldown")]
    [SerializeField] private float slamCooldownDuration = .5f; // (adjustable in Inspector)
    private float slamCooldownTimer = 0f;
    [SerializeField] private bool showSlamCooldownDebug = false; // (toggle in Inspector)
    private float slamLogTimer = 0f; // (throttle timer)
    private const float slamLogInterval = 1f; // (log once per second)

    // (Cooldown Feedback)
    [Header("Ground Attack Cooldown Feedback")]
    [SerializeField] private ParticleSystem slamCooldownParticles;
    [SerializeField] private JumpCooldownUI jumpCooldownUI;

    private ParticleSystem activeCooldownParticles;
    private bool slamCooldownFeedbackTriggered = false;

    [Header("Player Action Sounds")]
    public GrannysSounds grannySoundScript;

    [Header("Animation")]
    [SerializeField] public Animator animator;

    // DROP FIX (NEW)

    private Coroutine dropToGroundRoutine;

    private bool IsInJumpState()
    {
        return _currentJumpState == JumpState.ASCENDING ||
               _currentJumpState == JumpState.HOVERING ||
               _currentJumpState == JumpState.FALLING ||
               _currentJumpState == JumpState.SLAM;
    }

    private IEnumerator DropToGround()
    {
        float dropSpeed = 15f;

        while (!isGrounded)
        {
            // Stop drop if jump begins
            if (IsInJumpState())
            {
                dropToGroundRoutine = null;
                yield break;
            }

            Vector3 downward = Vector3.down * dropSpeed * Time.deltaTime;
            _characterController.Move(downward);

            yield return null;
        }

        dropToGroundRoutine = null;
    }

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _playerInputActions.UI.Disable();
        _characterController = GetComponent<CharacterController>();
        _groundPosition = GetComponent<GroundBelowPlayer>();
        _jumpImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        if (_playerCamera != null)
            _playerCameraScript = _playerCamera.GetComponent<PlayerCameraScript>();

        currentJumpTime = maxJumpTime; // HOVER (initialize hover timer)
        //Debug.Log($"[HOVER] Initialized hover time: {currentHoverTime}"); // DEBUG

        if (jumpCooldownUI != null)
        {
            jumpCooldownUI.SetReady(_currentJumpState == JumpState.READY_TO_JUMP);
        }

        if (DifficultyManager.Instance != null)
        {
            slamCooldownDuration *= DifficultyManager.Instance.playerCooldownMultiplier;
        }

        DataPersistenceManager.instance.GameData.totalRuns++;
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

        // DROP FIX TRIGGER (NEW)
        if (!isGrounded && !IsInJumpState())
        {
            if (dropToGroundRoutine == null)
                dropToGroundRoutine = StartCoroutine(DropToGround());
        }

        if (isKnockedBack) return; // KNOCKBACK (disable control during knockback)

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (hammerAttack != null && isGrounded)
            {
                hammerAttack.StartAttack(HammerAttack.AttackType.Tornado);
            }
        }

        ApplyRotation();
        ApplyMovement();
        UpdateAnimation();
    }

    public void StartTornadoSpin()
    {
        if (hammerAttack != null)
            hammerAttack.StartTornadoSpin();
    }

    public void EndTornadoSpin()
    {
        if (hammerAttack != null)
            hammerAttack.EndTornadoSpin();
    }

    // Attack callback (Pure New Input System)
    private void OnAttack(InputAction.CallbackContext context)
    {
        // Prevent hammer attack while hovering or not grounded
        if (!isGrounded) // || _currentJumpState != JumpState.READY_TO_JUMP || _currentJumpState != JumpState.JUMP_ON_COOLDOWN)
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
        //if (_moveInput == Vector3.zero) return;

        //Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        //Vector3 lookInput = isometricMatrix.MultiplyPoint3x4(_moveInput);

        //Quaternion rotation = Quaternion.LookRotation(lookInput, Vector3.up);

        //// IMPORTANT: This line locks the playerParent rotation to the horizontal plane only. 
        //rotation.Set(0.0f, rotation.y, 0.0f, rotation.w);

        //transform.rotation = Quaternion.RotateTowards(
        //    transform.rotation,
        //    rotation,
        //    turnSpeed * Time.deltaTime
        //);

        Vector3 horizontalInput = _moveInput;
        if (horizontalInput.sqrMagnitude < 0.01f) return;

        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 45f, 0f));
        Vector3 lookInput = isoMatrix.MultiplyPoint3x4(horizontalInput);

        Quaternion targetRotation = Quaternion.LookRotation(lookInput, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        Vector2 moveInput = callbackContext.ReadValue<Vector2>();
        _moveInput = new Vector3(moveInput.x, 0f, moveInput.y);
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

    
    // KNOCKBACK (restored only — no changes to your system)
    public void TakeHit(Vector3 hitSourcePosition, float damage)
    {
        if (isKnockedBack || isInvincible)
            return;

        Vector3 direction = transform.position - hitSourcePosition;
        direction.y = 0f;
        direction.Normalize();

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(direction));
    }

    // allows granny to be invincible for a set amount of seconds
    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;

        yield return new WaitForSeconds(duration);

        isInvincible = false;
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

    // This function is called via the Player Input component Invoke Unity Events by the Jump Action on the Player Action Map 
    public void OnJumpInputEvent(InputAction.CallbackContext callbackContext)
    {
        // When the Jump Action button is pressed
        if (callbackContext.action.WasPressedThisFrame() && JumpingPermitted())
        {
            //Debug.LogWarning("Jump Press event called.");
            _jumpAbilityRiseCoroutine = StartCoroutine(JumpAbilityRise());
        }

        // When the Jump Action button is released
        if (callbackContext.action.WasReleasedThisFrame() && !isGrounded && _currentJumpState == JumpState.HOVERING && !JumpingPermitted())
        {        
            //Debug.LogWarning("Jump Release event called.");
            _jumpAbilityFallSlamCoroutine = StartCoroutine(JumpAbilityFallSlam());
        }

    }

    // This function checks whether the player can start the jump ability
    private bool JumpingPermitted()
    {
        bool canPlayerStartJumping;
        canPlayerStartJumping = (isGrounded && _currentJumpState == JumpState.READY_TO_JUMP) ? true : false;
        //Debug.LogWarning(_currentJumpState);

        return canPlayerStartJumping;
    }
    
    // This coroutine handles the start of the jump and jump ascent state.
    private IEnumerator JumpAbilityRise()
    {
        _currentJumpState = JumpState.ASCENT_START;
        _currentJumpState = JumpState.ASCENDING;

        float jumpRiseDuration = 1.0f;
        float jumpRiseHeight = 5.0f;
        
        //Debug.Log("Jump Rise Coroutine begun.");

        groundAttack?.StartCharge(transform.position, maxJumpTime); // GROUND ATTACK
        if (grannySoundScript != null)
        {
            grannySoundScript.PlayJump();
        }

        // While the player is not at or above the designated jumpRiseHeight - this while loop acts like a mini Update function for just this coroutine
        while (transform.position.y <= (_groundPosition.GroundPointTransform.position.y + jumpRiseHeight))
        {
            //Debug.LogWarning("Moving from Y: " + transform.position.y + " To Y : " + (_groundPosition.GroundPointTransform.position.y + 5.0f));
            
            // Move the player towards the targeted height over the specified duration
            Mathf.SmoothDamp(transform.position.y,  (_groundPosition.GroundPointTransform.position.y + 5.0f), ref _verticalVelocity, jumpRiseDuration);

            currentJumpTime -= Time.deltaTime; // HOVER

            groundAttack?.UpdateCharge(transform.position); // GROUND ATTACK
            yield return null; // advances to next frame
        }

        // once at desired height, halt _verticalVelocity
        _verticalVelocity = 0.0f;
        
        //Debug.Log("Jump Rise Coroutine finished.");

        // helps stop the rising coroutine
        _jumpAbilityRiseCoroutine = null;

        // starts the hovering coroutine
        _jumpAbilityHoverCoroutine = StartCoroutine(JumpAbilityHover());
        yield return _jumpAbilityHoverCoroutine;
    }

    // This coroutine handles the hovering jump state.
    private IEnumerator JumpAbilityHover()
    {
        animator.ResetTrigger("isCharging");
        animator.SetTrigger("isCharging");

        //Debug.Log("Jump Hover Coroutine begun.");
        _currentJumpState = JumpState.HOVERING;
        
        // while the player can still hover - this while loop acts like a mini Update function for just this coroutine
        while (currentJumpTime > 0.0f)
        {
            currentJumpTime -= Time.deltaTime; // HOVER
            groundAttack?.UpdateCharge(transform.position); // GROUND ATTACK
            yield return null;
        }

        //Debug.Log("Jump Hover Coroutine finished.");

        // helps stop the hovering coroutine
        _jumpAbilityHoverCoroutine = null;

        // starts the falling coroutine
        _jumpAbilityFallSlamCoroutine = StartCoroutine(JumpAbilityFallSlam());
        yield return _jumpAbilityFallSlamCoroutine;
        

    }

    // This coroutine handles the fall and slam state of the jump ability.
    private IEnumerator JumpAbilityFallSlam()
    {
        animator.ResetTrigger("isAirAttacking");
        animator.SetTrigger("isAirAttacking");

        float jumpFallDuration = 0.3f;
        //Debug.Log("Jump Fall Slam Coroutine begun.");

        // this code block helps ensure the fall state occurs cleanly
        {
            if (_jumpAbilityRiseCoroutine != null) StopCoroutine(_jumpAbilityRiseCoroutine);
            if (_jumpAbilityHoverCoroutine != null) StopCoroutine(_jumpAbilityHoverCoroutine);   
        }

        _currentJumpState = JumpState.FALLING;

        // While the player is not grounded - this while loop acts like a mini Update function for just this coroutine
        while (!isGrounded)
        {
            // Move the player towards the ground over the specified duration
            Mathf.SmoothDamp(transform.position.y,
                _groundPosition.GroundPointTransform.position.y,
                ref _verticalVelocity,
                jumpFallDuration);
            yield return null;
        }

        groundAttack?.StopCharge(); // GROUND ATTACK
        _currentJumpState = JumpState.SLAM;
        _jumpImpulseSource.GenerateImpulse();
        StartCoroutine(InvincibilityCoroutine(slamInvincibilityDuration));
        _verticalVelocity = 0.0f;
        _currentJumpState = JumpState.JUMP_ON_COOLDOWN;

        if (jumpCooldownUI != null)
        {
            jumpCooldownUI.SetReady(false);
        }

        // Play slam impact sound (MOVE IT HERE)
        if (grannySoundScript != null)
        {
            grannySoundScript.PlaySlamImpact();
        }

        // this code block helps reset 
        {
            currentJumpTime = maxJumpTime; // HOVER
            slamCooldownTimer = slamCooldownDuration;  
            slamLogTimer = 0f;

            if (showSlamCooldownDebug)  
                Debug.Log($"[SLAM] Cooldown started ({slamCooldownDuration:F1}s)");  
            
        }

        //Debug.Log("Jump Fall Slam Coroutine Finished.");

        _jumpAbilityFallSlamCoroutine = null;
        yield break;
    }

    // NEW — small delay so particles aren't destroyed same frame
    private const float cooldownFinishBuffer = -0.1f;

    // This function manages the cooldown for the jump slam
    private void JumpSlamCooldownManager()
    {
        if (_currentJumpState == JumpState.JUMP_ON_COOLDOWN)  
        {
            slamCooldownTimer -= Time.deltaTime;  
            
            //  Spawn feedback ONCE when cooldown reaches 0
            //  Spawn feedback ONCE when cooldown reaches 0
            if (slamCooldownTimer <= 0f && !slamCooldownFeedbackTriggered)
            {
                slamCooldownFeedbackTriggered = true;

                if (slamCooldownParticles != null)
                {
                    // Spawn at player feet (ground position)
                    Vector3 spawnPos = _groundPosition.GroundPointTransform.position;
                    spawnPos += Vector3.up * 0.05f; // small offset to prevent clipping

                    activeCooldownParticles = Instantiate(
                        slamCooldownParticles,
                        spawnPos,
                        Quaternion.identity
                    );

                    // NEW — parent to ground so it follows player correctly
                    if (_groundPosition != null && _groundPosition.GroundPointTransform != null)
                    {
                        activeCooldownParticles.transform.SetParent(_groundPosition.GroundPointTransform);
                        activeCooldownParticles.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        Debug.LogWarning("[SLAM DEBUG] GroundPointTransform missing — particles not parented.");
                    }
            
                    // Play all particle systems (including children)
                    ParticleSystem[] systems = activeCooldownParticles.GetComponentsInChildren<ParticleSystem>();

                    foreach (ParticleSystem ps in systems)
                    {
                        ps.Play();
                    }

                    //Debug.Log("[SLAM DEBUG] Spawned cooldown particles at: " + spawnPos);
                }
                    if (grannySoundScript != null)
                    {
                        grannySoundScript.PlayCooldownFinished();
                    }
            }

            if (showSlamCooldownDebug)
            {
                slamLogTimer += Time.deltaTime;  

                if (slamLogTimer >= slamLogInterval)  
                {
                    //Debug.Log($"[SLAM] Cooldown remaining: {slamCooldownTimer:F1}s");  
                    slamLogTimer = 0f;  
                }
            }

            // Cleanup AFTER small buffer delay
            if (slamCooldownTimer <= cooldownFinishBuffer)  
            {
                // (Cooldown Feedback) — stop particles
                if (activeCooldownParticles != null)
                {
                    Destroy(activeCooldownParticles.gameObject, 0.5f); // small delay so effect finishes
                    activeCooldownParticles = null;
                }

                slamCooldownFeedbackTriggered = false;

                _currentJumpState = JumpState.READY_TO_JUMP;
                slamCooldownTimer = 0f;  
                slamLogTimer = 0f;

                if (jumpCooldownUI != null)
                {
                    jumpCooldownUI.SetReady(true);
                }

                if (showSlamCooldownDebug)
                {    
                    //Debug.Log("[SLAM] Cooldown finished");  
                }
            }
        }
    }

    // for charging effects
    public void OnAirAttackStart()
    {
        // Debug.Log("Air attack anim started");
    }

    // spawns particles on the ground for the jump attack
    public void OnAirSlamImpact()
    {
        if (groundAttackParticles != null && _groundPosition != null)
        {
            Vector3 groundPos = _groundPosition.GroundPointTransform.position;
            groundPos += Vector3.up * 0.05f;

            ParticleSystem ps = Instantiate(
                groundAttackParticles,
                groundPos,
                Quaternion.Euler(90f, 0f, 0f)
            );

            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        Vector3 horizontalMove = new Vector3(_moveInput.x, 0f, _moveInput.z);
        float speed = horizontalMove.magnitude;

        animator.SetFloat("Speed", speed);
    }

    // POWERUP METHODS
    private bool hasEmpoweredSlam = false;

    public void ApplySpeedBoost(int pickCount)  
    {
        float amount = 3f / (1f + ((pickCount - 1) * 0.5f));
        runSpeed += amount;

        Debug.Log("[POWERUP] Speed Boost +" + amount + " | Total Run Speed = " + runSpeed);
    }

    public void ApplyExtraHealth(int pickCount)  
    {
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            float amount = 25f / (1f + ((pickCount - 1) * 0.5f));
            health.IncreaseMaxHealth(amount);

            Debug.Log("[POWERUP] Extra Health +" + amount);
        }
    }

    public void ApplyGroundSmashUpgrade(int pickCount)  
    {
        slamCooldownDuration *= 0.5f;
        Debug.Log("[POWERUP] Ground Smash applied (one-time)");
    }

    public void ApplyQuickSwing(int pickCount)  
    {
        if (hammerAttack != null)
        {
            hammerAttack.ReduceCooldown(0.5f);
            Debug.Log("[POWERUP] Quick Swing applied (one-time)");
        }
    }

    public void ApplyEmpoweredSlam(int pickCount)
    {
        if (hasEmpoweredSlam) return;

        if (groundAttack != null)
        {
            groundAttack.EnableDamage();
            hasEmpoweredSlam = true;

            Debug.Log("[POWERUP] Ground Smash now deals damage.");
        }
    }

    public void ApplyHammerDamage(int pickCount)
    {
        if (hammerAttack != null)
        {
            float amount = 5f / (1f + ((pickCount - 1) * 0.5f));
            hammerAttack.IncreaseDamage(amount);

            Debug.Log("[POWERUP] Hammer Damage +" + amount);
        }
    }

    public void ApplyHammerKnockback(int pickCount)
    {
        if (hammerAttack != null)
        {
            hammerAttack.IncreaseKnockback();
            Debug.Log("[POWERUP] Hammer knockback upgraded.");
        }
    }

    // NEW — expose invincibility state
    public bool IsInvincible()
    {
        return isInvincible;
    }

    public void ApplyJumpDurationUpgrade(int pickCount)
    {
        float amount = 1f / (1f + ((pickCount - 1) * 0.5f));
        maxJumpTime += amount;

        Debug.Log("[POWERUP] Jump Duration +" + amount + " | Total = " + maxJumpTime);
    }

    public void ApplyJumpStunUpgrade(int pickCount)
    {
        if (groundAttack != null)
        {
            float amount = 1f / (1f + ((pickCount - 1) * 0.5f));
            groundAttack.IncreaseStunDuration(amount);

            Debug.Log("[POWERUP] Slam Stun Duration +" + amount);
        }
    }  

    public void ApplyJumpAOEUpgrade(int pickCount)
    {
        if (groundAttack != null)
        {
            float amount = 2f / (1f + ((pickCount - 1) * 0.5f));
            groundAttack.IncreaseAOESize(amount);

            Debug.Log("[POWERUP] Slam AOE Size +" + amount);
    }
    }

    public float GetSlamCooldownTimer()
    {
        return slamCooldownTimer;
    }

    public float GetSlamCooldownDuration()
    {
        return slamCooldownDuration;
    }

    public void LoadData(GameData data)
    {
        this.runSpeed = data.runSpeed;
        this.maxJumpTime = data.maxJumpTime;
        this.slamCooldownDuration = data.jumpSlamCooldown;
    }

    public void SaveData(ref GameData data)
    {
        //data.runSpeed = this.runSpeed;
        //data.maxJumpTime = this.maxJumpTime;
        //data.jumpSlamCooldown = this.slamCooldownDuration;

        // empty for now
    /* 
        Note from Jonathan:
        I can't think of too many/any of the variables within this script that we would need to 
        save from this script, moreso that this script will need to load a lot of variables. We 
        have to ensure that we're not upgrading Granny in a run using Gifts, then carrying over those upgrades, 
        just the Glamours.

        NEW (Thomas) — Added data persistence support for player glamour stats
        Saving and loading:
        - runSpeed
        - maxJumpTime
        - slamCooldownDuration (jumpSlamCooldown in GameData)
        These values are treated as Glamour upgrades and persist between runs.
    */
    }
}