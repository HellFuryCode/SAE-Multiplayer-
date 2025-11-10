using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Collections;


public class PlayerScript_Multi : NetworkBehaviour
{
    // coop
    public enum KeyboardProfile { None, WASD, Arrows }
    public KeyboardProfile keyboardProfile = KeyboardProfile.None;


    //camrea
    public Transform Camera;
    public Vector2 lookInput;          
   private Vector3 originalForward;  //true north essenrtionally 
    [SerializeField] float faceCamLerp = 12f;  //how quick is the roate in the yaw (twisiting in vertail)

    // movemnet
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float sprintSpeed = 8f;  //shift / r2
    [SerializeField] public float TurnSmoothTime = 0.1f;
    private float TurnSmoothVel;
    private Vector2 moveInput;                              // WASD & left stick on controller
    private Rigidbody rb;                      
   // private bool usePlayerInput;                           //keyboard 


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
    public float staminaDrainPerSecond = 22f;               // while sprinting (ui TO be added)
    public float staminaRegenPerSecond = 16f;               // while not sprinting / idle
    [Range(0f, 1f)] public float sprintMinPercentToStart = 0.2f;
    private float stamina;
    private bool sprintHeld;   

    // Input
    private InputSystem_Actions inputActions;              
    private PlayerInput playerInput;        //for the multi     
    private bool usePlayerInput => playerInput != null && playerInput.enabled;
    private bool IsManualKeyboard => keyboardProfile != KeyboardProfile.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // keep capsule upright duh
  
    if (!IsOwner) return;

        playerInput = GetComponent<PlayerInput>();
        // {
        //     usePlayerInput = playerInput != null;
        // }

        // if (!usePlayerInput)
        //     {
        //         inputActions = new InputSystem_Actions();
        //     }
    }

    // void OnEnable()
    // {
    //     // if (!usePlayerInput)
    //     // {
    //     //     inputActions.Player.Enable();

    //     //    inputActions.Player.Jump.performed += ctx => Jump();
    //     //   // inputActions.Player.Jump.performed += ctx => OnLook();
           
    //     //  inputActions.Player.Sprint.started   += ctx => sprintHeld = true;
    //     //     inputActions.Player.Sprint.canceled  += ctx => sprintHeld = false;
    //     // }

     
    // }

 

    // void OnDisable()
    // {
    //     // if (!usePlayerInput && inputActions != null)
    //     // {
    //     //       inputActions.Player.Disable();
    //     // }
    // }

    void Start()
    {
        // if (!Camera && Camera.main)
        // {
        //     Camera = Camera.main.transform;
        // }

        float playerHeight = 2f;
        if (TryGetComponent<CapsuleCollider>(out var cap))
        {
            playerHeight = cap.height * transform.localScale.y;
        }

        raycastDistance = (playerHeight / 2f) + 0.2f;

        startYScale = transform.localScale.y;
        originalForward = transform.forward;
           stamina = maxStamina;
    }

    void Update()
    {
        if (!IsOwner) return;
        if (IsManualKeyboard)
        {
            if (keyboardProfile == KeyboardProfile.WASD) ReadWASD();
            else if (keyboardProfile == KeyboardProfile.Arrows) ReadArrows();
        }

        Sprint();
 
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Move();
        ApplyJumpPhysics(); 
    }

    private void ReadWASD() //maunal readers for split keyboard
    {
        var kb = Keyboard.current;
        if (kb == null)
        {
            return;
        }

        float x = 0f, y = 0f;
        if (kb.wKey.isPressed) y += 1f;
        if (kb.sKey.isPressed) y -= 1f;
        if (kb.dKey.isPressed) x += 1f;
        if (kb.aKey.isPressed) x -= 1f;

        moveInput = new Vector2(x, y).normalized;

        if (kb.spaceKey.wasPressedThisFrame)
        {
            Jump();
            sprintHeld = kb.leftShiftKey.isPressed;
        }

        if (kb.cKey.wasPressedThisFrame)
        {
            Crouch();
        }

    }

      private void ReadArrows() //maunal readers for split keyboard
    {
        var kb = Keyboard.current;
        if (kb == null)
        {
            return;
        }

        float x = 0f, y = 0f;
        if (kb.upArrowKey.isPressed) y += 1f;
        if (kb.downArrowKey.isPressed) y -= 1f;
        if (kb.rightArrowKey.isPressed) x += 1f;
        if (kb.leftArrowKey.isPressed) x -= 1f;

        moveInput = new Vector2(x, y).normalized;

        if (kb.enterKey.wasPressedThisFrame || kb.rightCtrlKey.wasPressedThisFrame)
        {
            Jump();
            sprintHeld = kb.rightShiftKey.isPressed;
        }

        if (kb.rightAltKey.wasPressedThisFrame || kb.slashKey.wasPressedThisFrame)
        {
            Crouch();
        }

    }


    void Move()
    {
        // if (rb.isKinematic) return; 

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

    // public override void OnNetWorkSpawn()
    // {
    //     if (!IsOwner)
    //     {
    //         enabled = false;
    //         return;
    //         }
    // }
    
    private void AlignToCameraTwist()
    {
        if (!Camera) return;

        Vector3 camFwd = Camera.forward; //flatting the camera so it face the rotation
        camFwd.y = 0f;

        if (camFwd.sqrMagnitude < 0.0001f) return;
        {
            Quaternion target = Quaternion.LookRotation(camFwd.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * faceCamLerp);
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
         if (rb.isKinematic) return;  
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
         if (rb.isKinematic) return;  
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += fallMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; // up
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += ascendMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; // down/fall
        }
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
 // PlayerInput  (controllers only) 
    public void Look(InputValue value)   { if (usePlayerInput && !IsManualKeyboard) lookInput = value.Get<Vector2>(); }
    public void OnMove(InputValue value)   { if (usePlayerInput && !IsManualKeyboard) moveInput = value.Get<Vector2>(); }
    public void OnJump(InputValue value)   { if (usePlayerInput && !IsManualKeyboard && value.isPressed) Jump(); }
    public void OnSprint(InputValue value) { if (usePlayerInput && !IsManualKeyboard) sprintHeld = value.isPressed; }
    public void OnCrouch(InputValue value) { if (usePlayerInput && !IsManualKeyboard && value.isPressed) Crouch(); }

    // Netcode: only owner should read input online
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * raycastDistance);
    }
#endif
}

// How To Make A HORROR Game In Unity 
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube

//  Unity Input System in Unity 6 (1/7): Input Action Editor.
//date accessed 2025/9/17
//created by: Unity
//created on: 2025
//url: https://www.youtube.com/watch?v=TiTKAseu17A 
//Online Video
//youtube
