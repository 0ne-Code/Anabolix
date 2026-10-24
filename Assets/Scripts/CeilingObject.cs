
using UnityEngine;

public class CeilingObject : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBeenReleased = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "CeilingObject needs a Rigidbody on the same GameObject.",
                this
            );
            enabled = false;
            return;
        }

        // Keep the sofa attached to the ceiling initially.
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Release()
    {
        if (hasBeenReleased) return;

        hasBeenReleased = true;

        // Allow Unity physics to move the sofa.
        rb.isKinematic = false;
        rb.useGravity = true;

        Debug.Log("Ceiling object released: " + gameObject.name);
    }
}
