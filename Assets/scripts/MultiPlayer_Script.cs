using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]

public class MultiPlayer_Script : MonoBehaviour
{
    //Camera
    public Transform Camera; // assign the child Camera (or vcam) transform

    //Movement
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float TurnSmoothTime = 0.1f;
    private float TurnSmoothVel;

    //Jumping
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float ascendMultiplier = 2f;
    public LayerMask groundLayer;

    [Header("Crouch")]
    public float crouchSpeed = 2f;
    public float crouchYScale = 0.5f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private PlayerInput playerInput;

    // cached input actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // state
    private Vector2 moveInput;
    private bool isGrounded = true;
    private float raycastDistance;
    private bool isCrouching = false;
    private float startYScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();

        // Fetch this player's own actions instance from PlayerInput
        var actions = playerInput.actions;
        moveAction = actions["Move"];
        jumpAction = actions["Jump"];
        crouchAction = actions["Crouch"];

        // Subscribe per-player
        jumpAction.performed += _ => Jump();
        crouchAction.performed += _ => Crouch();

        rb.freezeRotation = true;

        float playerHeight = capsule.height * transform.localScale.y;
        raycastDistance = (playerHeight / 2f) + 0.2f;

        startYScale = transform.localScale.y;

        // If you forgot to assign Camera in the inspector, try to find a child camera
        if (Camera == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) Camera = cam.transform;
        }
    }

    void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        crouchAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        crouchAction?.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // Ground check
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
    }

    void FixedUpdate()
    {
        Move();
        ApplyJumpPhysics();
    }

    void Move()
    {
        // Camera-relative planar vectors
        Vector3 camForward = Vector3.Scale(Camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(Camera.right, new Vector3(1, 0, 1)).normalized;
        Vector3 movement = (camRight * moveInput.x + camForward * moveInput.y).normalized;

        Vector3 targetVelocity = movement * (isCrouching ? crouchSpeed : moveSpeed);

        Vector3 v = rb.linearVelocity;
        v.x = targetVelocity.x;
        v.z = targetVelocity.z;
        rb.linearVelocity = v;

        if (isGrounded && moveInput == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (movement.sqrMagnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVel, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    void Jump()
    {
        if (!isGrounded) return;

        isGrounded = false;
        Vector3 v = rb.linearVelocity;
        v.y = jumpForce;
        rb.linearVelocity = v;
    }

    void Crouch()
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

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime * Vector3.up;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime * Vector3.up;
        }
    }
}
