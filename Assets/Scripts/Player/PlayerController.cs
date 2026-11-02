
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float sprintTransitionSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float verticalVel;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float castPosOffset;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask layerMask;

    [Header("Gravity")]
    [SerializeField] private float normalGravity = -19.6f;
    [SerializeField] private float invertedGravity = -40f;

    [Header("Gravity Rotation")]
    [SerializeField] private float rotationSpeed = 360f;

    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerCamera;

    private PlayerInputActions playerInput;
    private bool movementLocked;
    private bool gravityInverted;

    private Quaternion initialRotation;
    private Quaternion targetRotation;

    public bool IsGrounded => isGrounded;
    public bool IsGravityInverted => gravityInverted;

    public bool IsSprinting => playerInput.Player.Sprint.IsPressed();

    public bool IsMoving =>
        playerInput.Player.Move.ReadValue<Vector2>().sqrMagnitude > 0.01f;

    public bool JustJumped { get; private set; }
    public bool JustLanded { get; private set; }
    public bool PlayerGravityMode { get; private set; }

    private void Awake()
    {
        playerInput = new PlayerInputActions();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentSpeed = moveSpeed;

        initialRotation = rb.rotation;
        targetRotation = initialRotation;
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Jump.performed += HandleJump;
    }

    private void HandleJump(InputAction.CallbackContext ctx)
    {
        if (movementLocked)
            return;

        if (isGrounded)
        {
            verticalVel = gravityInverted
                ? -jumpForce
                : jumpForce;

            JustJumped = true;
        }
    }

    private void OnDisable()
    {
        playerInput.Player.Jump.performed -= HandleJump;
        playerInput.Player.Disable();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleGravity();
        HandleRotation();

        if (movementLocked)
        {
            rb.linearVelocity = new Vector3(0f, verticalVel, 0f);
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = playerInput.Player.Move.ReadValue<Vector2>();

        bool sprinting = playerInput.Player.Sprint.IsPressed();

        float targetSpeed = sprinting
            ? moveSpeed * sprintMultiplier
            : moveSpeed;

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            sprintTransitionSpeed * Time.fixedDeltaTime
        );

        float airControl = isGrounded ? 1f : 0.7f;

        float inputX = moveInput.x * currentSpeed * airControl;
        float inputY = moveInput.y * currentSpeed * airControl;

        // Get the actual camera directions.
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;

        // Keep movement on the horizontal plane.
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement relative to camera direction.
        Vector3 moveDirection =
            cameraRight * inputX +
            cameraForward * inputY;

        // Preserve gravity movement.
        moveDirection.y = verticalVel;

        if (!isGrounded)
        {
            rb.linearVelocity = Vector3.Lerp(
                rb.linearVelocity,
                moveDirection,
                3f * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.linearVelocity = moveDirection;
        }
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            if (!gravityInverted && verticalVel < 0f)
                verticalVel = -1f;

            if (gravityInverted && verticalVel > 0f)
                verticalVel = 1f;

            return;
        }

        float currentGravity = gravityInverted
            ? invertedGravity
            : normalGravity;

        verticalVel += currentGravity * Time.fixedDeltaTime;
    }

    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;

        Vector3 gravityDirection = gravityInverted
            ? Vector3.up
            : Vector3.down;

        Vector3 castOrigin = transform.position;

        isGrounded = Physics.SphereCast(
            castOrigin,
            sphereRadius,
            gravityDirection,
            out RaycastHit hit,
            castPosOffset,
            layerMask,
            QueryTriggerInteraction.Ignore
        );

        JustLanded = !wasGrounded && isGrounded;
    }

    private void HandleRotation()
    {
        Quaternion nextRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(nextRotation);
    }

    public bool ToggleGravity()
    {
        if (!isGrounded)
            return false;

        gravityInverted = !gravityInverted;

        verticalVel = 0f;

        // Flip around X when inverted.
        // Restore the exact starting rotation when normal.
        targetRotation = gravityInverted
            ? initialRotation * Quaternion.Euler(180f, 0f, 0f)
            : initialRotation;

        return true;
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        if (locked)
            verticalVel = 0f;
    }

    public bool ConsumeJustJumped()
    {
        if (!JustJumped)
            return false;

        JustJumped = false;
        return true;
    }

    public bool ConsumeJustLanded()
    {
        if (!JustLanded)
            return false;

        JustLanded = false;
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.blue : Color.red;

        Vector3 gravityDirection = gravityInverted
            ? Vector3.up
            : Vector3.down;

        Vector3 drawPos = transform.position +
            gravityDirection * castPosOffset;

        Gizmos.DrawWireSphere(drawPos, sphereRadius);
    }

    public void EnablePlayerGravityMode()
    {
        PlayerGravityMode = true;
    }
}
