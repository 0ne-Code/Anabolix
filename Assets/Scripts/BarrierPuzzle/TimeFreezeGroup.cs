using UnityEngine;

public class TimeFreezeGroup : MonoBehaviour
{
    private bool isFrozen;

    public bool IsFrozen => isFrozen;

    public void ToggleFreeze()
    {
        isFrozen = !isFrozen;

        Debug.Log(isFrozen
            ? "Time Freeze Group: ON"
            : "Time Freeze Group: OFF");
    }
}