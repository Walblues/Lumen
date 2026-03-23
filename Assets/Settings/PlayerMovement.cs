using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSmoothTime = 0.12f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedGravity = -2f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController _controller;
    private float _verticalVelocity;
    private float _rotationVelocity;

    private InputAction _moveAction;
    private InputAction _runAction;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();

        // Get actions directly from the PlayerInput component on this GameObject
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _runAction  = playerInput.actions["Sprint"]; // Change name if yours differs
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        Vector3 inputDir = new Vector3(input.x, 0f, input.y).normalized;

        bool isRunning = _runAction != null && _runAction.IsPressed();
        float speed = isRunning ? runSpeed : walkSpeed;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                                 + cameraTransform.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref _rotationVelocity,
                rotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        _controller.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (_controller.isGrounded)
            _verticalVelocity = groundedGravity;
        else
            _verticalVelocity += gravity * Time.deltaTime;
    }
}