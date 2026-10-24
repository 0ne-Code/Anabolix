
using UnityEngine;
using System.Collections;

public class TriggerTextDisplay : MonoBehaviour
{
    [SerializeField] private string message = "Something feels wrong...";
    [SerializeField] private float displayDuration = 5f;

    private bool hasTriggered;
    private bool showMessage;
    private GUIStyle textStyle;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !IsPlayer(other))
            return;

        hasTriggered = true;
        StartCoroutine(DisplayMessage());
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

    private IEnumerator DisplayMessage()
    {
        showMessage = true;
        yield return new WaitForSeconds(displayDuration);
        showMessage = false;
    }

    private void OnGUI()
    {
        if (!showMessage)
            return;

        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontSize = 30;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.normal.textColor = Color.white;
        }

        GUI.Label(
            new Rect(0, Screen.height * 0.4f, Screen.width, 80),
            message,
            textStyle
        );
    }
}
