
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AudioClip footstepClip;

    [Header("Footstep Timing")]
    [SerializeField] private float walkingInterval = 0.5f;
    [SerializeField] private float sprintingInterval = 0.3f;

    [Header("Audio Settings")]
    [SerializeField] private float walkingPitch = 1f;
    [SerializeField] private float sprintingPitch = 1.1f;
    [SerializeField] private float jumpPitch = 1.2f;
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;

    private AudioSource audioSource;
    private float footstepTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Update()
    {
        if (playerController == null || footstepClip == null)
            return;

        if (playerController.ConsumeJustJumped())
        {
            audioSource.pitch = jumpPitch;
            audioSource.PlayOneShot(footstepClip, volume);
        }

        bool isMoving = playerController.IsMoving;
        bool isSprinting = playerController.IsSprinting;

        if (!playerController.IsGrounded || !isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        float interval = isSprinting
            ? sprintingInterval
            : walkingInterval;

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= interval)
        {
            audioSource.pitch = isSprinting
                ? sprintingPitch
                : walkingPitch;

            audioSource.PlayOneShot(footstepClip, volume);

            footstepTimer = 0f;
        }
    }
}
