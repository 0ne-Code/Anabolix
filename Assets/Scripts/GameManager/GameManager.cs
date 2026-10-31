using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private float voidHeight = -20f;

    private Transform player;
    private bool isRestarting;
    private PlayerInputActions playerInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        playerInput = new PlayerInputActions();
    }

    private void Start()
    {
        CachePlayer();
    }

    private void OnEnable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Restart.performed += OnRestart;
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Restart.performed -= OnRestart;
        playerInput.Player.Disable();
    }

    private void OnRestart(InputAction.CallbackContext context)
    {
        //if (Time.timeScale == 0f)
        //    return;

        RestartLevel();
    }

    private void Update()
    {
        if (isRestarting)
            return;

        if (player == null)
        {
            CachePlayer();
        }

        if (player != null && player.position.y <= voidHeight)
        {
            RestartLevel();
        }
    }

    private void CachePlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public void RestartLevel()
    {
        if (isRestarting)
            return;

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError(
                "GameManager: SceneTransitionManager is missing!",
                this
            );
            return;
        }

        if (SceneTransitionManager.Instance.ReloadCurrentScene())
        {
            isRestarting = true;
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.Dispose();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}