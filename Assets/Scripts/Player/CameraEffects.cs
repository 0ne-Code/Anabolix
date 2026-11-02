
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("Head Bob - Walking")]
    [SerializeField] private float walkFrequency = 8f;
    [SerializeField] private float walkAmplitude = 0.04f;

    [Header("Head Bob - Sprinting")]
    [SerializeField] private float sprintFrequency = 11f;
    [SerializeField] private float sprintAmplitude = 0.065f;

    [Header("Smoothing")]
    [SerializeField] private float bobSmoothSpeed = 8f;

    [Header("Field Of View")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float normalFOV = 75f;
    [SerializeField] private float sprintFOV = 85f;
    [SerializeField] private float fovSmoothSpeed = 5f;

    [Header("Idle Breathing")]
    [SerializeField] private float breathingAmplitude = 0.012f;
    [SerializeField] private float breathingFrequency = 1.2f;
    [SerializeField] private float breathingSmoothSpeed = 3f;
    [SerializeField] private float idleThreshold = 0.1f;

    [Header("Jump & Landing Feedback")]
    [SerializeField] private float jumpCameraOffset = 0.025f;
    [SerializeField] private float landingCameraOffset = 0.07f;
    [SerializeField] private float impactReturnSpeed = 10f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.35f;
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeReturnSpeed = 12f;

    private float breathingTimer;
    private float breathingOffset;
    private float impactOffset;

    private float shakeTimer;
    private Vector3 shakeOffset;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;

    private Vector3 startingPosition;
    private float bobTimer;

    private void Start()
    {
        startingPosition = transform.localPosition;
        playerCamera.fieldOfView = normalFOV;
    }

    private void Update()
    {
        HandleHeadBob();
        HandleJumpAndLanding();
        HandleCameraShake();
        HandleFOV();
    }

    private void HandleHeadBob()
    {
        float horizontalSpeed = new Vector3(
            playerRb.linearVelocity.x,
            0f,
            playerRb.linearVelocity.z
        ).magnitude;

        bool isMoving =
            horizontalSpeed > idleThreshold &&
            playerController.IsGrounded;

        bool isSprinting =
            isMoving && playerController.IsSprinting;

        float targetOffset = 0f;

        if (isMoving)
        {
            float frequency = isSprinting
                ? sprintFrequency
                : walkFrequency;

            float amplitude = isSprinting
                ? sprintAmplitude
                : walkAmplitude;

            bobTimer += Time.deltaTime * frequency;

            targetOffset = Mathf.Sin(bobTimer) * amplitude;

            breathingTimer = 0f;
        }
        else
        {
            breathingTimer += Time.deltaTime * breathingFrequency;

            targetOffset = Mathf.Sin(breathingTimer) *
                           breathingAmplitude;
        }

        float smoothSpeed = isMoving
            ? bobSmoothSpeed
            : breathingSmoothSpeed;

        breathingOffset = Mathf.Lerp(
            breathingOffset,
            targetOffset,
            smoothSpeed * Time.deltaTime
        );

        UpdateCameraPosition();
    }

    private void HandleJumpAndLanding()
    {
        if (playerController.ConsumeJustJumped())
        {
            impactOffset = jumpCameraOffset;
        }

        if (playerController.ConsumeJustLanded())
        {
            impactOffset = -landingCameraOffset;
        }

        impactOffset = Mathf.Lerp(
            impactOffset,
            0f,
            impactReturnSpeed * Time.deltaTime
        );

        UpdateCameraPosition();
    }

    private void HandleCameraShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            shakeOffset = Random.insideUnitSphere * shakeIntensity;
        }
        else
        {
            shakeOffset = Vector3.Lerp(
                shakeOffset,
                Vector3.zero,
                shakeReturnSpeed * Time.deltaTime
            );
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        transform.localPosition = startingPosition +
            Vector3.up * (breathingOffset + impactOffset) +
            shakeOffset;
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }

    private void HandleFOV()
    {
        bool isSprinting =
            playerController.IsGrounded &&
            playerController.IsSprinting;

        float targetFOV = isSprinting
            ? sprintFOV
            : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovSmoothSpeed * Time.deltaTime
        );
    }
}
