
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingCabinet : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasStartedFalling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Keep the cabinet suspended until triggered.
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void StartFalling()
    {
        if (hasStartedFalling)
            return;

        hasStartedFalling = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        Debug.Log("Filing cabinet: FALLING");
    }
}
