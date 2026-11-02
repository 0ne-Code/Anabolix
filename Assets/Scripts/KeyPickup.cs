
using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public bool IsCollected { get; private set; }

    [Header("Interaction")]
    [SerializeField] private string displayMessage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 1f;

    public string GetInteractionPrompt() => displayMessage;

    public void Interact()
    {
        if (IsCollected)
            return;

        IsCollected = true;

        Debug.Log("Key collected!");

        if (audioSource != null && pickupSFX != null)
            audioSource.PlayOneShot(pickupSFX, pickupVolume);

        gameObject.SetActive(false);
    }
}
