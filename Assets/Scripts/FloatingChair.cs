
using UnityEngine;
using System.Collections;

public class SinkingChair : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform chair;

    [Header("Sink Settings")]
    [SerializeField] private float sinkDelay = 0.4f;
    [SerializeField] private float sinkSpeed = 1.5f;
    [SerializeField] private float sinkAcceleration = 2f;
    [SerializeField] private float maxSinkSpeed = 5f;

    [Header("Reset Settings")]
    [SerializeField] private float returnSpeed = 3f;

    private Vector3 initialPosition;
    private Coroutine movementRoutine;
    private bool playerOnChair;

    private void Awake()
    {
        initialPosition = chair.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerOnChair = true;

        if (movementRoutine != null)
            StopCoroutine(movementRoutine);

        movementRoutine = StartCoroutine(SinkChair());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerOnChair = false;

        if (movementRoutine != null)
            StopCoroutine(movementRoutine);

        movementRoutine = StartCoroutine(ReturnChair());
    }

    private IEnumerator SinkChair()
    {
        yield return new WaitForSeconds(sinkDelay);

        float currentSpeed = sinkSpeed;

        while (playerOnChair)
        {
            currentSpeed = Mathf.Min(
                currentSpeed + sinkAcceleration * Time.deltaTime,
                maxSinkSpeed
            );

            chair.position += Vector3.down * currentSpeed * Time.deltaTime;

            yield return null;
        }

        movementRoutine = null;
    }

    private IEnumerator ReturnChair()
    {
        while (Vector3.Distance(chair.position, initialPosition) > 0.01f)
        {
            chair.position = Vector3.MoveTowards(
                chair.position,
                initialPosition,
                returnSpeed * Time.deltaTime
            );

            yield return null;
        }

        chair.position = initialPosition;
        movementRoutine = null;
    }
}
