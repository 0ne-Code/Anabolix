using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("Entity Dialogue")]
    [SerializeField] private string entityLine = "Where do you think you're going?";

    [Header("Next Level")]
    [SerializeField] private string nextSceneName = "Level_2";

    [Header("UI state manager")]
    [SerializeField] private UIStateManager uiStateManager;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        if (triggered)
            return;

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "LevelExitTrigger: SceneTransitionManager is missing!", this
            );
            return;
        }

        triggered = true;

        Debug.Log("Player entered the exit gate!");

        uiStateManager.ShowCompleted();

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