
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

    private const float gravity = -19.62f;

    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Rigidbody rb;

    private PlayerInputActions playerInput;
    private bool movementLocked;
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => playerInput.Player.Sprint.IsPressed();
    public bool IsMoving => playerInput.Player.Move.ReadValue<Vector2>().sqrMagnitude > 0.01f;
    public bool JustJumped { get; private set; }
    public bool JustLanded { get; private set; }

    void Awake()
    {
        playerInput = new PlayerInputActions();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentSpeed = moveSpeed;
    }

    void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Jump.performed += HandleJump;
    }

    void HandleJump(InputAction.CallbackContext ctx)
    {
        if (movementLocked)
            return;

        if (isGrounded)
        {
            verticalVel = jumpForce;
            JustJumped = true;
        }
    }

    void OnDisable()
    {
        playerInput.Player.Jump.performed -= HandleJump;
        playerInput.Player.Disable();
    }

    void FixedUpdate()
    {
        //JustJumped = false;
        //JustLanded = false;

        CheckGrounded();
        HandleGravity();

        if (movementLocked)
        {
            rb.linearVelocity = new Vector3(
                0f,
                verticalVel,
                0f
            );

            return;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        float inputX;
        float inputY;

        Vector2 moveInput = playerInput.Player.Move.ReadValue<Vector2>();

        bool sprinting = playerInput.Player.Sprint.IsPressed();
        float targetSpeed = sprinting
            ? (moveSpeed * sprintMultiplier)
            : moveSpeed;

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            sprintTransitionSpeed * Time.fixedDeltaTime
        );

        float airControl = isGrounded ? 1f : 0.7f;

        inputX = moveInput.x * currentSpeed * airControl;
        inputY = moveInput.y * currentSpeed * airControl;

        Vector3 currentLocalVel = new Vector3(
            rb.linearVelocity.x,
            verticalVel,
            rb.linearVelocity.z
        );

        Vector3 localVel = Quaternion.Euler(
            0f,
            mouseLook.currentYaw,
            0f
        ) * new Vector3(inputX, 0f, inputY);

        localVel.y = verticalVel;

        if (!isGrounded)
        {
            rb.linearVelocity = Vector3.Lerp(
                currentLocalVel,
                localVel,
                3f * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.linearVelocity = localVel;
        }
    }

    void HandleGravity()
    {
        if (isGrounded && verticalVel < 0f)
            verticalVel = -1f;
        else
            verticalVel += gravity * Time.fixedDeltaTime;
    }

    void CheckGrounded()
    {
        bool wasGrounded = isGrounded;

        Vector3 castPos = transform.position +
            (Vector3.down * castPosOffset);

        isGrounded = Physics.CheckSphere(
            castPos,
            sphereRadius,
            layerMask
        );

        JustLanded = !wasGrounded && isGrounded;
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        if (locked)
        {
            verticalVel = 0f;
        }
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

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.blue : Color.red;

        Vector3 drawPos = transform.position + (Vector3.down * castPosOffset);

        Gizmos.DrawWireSphere(drawPos, sphereRadius);
    }
}
