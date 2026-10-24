
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private string firstLevelScene = "Level_1";

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        mainButtons.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelScene);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    public void OpenSettings()
    {
        mainButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}
