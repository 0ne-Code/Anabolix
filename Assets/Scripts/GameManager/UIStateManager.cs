
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject transitionCanvas;

    private void Start()
    {
        ShowPlaying();
    }

    public void ShowPlaying()
    {
        Time.timeScale = 1f;

        hudCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        transitionCanvas.SetActive(false);
    }

    public void ShowPaused()
    {
        hudCanvas.SetActive(true);
        menuCanvas.SetActive(true);
        transitionCanvas.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ShowFailed()
    {
        hudCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        transitionCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowCompleted()
    {
        hudCanvas.SetActive(false);
        menuCanvas.SetActive(false);
        transitionCanvas.SetActive(true);

        Time.timeScale = 0f;
    }
}
