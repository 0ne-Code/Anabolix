
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI continueHint;

    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    [Header("Sanity Reference")]
    [SerializeField] private SanitySystem sanitySystem;

    [Header("Entity Voice")]
    [SerializeField] private AudioSource entityAudioSource;
    [SerializeField] private AudioClip entityVoiceClip;
    [SerializeField, Range(0f, 1f)] private float entityVoiceVolume = 0.7f;

    private DialogueSequence currentSequence;
    private Coroutine dialogueRoutine;
    private bool skipRequested;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (dialogueRoutine == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            skipRequested = true;
        }
    }

    public void PlayDialogue(DialogueSequence sequence)
    {
        if (sequence == null || sequence.lines == null ||
            sequence.lines.Length == 0)
            return;

        if (dialogueRoutine != null)
            return;

        currentSequence = sequence;

        dialoguePanel.SetActive(true);

        if (continueHint != null)
            continueHint.gameObject.SetActive(true);

        if (playerController != null)
            playerController.SetMovementLocked(true);

        if (sanitySystem != null)
            sanitySystem.SetDialogueState(true);

        dialogueRoutine = StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        foreach (DialogueSequence.DialogueLine line in currentSequence.lines)
        {
            speakerName.text = line.speaker.ToString();
            dialogueText.text = line.dialogue;

            if (line.speaker == DialogueSpeaker.Entity &&
            entityAudioSource != null &&
            entityVoiceClip != null)
            {
                entityAudioSource.PlayOneShot(entityVoiceClip, entityVoiceVolume);
            }

            skipRequested = false;

            float elapsedTime = 0f;

            while (elapsedTime < line.duration && !skipRequested)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        dialoguePanel.SetActive(false);

        if (playerController != null)
            playerController.SetMovementLocked(false);

        if (sanitySystem != null)
            sanitySystem.SetDialogueState(false);

        currentSequence = null;
        dialogueRoutine = null;
    }
}
