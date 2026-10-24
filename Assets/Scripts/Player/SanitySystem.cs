
using UnityEngine;
using UnityEngine.InputSystem;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;

    [Header("Sanity Drain")]
    [SerializeField] private float sanityDrainPerSecond = 1f;

    public float CurrentSanity => currentSanity;
    public float MaxSanity => maxSanity;
    public bool IsGameOver { get; private set; }

    private PlayerInputActions playerInput;

    public void ChangeSanity(float amount)
    {
        if (IsGameOver) return;

        currentSanity = Mathf.Clamp(
            currentSanity + amount,
            0f,
            maxSanity
        );

        if (currentSanity <= 0f)
        {
            currentSanity = 0f;
            IsGameOver = true;
            Debug.Log("GAME OVER - Sanity depleted!");
        }
    }

    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void OnDestroy()
    {
        playerInput.Dispose();
    }

    private void Update()
    {
        // Drain sanity only during active gameplay.
        if (!IsGameOver && Time.timeScale > 0f)
        {
            ChangeSanity(sanityDrainPerSecond * -Time.deltaTime);
        }

        // Testing keys.
        if (Keyboard.current == null) return;

        if (Keyboard.current.kKey.wasPressedThisFrame)
            ChangeSanity(-10f);

        if (Keyboard.current.lKey.wasPressedThisFrame)
            ChangeSanity(10f);
    }
}
