
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private float voidHeight = -20f;

    private bool isRestarting = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        // Restart the current level when the player falls into the void.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.transform.position.y <= voidHeight)
        {
            RestartLevel();
            return;
        }

        // Temporary testing shortcut: press R to restart.
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        if (isRestarting) return;

        isRestarting = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        isRestarting = false;
    }
}
