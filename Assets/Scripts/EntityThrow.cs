
using System.Collections;
using UnityEngine;

public class EntityThrow : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private Collider activationTrigger;

    [Header("Target")]
    [SerializeField] private Rigidbody boxRigidbody;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;

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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip entityAppearanceSound;
    [SerializeField, Range(0f, 1f)] private float appearanceVolume = 1f;

    private bool sequenceStarted;

    private void Awake()
    {
        if (activationTrigger == null)
        {
            Debug.LogError("EntityThrow: Activation Trigger is missing!", this);
            return;
        }

        if (!activationTrigger.isTrigger)
        {
            Debug.LogError("EntityThrow: Assigned Collider must have Is Trigger enabled!", this);
            return;
        }

        EntityActivationTrigger trigger =
            activationTrigger.GetComponent<EntityActivationTrigger>();

        if (trigger == null)
            trigger = activationTrigger.gameObject.AddComponent<EntityActivationTrigger>();

        trigger.Initialize(this);
    }

    public void ActivateSequence(Collider other)
    {
        if (sequenceStarted || !IsPlayer(other))
            return;

        if (boxRigidbody == null || playerController == null)
        {
            Debug.LogError("EntityThrow: Missing references!", this);
            return;
        }

        sequenceStarted = true;
        activationTrigger.enabled = false;

        if (audioSource != null && entityAppearanceSound != null)
            audioSource.PlayOneShot(entityAppearanceSound, appearanceVolume);

        playerController.SetMovementLocked(true);
        StartCoroutine(ThrowSequence());
    }

    private bool IsPlayer(Collider other)
    {
        Transform current = other.transform;

        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;

            current = current.parent;
        }

        return false;
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
        playerController.SetMovementLocked(false);
    }

    private void ThrowBox()
    {
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

public class EntityActivationTrigger : MonoBehaviour
{
    private EntityThrow entityThrow;

    public void Initialize(EntityThrow entity)
    {
        entityThrow = entity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entityThrow != null)
            entityThrow.ActivateSequence(other);
    }
}
