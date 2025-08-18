using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Accessibility;
using Unity.VisualScripting;
using System;
using UnityEditor.Experimental.GraphView;
public class PlayerScript : MonoBehaviour
{
    //camera 
    public Transform Camera;
    private Vector3 originalForward;
   
     // Movement
    private Rigidbody rb;
      [SerializeField] public float moveSpeed = 5f;
     [SerializeField] public float TurnSmoothTime = 0.1f;
     private float TurnSmoothVel;  
    private Vector2 moveInput;  //controller



    // Jumping
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f; //makes the fall look good
    public float ascendMultiplier = 2f;
    private bool isGrounded = true;
    public LayerMask groundLayer;
    private float raycastDistance;

    //crouching
    private bool isCrouching = false;
    private float startYScale;  //takes storage of player height
    public float crouchSpeed = 2f; //smooth move for crouch
    public float crouchYScale = 0.5f; //half player height for croucnh duh

    // Input System
    private InputSystem_Actions inputActions;  //stupid fucker i got you to work ^w^
  

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Crouch.performed += ctx => Crouch();
  
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        float playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f; //raycast length is divded by 2 then plus 0.2

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        startYScale = transform.localScale.y;
         originalForward = transform.forward; //orginal forward facing postion for snap back 
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();  //input controller 

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; //detecion of the ground
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
       
    }

    void FixedUpdate()
    {
        Move();
        ApplyJumpPhysics();
    }

    void Move()
    {
        Vector3 camForward = Vector3.Scale(Camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(Camera.right, new Vector3(1, 0, 1)).normalized;
        Vector3 movement = (camRight * moveInput.x + camForward * moveInput.y).normalized;

        Vector3 targetVelocity = movement * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        if (isGrounded && moveInput == Vector2.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (movement.magnitude >= 0.1f) //still needs to be editted feels a little off
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg; //brackeys trignomtry shit
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVel, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

    }
//+ Camera.eulerAngles.y

    private void Crouch() //DNT
    {
        // Debug.Log("Crouched"); //works

        isCrouching = !isCrouching;

        if (isCrouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            moveSpeed = crouchSpeed;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            moveSpeed = 5f; //or the normal moveSpeed
        }
    }


    void Jump() //worked then didny then it works again. DO NOT TOUCH!
    {
        if (isGrounded)
        {
            isGrounded = false; //not jumping basically in the air
            Vector3 jumpVelocity = rb.linearVelocity;
            jumpVelocity.y = jumpForce;
            rb.linearVelocity = jumpVelocity;

         
        }
    }

    void ApplyJumpPhysics()   //makes fall smoother (DNT!)
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += fallMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; //up
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += ascendMultiplier * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up; //down/fall
        }
    }

}


//https://www.youtube.com/watch?v=xCxSjgYTw9c
// 
// How To Make A HORROR Game In Unity | whole series
//date accessed 2025/07/30
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=985AMajuZO4&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=4
//Online Video
//youtube