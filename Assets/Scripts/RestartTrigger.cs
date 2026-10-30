using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
            return;
        }

        // Fallback for scenes without a GameManager.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
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