
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private IInteractable currentInteractable;

    [SerializeField] private InteractionUI interactionUI;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void Update()
    {
        if (currentInteractable == null)
            return;

        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            interactionUI.ShowPrompt(interactable.GetInteractionPrompt());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (currentInteractable == interactable)
            {
                currentInteractable = null;
                interactionUI.HidePrompt();
            }
        }
    }
}
