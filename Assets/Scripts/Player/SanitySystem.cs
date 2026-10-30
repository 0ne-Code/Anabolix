
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity")]
    [SerializeField, Min(0f)] private float maxSanity = 100f;
    [SerializeField, Min(0f)] private float currentSanity = 100f;

    [Header("Sanity Drain")]
    [SerializeField, Min(0f)] private float sanityDrainPerSecond = 1f;

    public float CurrentSanity => currentSanity;
    public float MaxSanity => maxSanity;
    public bool IsGameOver { get; private set; }

    public event Action GameOver;

    private void Awake()
    {
        maxSanity = Mathf.Max(0f, maxSanity);
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        if (currentSanity <= 0f)
            SetGameOver();
    }

    public void ChangeSanity(float amount)
    {
        if (IsGameOver || amount == 0f)
            return;

        float newSanity = Mathf.Clamp(
            currentSanity + amount,
            0f,
            maxSanity
        );

        if (Mathf.Approximately(newSanity, currentSanity))
            return;

        currentSanity = newSanity;

        if (currentSanity <= 0f)
            SetGameOver();
    }

    private void Update()
    {
        if (!IsGameOver && Time.timeScale > 0f)
        {
            ChangeSanity(-sanityDrainPerSecond * Time.deltaTime);
        }

        // Temporary testing shortcuts.
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.kKey.wasPressedThisFrame)
            ChangeSanity(-10f);

        if (Keyboard.current.lKey.wasPressedThisFrame)
            ChangeSanity(10f);
    }

    private void SetGameOver()
    {
        if (IsGameOver)
            return;

        currentSanity = 0f;
        IsGameOver = true;

        Debug.Log("GAME OVER - Sanity depleted!");
        GameOver?.Invoke();
    }
}