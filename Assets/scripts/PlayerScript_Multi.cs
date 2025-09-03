using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript_Multi : MonoBehaviour
{
    //camrea
    public Transform Camera;                 
    private Vector3 originalForward;

    // movemnet
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float sprintSpeed = 8f;
    [SerializeField] public float TurnSmoothTime = 0.1f;
   
       private float TurnSmoothVel;
    private Vector2 moveInput;                              // WASD & left stick on controller
    private Rigidbody rb;                      
    private bool usePlayerInput;                           

    //Jump 
    public float jumpForce = 10f;                           
    public float fallMultiplier = 2.5f;                     // makes the fall look good
    public float ascendMultiplier = 2f;
    private bool isGrounded = true;
    public LayerMask groundLayer = ~0;
    private float raycastDistance;

    // Crouching
    private bool isCrouching = false;
    private float startYScale;            // stores player height (scale.y)
    public float crouchSpeed = 2f;             //smooth move for crouch
    public float crouchYScale = 0.5f;         // half height

    // Stamina & Sprint 
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 22f;               // while sprinting
    public float staminaRegenPerSecond = 16f;               // while not sprinting / idle
    [Range(0f, 1f)] public float sprintMinPercentToStart = 0.2f;
    private float stamina;
    private bool sprintHeld;

    // Input
    private InputSystem_Actions inputActions;              
    private PlayerInput playerInput;        //for the multi          


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // keep capsule upright

        playerInput = GetComponent<PlayerInput>();
        {
            usePlayerInput = playerInput != null;
        }

        if (!usePlayerInput)
            {
                inputActions = new InputSystem_Actions();
            }
    }

    void OnEnable()
    {
        if (!usePlayerInput)
        {
            inputActions.Player.Enable();

           inputActions.Player.Jump.performed += ctx => Jump();
            inputActions.Player.Crouch.performed  += ctx => Crouch();
           
         inputActions.Player.Sprint.started   += ctx => sprintHeld = true;
            inputActions.Player.Sprint.canceled  += ctx => sprintHeld = false;
        }

        stamina = maxStamina;
    }

    void OnDisable()
    {
        if (!usePlayerInput && inputActions != null)
        {
              inputActions.Player.Disable();
        }
    }

    void Start()
    {
     
        float playerHeight = 2f;
        if (TryGetComponent<CapsuleCollider>(out var cap))
        {
            playerHeight = cap.height * transform.localScale.y;
        }

        raycastDistance = (playerHeight / 2f) + 0.2f;

        startYScale = transform.localScale.y;
        originalForward = transform.forward;
    }

    void Update()
    {
            if (!usePlayerInput)
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();

             Sprint();
    }

    void FixedUpdate()
    {
        Move();
        ApplyJumpPhysics(); 
    }

    void Move()
    {
        Vector3 camForward = Vector3.forward;
        Vector3 camRight   = Vector3.right;
        if (Camera)
        {
            camForward = Vector3.Scale(Camera.forward, new Vector3(1, 0, 1)).normalized;
            camRight   = Vector3.Scale(Camera.right,   new Vector3(1, 0, 1)).normalized;
        }

        Vector3 movement = (camRight * moveInput.x + camForward * moveInput.y).normalized;
      

        bool canSprint = sprintHeld && stamina > 0.01f && movement.sqrMagnitude > 0.01f && !isCrouching;
        float chosenSpeed = isCrouching ? crouchSpeed : (canSprint ? sprintSpeed : moveSpeed);

        Vector3 targetVelocity = movement * chosenSpeed;

        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

      
        if (isGrounded && moveInput == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

    
        if (movement.sqrMagnitude >= 0.01f) //smoothness for roation and movement
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVel, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void Sprint()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);

        // Stamina logic
        bool wantsSprint = sprintHeld && moveInput.sqrMagnitude > 0.01f;
        bool hasStartStamina = stamina >= maxStamina * sprintMinPercentToStart;

        if (wantsSprint && (hasStartStamina || stamina > 0f))
        {
            stamina = Mathf.Max(0f, stamina - staminaDrainPerSecond * Time.deltaTime);
        }

        else
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenPerSecond * Time.deltaTime);
        }
    }

    private void Crouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false; 
            Vector3 jumpVelocity = rb.linearVelocity;
            jumpVelocity.y = jumpForce;
            rb.linearVelocity = jumpVelocity;
        }
    }

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += fallMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; // up
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += ascendMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; // down/fall
        }
    }

    // -------- PlayerInput (local multiplayer) --------
    public void OnMove(InputValue value)  => moveInput = value.Get<Vector2>();
    public void OnJump(InputValue value)  { if (value.isPressed) Jump(); }
    public void OnSprint(InputValue value){ sprintHeld = value.isPressed; }
    public void OnCrouch(InputValue value){ if (value.isPressed) Crouch(); }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * raycastDistance);
    }
#endif
}