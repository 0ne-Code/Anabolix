
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

        // Make sure the fade overlay renders above the game.
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 999;
        }

        SetFadeAlpha(1f);
        SetDialogueAlpha(0f);
    }

    private IEnumerator Start()
    {
        // Fade into the first level when the game starts.
        yield return Fade(1f, 0f);
    }

    public void PlayLineAndLoadScene(string line, string nextScene)
    {
        if (transitionInProgress)
            return;

        StartCoroutine(TransitionRoutine(line, nextScene));
    }

    private IEnumerator TransitionRoutine(string line, string nextScene)
    {
        transitionInProgress = true;

        // Show the entity's line.
        dialogueText.text = line;
        SetDialogueAlpha(1f);

        yield return new WaitForSeconds(dialogueDuration);

        // Hide the dialogue and fade the screen to black.
        SetDialogueAlpha(0f);
        yield return Fade(0f, 1f);

        // Load the next scene while the screen is black.
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        if (operation == null)
        {
            Debug.LogError("Could not load scene: " + nextScene);
            transitionInProgress = false;
            yield return Fade(1f, 0f);
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        // Reveal the new level.
        yield return Fade(1f, 0f);

        transitionInProgress = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        SetFadeAlpha(startAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
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
    }

    private void SetDialogueAlpha(float alpha)
    {
        if (dialogueText == null)
            return;

        Color color = dialogueText.color;
        color.a = alpha;
        dialogueText.color = color;
    }
}
