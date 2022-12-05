using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Version 1.0
public class PlayerController : MonoBehaviour
{
//---------------------------------------------------------------DECLARATIONS SECTION----------------------------------------------------------------------

    //Player Settings------------------------------------------------------------------------------------------------------------
    [Header("Player")]
    [Tooltip("How fast the player moves while walking")]
    public float moveSpeed = 5.0f;

    [Tooltip("How fast the player moves while sprinting")]
    public float sprintSpeed = 8.0f;

    [Tooltip("How high can the player jump")]
    public float jumpHeight = 1.5f;

    [Tooltip("The player has a different gravity multiplier")]
    public float playerGravity = -15.0f;

    //Ground Settings------------------------------------------------------------------------------------------------------------
    [Header("Ground Settings")]
    [Tooltip("Is the player on the ground?")]
    public bool isGrounded = true;

    [Tooltip("Offset used to calculate ground boundaries")]
    public float groundedOffset = 0.78f;

    [Tooltip("The radius used to check if player is grounded. Should match the radius of Character Controller")]
    public float groundedRadius = 0.5000001f;

    [Tooltip("The time the player must wait before jumping again")]
    public float jumpCooldown = 0.5f;

    [Tooltip("Time required to enter falling state. Useful for walking down stairs and steep hills")]
    public float fallTimeout = 0.15f;

    [Tooltip("All layers that are considered ground")]
    public LayerMask groundLayers;


    //Camera Settings------------------------------------------------------------------------------------------------------------

    [Header("Camera Settings")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cameraTarget;

    [Tooltip("How sensitive will the rotation of the camera be")]
    [Range(0.1f, 1.0f)]
    public float mouseSensitivity = 0.5f;

    [Tooltip("How far in degrees can you move the camera up")]
    public float topClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float bottomClamp = -30.0f;

    [Tooltip("How fast will the camera rotate")] 
    [Range(0.0f , 0.5f)]
    public float rotationTime = 0.12f;

    //Private objects-------------------------------------------------------------------------------------------------------------------
    GameObject _mainCamera;
    CharacterController _controller;

    //Private Variables-----------------------------------------------------------------------------------------------------------------
    float _moveSpeed; //Stores ground velocity for the player
    float _targetRotation = 0.0f;
    float _rotationVelocity; //How fast the player rotates
    float _verticalVelocity; //Used when jumping or falling
    float _cinemachineTargetYaw; //Horizontal axis camera movement
    float _cinemachineTargetPitch; //Vertical axis camera movement
    float _terminalVelocity = 53.0f; //Maximum velocity able to be reached

    //Fall and Jump Delta---------------------------------------------------------------------------------------------------------------
    [SerializeField] float _fallTimeoutDelta;
    [SerializeField] float _jumpCooldownDelta;

    //Player Input----------------------------------------------------------------------------------------------------------------------
    public PlayerInputActions playerControls;

    //Input Actions---------------------------------------------------------------------------------------------------------------------
    InputAction playerMovement;
    InputAction playerJump;
    InputAction playerAttack;
    InputAction playerLook;
    InputAction playerSprint;

    //Move and Look directions----------------------------------------------------------------------------------------------------------
    Vector3 moveDirection;
    Vector2 lookDirection;


 //---------------------------------------------------------------END OF DECLARATIONS SECTION--------------------------------------------------------------


//----------------------------------------------------------------METHODS SECTION--------------------------------------------------------------------------
    void Awake()
    {
        
        playerControls = new PlayerInputActions();

        //Find the main camera at the start 
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>(); //Fetch the CharacterController component
        
        _cinemachineTargetYaw = cameraTarget.transform.rotation.eulerAngles.y; //Set the camera 
        
        Cursor.lockState = CursorLockMode.Locked; //Lock the cursor

        _fallTimeoutDelta = fallTimeout;
        _jumpCooldownDelta = jumpCooldown;
 
    }
  

  

    // Update is called once per frame
    void Update()
    {  
        GravitySystem();
       
        Move();
    }

    

    void LateUpdate()
    {
        CheckGrounded();

        RotateCamera();
    }


    void RotateCamera()
    {
        lookDirection = playerLook.ReadValue<Vector2>();

        // if there is an input
        if (lookDirection != Vector2.zero)
        {
            _cinemachineTargetYaw += lookDirection.x * mouseSensitivity;
            _cinemachineTargetPitch += lookDirection.y * mouseSensitivity;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        cameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
               _cinemachineTargetYaw, 0.0f);
    }


    void CheckGrounded()
    {
        // Set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        //Checks if the nearby imaginary sphere meets the requirements to be ground for the player
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
    }



    void Move()
    {

        moveDirection = playerMovement.ReadValue<Vector2>();

        //If there is input
        if (moveDirection == Vector3.zero) 
        {
            _moveSpeed = 0.0f;
        }
        else
        {
            //Checks if the player is currently sprinting 
            if (playerSprint.IsPressed())
            {
                _moveSpeed = sprintSpeed;
            }
            else
            {
                _moveSpeed = moveSpeed;
                
            }
            //Finds the tan line, which depicts the linear direction of our target rotation
            _targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationTime);

            // Rotates the player according to the camera
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 newDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // Move the player
        _controller.Move(newDirection.normalized * (_moveSpeed * Time.deltaTime) //Calculate movement in horizontal axis
            + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime) ; //and add movement to the vertical axis
    }


    void GravitySystem()
    {

        if (isGrounded)
        {
            //Reset fall timer
            _fallTimeoutDelta = fallTimeout;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jumpCooldownDelta >= 0.0f)
            {
                _jumpCooldownDelta -= Time.deltaTime;
            }

            if (playerJump.WasPressedThisFrame() && _jumpCooldownDelta < 0.0f)
            {
                //Velocity needed to reach a certain height is given by the physics equation Vf = SquareRoot(2*g*h).
                //We multiply by -2 because gravity is a negative number 
                _verticalVelocity = Mathf.Sqrt(-2f * playerGravity * jumpHeight);

            }

        }
        else
        {
            //Reset jump cooldown
            _jumpCooldownDelta = jumpCooldown;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += playerGravity * Time.deltaTime;
        }

    }

    //Attack system, to be implemented
    void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack context: " + context);
    }


    //Version of clamp used both for negative and positive angles
    static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    //Enabling all input systems
    void OnEnable()
    {
        //Movement System
        playerMovement = playerControls.Player.Move;
        playerMovement.Enable();

        //Camera Syste,
        playerLook = playerControls.Player.Look;
        playerLook.Enable();

        //Jump System
        playerJump = playerControls.Player.Jump;
        playerJump.Enable();

        //Attack System
        playerAttack = playerControls.Player.Attack;
        playerAttack.Enable();
        playerAttack.performed += Attack;

        //Sprint System
        playerSprint = playerControls.Player.Sprint;
        playerSprint.Enable();

    }


    //Disable input systems 
    void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        playerLook.Disable();
        playerAttack.Disable();
        playerSprint.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }
}
