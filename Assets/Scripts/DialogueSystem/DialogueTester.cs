using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueSequence dialogueSequence;


    private void OnTriggerEnter(Collider other)
    {
        dialogueManager.PlayDialogue(dialogueSequence);
    }

    private void OnTriggerExit(Collider other)
    {
        this.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            dialogueManager.PlayDialogue(dialogueSequence);
        }
    }
}