using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float dialogueDuration = 2.5f;

    private bool transitionInProgress;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Canvas canvas = GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.sortingOrder = 10;
        }

        SetFadeAlpha(1f);
        SetDialogueAlpha(0f);
    }

    private IEnumerator Start()
    {
        // The first scene also enters through the shared fade system.
        transitionInProgress = true;

        yield return null;

        Time.timeScale = 1f;

        yield return Fade(1f, 0f);

        transitionInProgress = false;
    }

    // Use this for loading any scene without dialogue.
    public void LoadSceneWithTransition(string sceneName)
    {
        StartTransition(sceneName, null);
    }

    // Use this for level completion with entity dialogue.
    public void PlayLineAndLoadScene(string line, string nextScene)
    {
        StartTransition(nextScene, line);
    }

    // Use this for restarting the current level.
    public bool ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return StartTransition(currentScene, null);
    }

    private bool StartTransition(string sceneName, string line)
    {
        if (transitionInProgress)
            return false;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "SceneTransitionManager: Scene name is empty!",
                this
            );
            return false;
        }

        transitionInProgress = true;
        StartCoroutine(TransitionRoutine(sceneName, line));

        return true;
    }

    private IEnumerator TransitionRoutine(string sceneName, string line)
    {
        // Keep transitions functional even if gameplay is paused.
        Time.timeScale = 1f;

        // Show dialogue only when a line was provided.
        if (!string.IsNullOrEmpty(line))
        {
            if (dialogueText != null)
            {
                dialogueText.text = line;
                SetDialogueAlpha(1f);
            }

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0f, dialogueDuration)
            );

            SetDialogueAlpha(0f);
        }

        // Fade out the current scene.
        yield return Fade(0f, 1f);

        // Load the requested scene while the screen is black.
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError(
                "SceneTransitionManager: Could not load scene: " +
                sceneName,
                this
            );

            Time.timeScale = 1f;
            yield return Fade(1f, 0f);

            transitionInProgress = false;
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        // Allow the new scene's objects and UI to initialize.
        yield return null;

        Time.timeScale = 1f;

        // Fade into the newly loaded scene.
        yield return Fade(1f, 0f);

        transitionInProgress = false;
        Debug.Log("Transition finished. Lock released.");
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float duration = Mathf.Max(0f, fadeDuration);
        float elapsed = 0f;

        SetFadeAlpha(startAlpha);

        if (duration <= 0f)
        {
            SetFadeAlpha(endAlpha);
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            SetFadeAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;

        // Only block clicks while the screen is actually fading.
        fadeImage.raycastTarget = alpha > 0.01f;
    }

    private void SetDialogueAlpha(float alpha)
    {
        if (dialogueText == null)
            return;

        Color color = dialogueText.color;
        color.a = alpha;
        dialogueText.color = color;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}