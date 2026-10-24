
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private const string SensitivityKey = "MouseSensitivity";

    [SerializeField] private float mouseSens = 0.1f;
    [SerializeField] private Transform player;
    [SerializeField] private float transitionSpeed = 20f;

    public float currentYaw;

    private float targetYaw;
    private float currentPitch;
    private float targetPitch;

    private PlayerInputActions playerInput;

    private void Awake()
    {
        playerInput = new PlayerInputActions();
        LoadSensitivity();
    }

    private void Start()
    {
        LockCursor();
    }

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.Player.Disable();
    }

    private void OnDestroy()
    {
        playerInput?.Dispose();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LoadSensitivity()
    {
        mouseSens = PlayerPrefs.GetFloat(SensitivityKey, 0.1f);
    }

    public void SetSensitivity(float value)
    {
        mouseSens = value;
    }

    private void Update()
    {
        // Don't rotate the camera while the game is paused.
        if (Time.timeScale == 0f)
            return;

        // Keep the cursor hidden during active gameplay.
        if (Cursor.lockState != CursorLockMode.Locked)
            LockCursor();

        Vector2 lookInput = playerInput.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSens;
        float mouseY = lookInput.y * mouseSens;

        targetYaw += mouseX;
        currentYaw = Mathf.Lerp(
            currentYaw, targetYaw, transitionSpeed * Time.deltaTime
        );

        targetPitch -= mouseY;
        targetPitch = Mathf.Clamp(targetPitch, -90f, 90f);

        currentPitch = Mathf.Lerp(
            currentPitch, targetPitch, transitionSpeed * Time.deltaTime
        );
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        transform.localRotation =
            Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}
