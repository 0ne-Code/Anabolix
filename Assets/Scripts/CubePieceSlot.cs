
using UnityEngine;

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

    private bool completionTriggered;

    public string GetInteractionPrompt()
    {
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

        return "";
    }

    public void Interact()
    {
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

        Debug.Log("All cube pieces placed! Final dialogue starting.");

        if (dialogueManager != null && finalDialogue != null)
        {
            dialogueManager.PlayDialogue(finalDialogue);
        }
        else
        {
            Debug.LogError("DialogueManager or Final Dialogue is missing!");
        }
    }
}
