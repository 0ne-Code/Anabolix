
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SanitySystem sanitySystem;
    [SerializeField] private GameObject failedPanel;

    private bool gameOverShown;

    private void Awake()
    {
        if (failedPanel != null)
            failedPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (sanitySystem == null)
        {
            Debug.LogError("GameOverUI: SanitySystem is not assigned.", this);
            return;
        }

        sanitySystem.GameOver += ShowFailedScreen;

        if (sanitySystem.IsGameOver)
            ShowFailedScreen();
    }

    private void OnDisable()
    {
        if (sanitySystem != null)
            sanitySystem.GameOver -= ShowFailedScreen;
    }

    private void ShowFailedScreen()
    {
        if (gameOverShown)
            return;

        if (failedPanel == null)
        {
            Debug.LogError("GameOverUI: Failed Panel is not assigned.", this);
            return;
        }

        gameOverShown = true;
        failedPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}