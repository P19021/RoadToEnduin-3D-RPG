using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Version 1.0
public class PlayerController : MonoBehaviour
{

    //Player settings
    [Header("Player")]
    [Tooltip("How fast the player moves while walking")]
    public float moveSpeed = 5.0f;

    [Tooltip("How fast the player moves while sprinting")]
    public float sprintSpeed = 8.0f;

    [Tooltip("How high can the player jump")]
    public float jumpHeight = 1.5f;


    //Camera Settings------------------------------------------------------------------------------------------------------------

    [Header("Camera Settings")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cameraTarget;

    [Tooltip("How sensitive will the rotation of the camera be")]
    public float mouseSensitivity = 1.0f;

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
    float _moveSpeed;
    float _targetRotation = 0.0f;
    float _rotationVelocity;
    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;

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

    bool _isSprinting = false;




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
        _controller = GetComponent<CharacterController>();

        _cinemachineTargetYaw = cameraTarget.transform.rotation.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
 
    }
  

  

    // Update is called once per frame
    void Update()
    {

    }

    

    void FixedUpdate()
    {
        Move();
        RotateCamera();
    }

      

    void Jump(InputAction.CallbackContext context)
    {
        float jumpHeight = transform.position.y;
        while (jumpHeight < 0.5f)
        {
            jumpHeight = Mathf.Lerp(transform.position.y, 0.5f, 1.5f);
            transform.position += new Vector3(0, jumpHeight, 0);
        }
    }

    //Algorithmos papapap
   

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
                
                //Set bool to false again, just in case
                _isSprinting = false;
            }
            
            _targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationTime);

            // Rotates the player according to the camera
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 newDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // Move the player
        _controller.Move(newDirection.normalized * (_moveSpeed * Time.deltaTime));
    }


    //Attack system, to be implemented
    void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack context: " + context);
    }

    //Sprint System, to be implemented
    void  Sprint(InputAction.CallbackContext context)
    {
        _isSprinting = true;
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
        playerJump.performed += Jump;

        //Attack System
        playerAttack = playerControls.Player.Attack;
        playerAttack.Enable();
        playerAttack.performed += Attack;

        //Sprint System
        playerSprint = playerControls.Player.Sprint;
        playerSprint.Enable();
        playerSprint.performed += Sprint;

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
}
