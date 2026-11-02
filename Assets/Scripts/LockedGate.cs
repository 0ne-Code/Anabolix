
using UnityEngine;
using System.Collections;

public class LockedGate : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private KeyPickup keyPickup;
    [SerializeField] private Transform gate;

    [Header("Gate Settings")]
    [SerializeField] private float openHeight = 4f;
    [SerializeField] private float openSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSFX;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen;

    private void Awake()
    {
        closedPosition = gate.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    public string GetInteractionPrompt()
    {
        return keyPickup != null && keyPickup.IsCollected
            ? "Press E to use key"
            : "Key required";
    }

    public void Interact()
    {
        if (isOpen)
            return;

        if (keyPickup == null || !keyPickup.IsCollected)
        {
            Debug.Log("You need the key!");
            return;
        }

        isOpen = true;

        if (audioSource != null && unlockSFX != null)
            audioSource.PlayOneShot(unlockSFX);

        StartCoroutine(OpenGate());
    }

    private IEnumerator OpenGate()
    {
        while (Vector3.Distance(gate.position, openPosition) > 0.01f)
        {
            gate.position = Vector3.MoveTowards(
                gate.position,
                openPosition,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        gate.position = openPosition;
    }
}
