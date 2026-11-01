using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private float resetHeight = -10f;
    [SerializeField] private float resetSpeed = 5f;

    private Rigidbody rb;
    private AbilityTarget abilityTarget;
    private CeilingObject ceilingObject;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private bool resetting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        abilityTarget = GetComponent<AbilityTarget>();
        ceilingObject = GetComponent<CeilingObject>();

        startingPosition = rb.position;
        startingRotation = rb.rotation;
    }

    private void FixedUpdate()
    {
        if (!resetting && rb.position.y <= resetHeight)
        {
            StartReset();
        }

        if (!resetting)
            return;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            startingPosition,
            resetSpeed * Time.fixedDeltaTime
        );

        Quaternion newRotation = Quaternion.RotateTowards(
            rb.rotation,
            startingRotation,
            360f * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
        rb.MoveRotation(newRotation);

        if (Vector3.Distance(rb.position, startingPosition) < 0.01f)
        {
            rb.position = startingPosition;
            rb.rotation = startingRotation;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (ceilingObject != null)
            {
                ceilingObject.ResetToCeiling();
            }
            else
            {
                rb.isKinematic = false;
            }

            resetting = false;
        }
    }

    private void StartReset()
    {
        resetting = true;

        if (abilityTarget != null)
            abilityTarget.ResetGravityState();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}