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

    [Header("UI State")]
    [SerializeField] private UIStateManager uiStateManager;

    private bool isPaused;
    private PlayerInputActions playerInput;

    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void Start()
    {
        if (pausePanel == null || settingsPanel == null)
        {
            Debug.LogError(
                "PauseMenuManager: Assign both UI panels!", this
            );
            enabled = false;
            return;
        }

        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Pause.performed += OnPause;
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Pause.performed -= OnPause;
        playerInput.Player.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else if (settingsPanel.activeSelf)
        {
            CloseSettings();
        }
        else
        {
            ResumeGame();
        }
    }

    private void OnDestroy()
    {
        playerInput?.Dispose();
    }

    //private void Update()
    //{
    //    if (Keyboard.current == null ||
    //        !Keyboard.current.escapeKey.wasPressedThisFrame)
    //    {
    //        return;
    //    }

    //    if (!isPaused)
    //    {
    //        PauseGame();
    //    }
    //    else if (settingsPanel.activeSelf)
    //    {
    //        CloseSettings();
    //    }
    //    else
    //    {
    //        ResumeGame();
    //    }
    //}

    public void PauseGame()
    {
        if (pausePanel == null || settingsPanel == null || isPaused)
            return;

        settingsPanel.SetActive(false);

        isPaused = true;

        uiStateManager.ShowPaused();

        pausePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void ResumeGame()
    {
        if (pausePanel == null || settingsPanel == null)
            return;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);

        isPaused = false;

        uiStateManager.ShowPlaying();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        if (!isPaused || pausePanel == null || settingsPanel == null)
            return;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (pausePanel == null || settingsPanel == null)
            return;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuScene))
        {
            Debug.LogError(
                "PauseMenuManager: Main menu scene is empty!",
                this
            );
            return;
        }

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "PauseMenuManager: SceneTransitionManager is missing!",
                this
            );
            return;
        }

        SceneTransitionManager.Instance.LoadSceneWithTransition(
            mainMenuScene
        );
    }
}