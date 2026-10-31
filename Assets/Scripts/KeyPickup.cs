
using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public bool IsCollected { get; private set; }
    [SerializeField] private string displayMessage;

    public string GetInteractionPrompt()
    {
        return displayMessage;
    }

    public void Interact()
    {
        IsCollected = true;

        Debug.Log("Key collected!");

        gameObject.SetActive(false);
    }
}
