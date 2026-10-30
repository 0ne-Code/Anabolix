using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private float voidHeight = -20f;

    private Transform player;
    private bool isRestarting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CachePlayer();
    }

    private void Update()
    {
        // Retry only while the Player reference is unavailable.
        if (player == null)
        {
            CachePlayer();
        }

        if (player != null && player.position.y <= voidHeight)
        {
            RestartLevel();
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartLevel();
        }
    }

    private void CachePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public void RestartLevel()
    {
        if (isRestarting)
            return;

        isRestarting = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        // Do not reset isRestarting here.
        // This scene's GameManager will be destroyed during reloading.
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}