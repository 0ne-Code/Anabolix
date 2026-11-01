
using UnityEngine;
using System.Collections;

public class CubePieceSlot : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class PieceSlot
    {
        public KeyPickup requiredPiece;
        public Transform snapPoint;
        public Transform pieceVisual;

        [HideInInspector]
        public bool isPlaced;
    }

    [Header("Missing Pieces")]
    [SerializeField] private PieceSlot[] pieces;

    [Header("Final Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueSequence finalDialogue;

    [Header("Door")]
    [SerializeField] private Transform door;
    [SerializeField] private float doorOpenHeight = 4f;
    [SerializeField] private float doorOpenSpeed = 2f;

    private Vector3 doorClosedPosition;
    private Vector3 doorOpenPosition;

    private bool completionTriggered;

    private void Awake()
    {
        if (door != null)
        {
            doorClosedPosition = door.position;
            doorOpenPosition = doorClosedPosition + Vector3.up * doorOpenHeight;
        }
    }

    public string GetInteractionPrompt()
    {
        if (completionTriggered)
            return "";

        foreach (PieceSlot piece in pieces)
        {
            if (piece == null || piece.isPlaced)
                continue;

            if (piece.requiredPiece != null &&
                piece.requiredPiece.IsCollected)
            {
                return "Press E to place the missing piece";
            }
        }

        return "Find the missing pieces";
    }

    public void Interact()
    {
        if (completionTriggered)
            return;

        foreach (PieceSlot piece in pieces)
        {
            if (piece == null || piece.isPlaced)
                continue;

            if (piece.requiredPiece != null &&
                piece.requiredPiece.IsCollected)
            {
                PlacePiece(piece);
                return;
            }
        }

        Debug.Log("Find the missing pieces!");
    }

    private void PlacePiece(PieceSlot piece)
    {
        if (piece.snapPoint == null || piece.pieceVisual == null)
            return;

        piece.isPlaced = true;

        piece.pieceVisual.SetParent(piece.snapPoint, false);
        piece.pieceVisual.localPosition = Vector3.zero;
        piece.pieceVisual.localRotation = Quaternion.identity;
        piece.pieceVisual.gameObject.SetActive(true);

        Debug.Log("Missing piece placed!");

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (completionTriggered)
            return;

        foreach (PieceSlot piece in pieces)
        {
            if (piece == null || !piece.isPlaced)
                return;
        }

        completionTriggered = true;

        Debug.Log("All cube pieces placed!");

        if (door != null)
            StartCoroutine(OpenDoor());

        if (dialogueManager != null && finalDialogue != null)
            dialogueManager.PlayDialogue(finalDialogue);
        else
            Debug.LogError("DialogueManager or Final Dialogue is missing!");
    }

    private IEnumerator OpenDoor()
    {
        while (Vector3.Distance(door.position, doorOpenPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(
                door.position,
                doorOpenPosition,
                doorOpenSpeed * Time.deltaTime
            );

            yield return null;
        }

        door.position = doorOpenPosition;
    }
}
