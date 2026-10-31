using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueSequence dialogueSequence;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            dialogueManager.PlayDialogue(dialogueSequence);
        }
    }
}