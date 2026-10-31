
using System;
using UnityEngine;

public enum DialogueSpeaker
{
    Player,
    Entity
}

[CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Dialogue/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public DialogueSpeaker speaker;

        [TextArea(2, 5)]
        public string dialogue;

        [Min(0.1f)]
        public float duration = 3f;
    }

    public DialogueLine[] lines;
}
