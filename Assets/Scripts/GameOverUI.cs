
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SanitySystem sanitySystem;
    [SerializeField] private GameObject failedPanel;

    private bool gameOverShown;

    private void Start()
    {
        if (failedPanel != null)
            failedPanel.SetActive(false);
    }

    private void Update()
    {
        if (sanitySystem == null || failedPanel == null)
            return;

        if (sanitySystem.IsGameOver && !gameOverShown)
        {
            ShowFailedScreen();
        }
    }

    private void ShowFailedScreen()
    {
        gameOverShown = true;

        failedPanel.SetActive(true);

        // Stop gameplay.
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        // Restore time before reloading.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
