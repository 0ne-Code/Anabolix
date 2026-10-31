using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueSequence dialogueSequence;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !IsPlayer(other))
            return;

        if (dialogueManager == null || dialogueSequence == null)
        {
            Debug.LogError("DialogueTrigger: Missing dialogue references!", this);
            return;
        }

        dialogueManager.PlayDialogue(dialogueSequence);
        triggered = true;
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
}