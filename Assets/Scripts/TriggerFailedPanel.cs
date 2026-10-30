using UnityEngine;

public class TriggerFailedPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject failedPanel;

    [Header("Settings")]
    [SerializeField] private bool stopGame = true;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool triggered;

    private void Start()
    {
        if (failedPanel == null)
        {
            Debug.LogError(
                "TriggerFailedPanel: FailedPanel is not assigned!", this
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        if (triggerOnlyOnce && triggered)
            return;

        if (failedPanel == null)
            return;

        triggered = true;
        failedPanel.SetActive(true);

        if (stopGame)
        {
            Time.timeScale = 0f;
        }
    }

    private bool IsPlayerCollider(Collider other)
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