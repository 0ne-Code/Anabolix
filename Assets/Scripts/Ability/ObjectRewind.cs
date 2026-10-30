
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectRewind : MonoBehaviour
{
    [Header("Recording")]
    [SerializeField, Min(1f)] private float historySeconds = 5f;

    [Header("Playback")]
    [SerializeField, Min(1f)] private float playbackSpeed = 1f;

    private struct ObjectState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public bool useGravity;
        public bool gravityInverted;
    }

    private Rigidbody rb;
    private AbilityTarget abilityTarget;
    private ObjectState[] history;

    private int writeIndex;
    private int historyCount;
    private int playbackIndex;
    private int playbackSamplesRemaining;
    private int playbackStep;

    private bool isRewinding;
    private bool previousIsKinematic;
    private ObjectState lastAppliedState;

    public bool IsRewinding => isRewinding;

    public bool CanRewind =>
        !isRewinding && historyCount > 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        abilityTarget = GetComponent<AbilityTarget>();

        int capacity = Mathf.Max(
            2,
            Mathf.CeilToInt(historySeconds / Time.fixedDeltaTime) + 1
        );

        history = new ObjectState[capacity];

        RecordState();
    }

    private void FixedUpdate()
    {
        if (isRewinding)
        {
            PlayBackward();
            return;
        }

        RecordState();
    }

    private ObjectState CaptureState()
    {
        return new ObjectState
        {
            position = rb.position,
            rotation = rb.rotation,
            velocity = rb.linearVelocity,
            angularVelocity = rb.angularVelocity,
            useGravity = rb.useGravity,
            gravityInverted = abilityTarget != null &&
                              abilityTarget.GravityInverted
        };
    }

    private void RecordState()
    {
        history[writeIndex] = CaptureState();

        writeIndex = (writeIndex + 1) % history.Length;
        historyCount = Mathf.Min(
            historyCount + 1,
            history.Length
        );
    }

    public void BeginRewind()
    {
        if (!CanRewind)
            return;

        isRewinding = true;

        previousIsKinematic = rb.isKinematic;
        rb.isKinematic = true;

        playbackIndex = (writeIndex - 1 + history.Length)
                        % history.Length;

        // Skip the latest sample: the object is already there.
        playbackSamplesRemaining = historyCount - 1;

        playbackStep = Mathf.Max(
            1,
            Mathf.RoundToInt(playbackSpeed)
        );

        // Move backward immediately.
        StepBackward();
    }

    private void PlayBackward()
    {
        for (int i = 0; i < playbackStep; i++)
        {
            if (!isRewinding)
                return;

            StepBackward();
        }
    }

    private void StepBackward()
    {
        if (playbackSamplesRemaining <= 0)
        {
            FinishRewind();
            return;
        }

        playbackIndex--;

        if (playbackIndex < 0)
            playbackIndex = history.Length - 1;

        lastAppliedState = history[playbackIndex];

        rb.position = lastAppliedState.position;
        rb.rotation = lastAppliedState.rotation;

        if (abilityTarget != null)
        {
            abilityTarget.RestoreGravityState(
                lastAppliedState.gravityInverted,
                lastAppliedState.useGravity
            );
        }
        else
        {
            rb.useGravity = lastAppliedState.useGravity;
        }

        playbackSamplesRemaining--;

        if (playbackSamplesRemaining <= 0)
            FinishRewind();
    }

    private void FinishRewind()
    {
        isRewinding = false;

        // Restore the Rigidbody's original physics mode.
        rb.isKinematic = previousIsKinematic;

        if (!previousIsKinematic)
        {
            rb.linearVelocity = lastAppliedState.velocity;
            rb.angularVelocity = lastAppliedState.angularVelocity;
        }

        // Discard old history so it cannot be replayed again.
        writeIndex = 0;
        historyCount = 0;
        RecordState();
    }

    private void OnDisable()
    {
        if (!isRewinding || rb == null)
            return;

        isRewinding = false;
        rb.isKinematic = previousIsKinematic;

        if (!previousIsKinematic)
        {
            rb.linearVelocity = lastAppliedState.velocity;
            rb.angularVelocity = lastAppliedState.angularVelocity;
        }
    }
}
