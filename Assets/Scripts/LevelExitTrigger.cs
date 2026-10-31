using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("Entity Dialogue")]
    [SerializeField]
    private string entityLine =
        "Where do you think you're going?";

    [Header("Next Level")]
    [SerializeField] private string nextSceneName = "Level_2";

    [Header("UI State Manager")]
    [SerializeField] private UIStateManager uiStateManager;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !IsPlayerCollider(other))
            return;

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "LevelExitTrigger: SceneTransitionManager is missing!",
                this
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "LevelExitTrigger: Next scene name is empty!",
                this
            );
            return;
        }

        triggered = true;

        Debug.Log("Player entered the exit gate!");

        if (uiStateManager != null)
        {
            uiStateManager.ShowCompleted();
        }

        SceneTransitionManager.Instance.PlayLineAndLoadScene(
            entityLine,
            nextSceneName
        );
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