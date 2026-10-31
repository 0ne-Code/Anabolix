using System.Collections;
using UnityEngine;

public class EntityThrow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Rigidbody boxRigidbody;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Timing")]
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float throwDelay = 0.5f;
    [SerializeField] private float disappearDelay = 1f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 8f;
    [SerializeField] private float upwardForce = 2f;

    private bool sequenceStarted;

    private void Start()
    {
        StartCoroutine(ThrowSequence());
    }

    private IEnumerator ThrowSequence()
    {
        yield return new WaitForSeconds(startDelay);

        while (Vector3.Distance(transform.position, boxRigidbody.position) > stopDistance)
        {
            Vector3 direction = boxRigidbody.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();

                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.forward = direction;
            }

            yield return null;
        }

        yield return new WaitForSeconds(throwDelay);

        ThrowBox();

        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);
    }

    private void ThrowBox()
    {
        if (boxRigidbody == null)
        {
            Debug.LogError("EntityThrow: Box Rigidbody is missing!", this);
            return;
        }

        Vector3 direction = boxRigidbody.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        Vector3 force = direction * throwForce;
        force.y += upwardForce;

        boxRigidbody.AddForce(force, ForceMode.Impulse);
    }
}