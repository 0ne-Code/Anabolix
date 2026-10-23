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

    private Rigidbody rb;
    private PlayerInputActions playerInput;


    void Awake()
    {
        playerInput = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

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
        if (isGrounded) verticalVel = jumpForce;
    }
    void OnDisable()
    {
        playerInput.Player.Jump.performed -= HandleJump;
        playerInput.Player.Disable();
    }
    void FixedUpdate()
    {
        CheckGrounded();
        HandleGravity();
        HandleMovement();
    }
    void HandleMovement()
    {
        float inputX;
        float inputY;

        Vector2 moveInput = playerInput.Player.Move.ReadValue<Vector2>();

        bool sprinting = playerInput.Player.Sprint.IsPressed();
        float targetSpeed = sprinting ? (moveSpeed * sprintMultiplier) : moveSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintTransitionSpeed * Time.fixedDeltaTime);

        float airControl = isGrounded ? 1f : 0.7f;
        inputX = moveInput.x * currentSpeed * airControl;
        inputY = moveInput.y * currentSpeed * airControl;

        Vector3 currentLocalVel = new Vector3(rb.linearVelocity.x, verticalVel, rb.linearVelocity.z);
        Vector3 localVel = Quaternion.Euler(0f, mouseLook.currentYaw, 0f) * new Vector3(inputX, 0f, inputY);

        localVel.y = verticalVel;

        if (!isGrounded)
        {
            rb.linearVelocity = Vector3.Lerp(currentLocalVel, localVel, 3f * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = localVel;            
        }

    }
    void HandleGravity()
    {
        if (isGrounded && verticalVel < 0f) verticalVel = -1f;
        else verticalVel += gravity * Time.fixedDeltaTime;
    }
    void CheckGrounded()
    {
        //RaycastHit hitInfo;
        Vector3 castPos = transform.position + (Vector3.down * castPosOffset);

        bool hit = Physics.CheckSphere(castPos, sphereRadius, layerMask);

        isGrounded = hit;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.blue : Color.red;
        Vector3 drawPos = transform.position + (Vector3.down * castPosOffset);

        Gizmos.DrawWireSphere(drawPos, sphereRadius);
    }
}