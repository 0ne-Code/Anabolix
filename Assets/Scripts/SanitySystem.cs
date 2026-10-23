using UnityEngine;
using UnityEngine.InputSystem;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 50f;

    public float CurrentSanity => currentSanity;
    public float MaxSanity => maxSanity;
    public bool IsGameOver { get; private set; }

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
            IsGameOver = true;
            Debug.Log("GAME OVER - Sanity depleted!");
        }
    }


    //
    //Testing
    //
    private PlayerInputActions playerInput;

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

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.kKey.wasPressedThisFrame)
            ChangeSanity(-10f);

        if (Keyboard.current.lKey.wasPressedThisFrame)
            ChangeSanity(10f);
    }
}
