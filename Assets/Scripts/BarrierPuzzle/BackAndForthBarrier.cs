using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BackAndForthBarrier : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;

    [Header("Linked Barrier")]
    [SerializeField] private BackAndForthBarrier linkedBarrier;

    private Rigidbody rb;
    private Transform destination;

    private bool isFrozen;

    public bool IsFrozen => isFrozen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.useGravity = false;
        destination = pointB;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null)
            return;

        // Stop this barrier while frozen.
        if (isFrozen)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 difference = destination.position - rb.position;
        float distance = difference.magnitude;

        // Reached destination.
        if (distance <= speed * Time.fixedDeltaTime)
        {
            rb.position = destination.position;

            destination = destination == pointA
                ? pointB
                : pointA;

            difference = destination.position - rb.position;
        }

        rb.linearVelocity = difference.normalized * speed;
    }

    public void ToggleFreeze()
    {
        SetFreezeState(!isFrozen, true);
    }

    private void SetFreezeState(bool frozen, bool notifyLinkedBarrier)
    {
        isFrozen = frozen;

        if (isFrozen)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Tell the other barrier to use the same state.
        // notifyLinkedBarrier prevents infinite A -> B -> A -> B...
        if (notifyLinkedBarrier && linkedBarrier != null)
        {
            linkedBarrier.SetFreezeState(frozen, false);
        }

        Debug.Log(
            gameObject.name + (isFrozen ? " FREEZED" : " UNFREEZED")
        );
    }
}