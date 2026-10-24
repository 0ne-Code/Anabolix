
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Menu")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                // Close Settings first if it is open.
                if (settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        settingsPanel.SetActive(false);

        isPaused = true;
        Time.timeScale = 0f;

        pausePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
