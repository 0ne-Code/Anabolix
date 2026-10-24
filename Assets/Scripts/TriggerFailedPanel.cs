
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
        Debug.Log(
            "Trigger entered by: " + other.name, this
        );

        Transform player = other.transform;

        if (!player.CompareTag("Player") &&
            (player.root == null || !player.root.CompareTag("Player")))
        {
            Debug.Log("Trigger ignored: object is not tagged Player.", this);
            return;
        }

        if (triggerOnlyOnce && triggered)
            return;

        if (failedPanel == null)
            return;

        triggered = true;

        failedPanel.SetActive(true);
        Debug.Log("FailedPanel activated by trigger!", this);

        if (stopGame)
        {
            Time.timeScale = 0f;
        }
    }
}
    