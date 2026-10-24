
using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("Entity Dialogue")]
    [SerializeField]
    private string entityLine = "Where do you think you're going?";

    [Header("Next Level")]
    [SerializeField]
    private string nextSceneName = "Level_2";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        // Support colliders attached to child objects of the Player.
        Transform player = other.transform;

        while (player != null && !player.CompareTag("Player"))
        {
            player = player.parent;
        }

        if (player == null) return;

        if (triggered) return;

        Debug.Log("Player entered the exit gate!");

        if (SceneTransitionManager.Instance == null) return;

        triggered = true;

        SceneTransitionManager.Instance.PlayLineAndLoadScene(
            entityLine,
            nextSceneName
        );
    }
}
