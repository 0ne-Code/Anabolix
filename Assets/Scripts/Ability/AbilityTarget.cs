
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AbilityTarget : MonoBehaviour
{
    [Header("Supported Abilities")]
    [SerializeField] private bool canFlipGravity = true;
    [SerializeField] private bool canTimeFreeze = true;
    [SerializeField] private bool canRewind;

    private Rigidbody rb;
    [SerializeField] private CeilingObject ceilingObject;
    [SerializeField] private BackAndForthBarrier backAndForthBarrier;
    [SerializeField] private ObjectRewind objectRewind;
    [SerializeField] private PuzzleGear puzzleGear;

    private bool gravityInverted;
    public bool GravityInverted => gravityInverted;
    private bool timeFrozen;

    private bool originalUseGravity;
    private bool originalIsKinematic;

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    public bool CanFlipGravity => canFlipGravity;
    public bool CanTimeFreeze => canTimeFreeze;

    public bool CanRewind =>
        canRewind &&
        !timeFrozen &&
        objectRewind != null &&
        objectRewind.CanRewind;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ceilingObject = GetComponent<CeilingObject>();
        backAndForthBarrier = GetComponent<BackAndForthBarrier>();
        objectRewind = GetComponent<ObjectRewind>();

        originalUseGravity = rb.useGravity;
        originalIsKinematic = rb.isKinematic;

        // Preserve the ceiling object's custom physics setup.
        if (ceilingObject == null)
            rb.useGravity = true;

        if (canRewind && objectRewind == null)
        {
            Debug.LogWarning(
                $"AbilityTarget on {name} has Rewind enabled, " +
                "but no ObjectRewind component is attached.",
                this
            );
        }
    }

    private void FixedUpdate()
    {
        if (ceilingObject == null &&
            gravityInverted &&
            !timeFrozen &&
            (objectRewind == null || !objectRewind.IsRewinding))
        {
            rb.AddForce(Vector3.up * 9.81f, ForceMode.Acceleration);
        }
    }

    public void FlipGravity()
    {
        if (!canFlipGravity || timeFrozen)
            return;

        if (ceilingObject != null)
        {
            ceilingObject.Release();
            return;
        }

        gravityInverted = !gravityInverted;
        rb.useGravity = gravityInverted ? false : originalUseGravity;
    }

    public void ToggleTimeFreeze()
    {
        if (!canTimeFreeze)
            return;

        if (backAndForthBarrier != null)
        {
            backAndForthBarrier.ToggleFreeze();
            return;
        }

        if (puzzleGear != null)
        {
            timeFrozen = !timeFrozen;
            puzzleGear.SetFrozen(timeFrozen);
            return;
        }

        if (!timeFrozen)
        {
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;
            originalIsKinematic = rb.isKinematic;

            rb.isKinematic = true;
            timeFrozen = true;
        }
        else
        {
            rb.isKinematic = originalIsKinematic;

            if (!originalIsKinematic)
            {
                rb.linearVelocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;
            }

            timeFrozen = false;
        }
    }

    public void Rewind()
    {
        if (objectRewind == null)
        {
            Debug.LogWarning("No ObjectRewind component found.", this);
            return;
        }

        if (!objectRewind.CanRewind)
            return;

        objectRewind.BeginRewind();
    }

    public void RestoreGravityState(bool inverted, bool useGravity)
    {
        gravityInverted = inverted;
        rb.useGravity = useGravity;
    }
}
