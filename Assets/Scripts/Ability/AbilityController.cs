
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AbilityManager abilityManager;

    [Header("Settings")]
    [SerializeField, Min(0f)] private float abilityRange = 10f;
    [SerializeField] private LayerMask abilityLayers = Physics.DefaultRaycastLayers;

    private PlayerInputActions playerInput;

    private void Awake()
    {
        playerInput = new PlayerInputActions();

        if (playerCamera == null)
            Debug.LogError("AbilityController: Player Camera is not assigned.", this);

        if (abilityManager == null)
            Debug.LogError("AbilityController: Ability Manager is not assigned.", this);
    }

    private void OnEnable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Attack.performed += OnAbilityUsed;
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.Attack.performed -= OnAbilityUsed;
        playerInput.Player.Disable();
    }

    private void OnDestroy()
    {
        playerInput?.Dispose();
        playerInput = null;
    }

    private void OnAbilityUsed(InputAction.CallbackContext context)
    {
        if (playerCamera == null || abilityManager == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                abilityRange,
                abilityLayers,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        AbilityTarget target =
            hit.collider.GetComponentInParent<AbilityTarget>();

        if (target == null)
            return;

        switch (abilityManager.SelectedAbility)
        {
            case AbilityManager.Ability.GravityFlip:
                if (target.CanFlipGravity)
                    target.FlipGravity();
                break;

            case AbilityManager.Ability.TimeFreeze:
                if (target.CanTimeFreeze)
                    target.ToggleTimeFreeze();
                break;

            case AbilityManager.Ability.Rewind:
                if (target.CanRewind)
                    target.Rewind();
                break;
        }
    }
}
