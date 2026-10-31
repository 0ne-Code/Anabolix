using System.Collections;
using UnityEngine;

public class BoxGateTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody targetBox;
    [SerializeField] private Transform gate;

    [Header("Gate Settings")]
    [SerializeField] private float openHeight = 4f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private Coroutine gateRoutine;
    private bool isOpen;

    private void Awake()
    {
        closedPosition = gate.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != targetBox)
            return;

        SetGateState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != targetBox)
            return;

        SetGateState(false);
    }

    private void SetGateState(bool open)
    {
        if (isOpen == open)
            return;

        isOpen = open;

        if (gateRoutine != null)
            StopCoroutine(gateRoutine);

        gateRoutine = StartCoroutine(MoveGate(
            open ? openPosition : closedPosition
        ));
    }

    private IEnumerator MoveGate(Vector3 targetPosition)
    {
        while (Vector3.Distance(gate.position, targetPosition) > 0.01f)
        {
            gate.position = Vector3.MoveTowards(
                gate.position,
                targetPosition,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        gate.position = targetPosition;
        gateRoutine = null;
    }
}