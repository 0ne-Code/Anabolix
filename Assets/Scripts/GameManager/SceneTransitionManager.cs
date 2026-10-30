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
            canvas.sortingOrder = 999;
        }

        SetFadeAlpha(1f);
        SetDialogueAlpha(0f);
    }

    private IEnumerator Start()
    {
        yield return Fade(1f, 0f);
    }

    public void PlayLineAndLoadScene(string line, string nextScene)
    {
        if (transitionInProgress)
            return;

        if (string.IsNullOrWhiteSpace(nextScene))
        {
            Debug.LogError("SceneTransitionManager: Next scene is empty!", this);
            return;
        }

        StartCoroutine(TransitionRoutine(line, nextScene));
    }

    private IEnumerator TransitionRoutine(string line, string nextScene)
    {
        transitionInProgress = true;

        if (dialogueText != null)
        {
            dialogueText.text = line;
            SetDialogueAlpha(1f);
        }

        yield return new WaitForSecondsRealtime(
            Mathf.Max(0f, dialogueDuration)
        );

        SetDialogueAlpha(0f);

        yield return Fade(0f, 1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        if (operation == null)
        {
            Debug.LogError(
                "Could not load scene: " + nextScene, this
            );

            transitionInProgress = false;
            yield return Fade(1f, 0f);
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        yield return Fade(1f, 0f);

        transitionInProgress = false;
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