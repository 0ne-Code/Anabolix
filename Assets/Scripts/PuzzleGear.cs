
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PuzzleGear : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    private Rigidbody rb;
    private bool isFrozen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isFrozen)
            return;

        float angle = rotationSpeed * Time.fixedDeltaTime;

        Quaternion deltaRotation = Quaternion.AngleAxis(
            angle,
            rotationAxis.normalized
        );

        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }
}
