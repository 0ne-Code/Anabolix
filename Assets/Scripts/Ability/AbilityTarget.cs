using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AbilityTarget : MonoBehaviour
{
    [Header("Supported Abilities")]
    [SerializeField] private bool canFlipGravity = true;
    [SerializeField] private bool canTimeFreeze = true;

    private Rigidbody rb;
    private bool gravityInverted;
    private bool timeFrozen;

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    // Reference to the special back-and-forth barrier behavior.
    private BackAndForthBarrier backAndForthBarrier;

    [Header("Phase Shift")]
    [SerializeField] private bool canPhaseShift = false;
    [SerializeField] private float phaseDuration = 3f;

    private Collider[] targetColliders;
    private bool[] originalColliderStates;
    private Coroutine phaseRoutine;

    // Special behavior for ceiling-mounted objects.
    private CeilingObject ceilingObject;

    public bool CanPhaseShift => canPhaseShift;
    public bool CanFlipGravity => canFlipGravity;
    public bool CanTimeFreeze => canTimeFreeze;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ceilingObject = GetComponent<CeilingObject>();
        backAndForthBarrier = GetComponent<BackAndForthBarrier>();

        targetColliders = GetComponentsInChildren<Collider>();
        originalColliderStates = new bool[targetColliders.Length];

        for (int i = 0; i < targetColliders.Length; i++)
        {
            originalColliderStates[i] = targetColliders[i].enabled;
        }

        // CeilingObject controls its own initial physics settings.
        if (ceilingObject == null)
        {
            rb.useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        // Normal Gravity Flip behavior.
        // Ceiling objects use their own release behavior instead.
        if (ceilingObject == null &&
            gravityInverted &&
            !timeFrozen)
        {
            rb.AddForce(Vector3.up * 9.81f, ForceMode.Acceleration);
        }
    }

    public void FlipGravity()
    {
        if (!canFlipGravity || timeFrozen)
            return;

        // Release the sofa if this is a ceiling-mounted object.
        if (ceilingObject != null)
        {
            ceilingObject.Release();
            return;
        }

        gravityInverted = !gravityInverted;
        rb.useGravity = !gravityInverted;

        Debug.Log(gravityInverted
            ? "Gravity reversed: UP"
            : "Gravity restored: DOWN");
    }

    public void ToggleTimeFreeze()
    {
        if (!canTimeFreeze)
            return;

        // If this is one of the linked barriers,
        // let the barrier handle freezing both barriers.
        if (backAndForthBarrier != null)
        {
            backAndForthBarrier.ToggleFreeze();
            return;
        }

        // Normal AbilityTarget objects freeze individually.
        if (!timeFrozen)
        {
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;

            rb.isKinematic = true;
            timeFrozen = true;

            Debug.Log("Time Freeze: ON");
        }
        else
        {
            rb.isKinematic = false;

            rb.linearVelocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;

            timeFrozen = false;

            Debug.Log("Time Freeze: OFF");
        }
    }

    public void PhaseShift()
    {
        if (!canPhaseShift || phaseRoutine != null)
            return;

        phaseRoutine = StartCoroutine(PhaseShiftRoutine());
    }

    private IEnumerator PhaseShiftRoutine()
    {
        foreach (Collider targetCollider in targetColliders)
        {
            targetCollider.enabled = false;
        }

        Debug.Log("Phase Shift: ON");

        yield return new WaitForSeconds(phaseDuration);

        for (int i = 0; i < targetColliders.Length; i++)
        {
            targetColliders[i].enabled = originalColliderStates[i];
        }

        Debug.Log("Phase Shift: OFF");

        phaseRoutine = null;
    }
}