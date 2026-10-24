
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerWalkingSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Rigidbody playerRb;

    [Header("Settings")]
    [SerializeField] private float movementThreshold = 0.15f;

    private void Awake()
    {
        if (playerRb == null)
            playerRb = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError(
                "PlayerWalkingSound: Add an AudioSource to the Player.",
                this
            );
            enabled = false;
            return;
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        bool isMoving =
            new Vector3(
                playerRb.linearVelocity.x,
                0f,
                playerRb.linearVelocity.z
            ).magnitude > movementThreshold;

        bool isGrounded =
            Physics.Raycast(
                transform.position + Vector3.up * 0.1f,
                Vector3.down,
                1.2f
            );

        if (isMoving && isGrounded && Time.timeScale > 0f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
