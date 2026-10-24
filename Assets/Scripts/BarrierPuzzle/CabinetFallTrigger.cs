
using UnityEngine;

public class CabinetFallTrigger : MonoBehaviour
{
    [SerializeField] private FallingCabinet cabinet;

    private bool hasTriggered;

    private void Start()
    {
        Debug.Log("[Cabinet Trigger] Script is active.", this);

        if (cabinet == null)
        {
            Debug.LogError(
                "[Cabinet Trigger] Cabinet reference is MISSING!",
                this
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "[Cabinet Trigger] Something entered: " + other.name,
            other
        );

        Transform current = other.transform;

        // Also support player colliders on child objects.
        while (current != null && !current.CompareTag("Player"))
        {
            current = current.parent;
        }

        if (hasTriggered || current == null)
            return;

        if (cabinet == null)
        {
            Debug.LogError("[Cabinet Trigger] No cabinet assigned!");
            return;
        }

        hasTriggered = true;

        Debug.Log("[Cabinet Trigger] Player detected. Starting cabinet fall!");

        cabinet.StartFalling();
    }
}
