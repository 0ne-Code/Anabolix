
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AbilityManager abilityManager;

    [Header("Settings")]
    [SerializeField] private float abilityRange = 10f;

    private PlayerInputActions playerInput;


    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Attack.performed += OnAbilityUsed;
    }

    private void OnDisable()
    {
        playerInput.Player.Attack.performed -= OnAbilityUsed;
        playerInput.Player.Disable();
        playerInput.Dispose();
    }

    private void OnAbilityUsed(InputAction.CallbackContext context)
    {
        Debug.Log("1. Ability input detected!");

        if (playerCamera == null || abilityManager == null)
        {
            Debug.LogError("2. Player Camera or Ability Manager is missing!");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, abilityRange))
        {
            Debug.Log("3. Nothing hit by raycast.");
            return;
        }

        Debug.Log("3. Raycast hit: " + hit.collider.name);

        AbilityTarget target =
            hit.collider.GetComponentInParent<AbilityTarget>();

        if (target == null)
        {
            Debug.LogWarning("4. Hit object has no AbilityTarget component.");
            return;
        }

        Debug.Log("4. Valid target found. Selected ability: "
            + abilityManager.SelectedAbility);

        switch (abilityManager.SelectedAbility)
        {
            case AbilityManager.Ability.GravityFlip:
                if (target.CanFlipGravity)
                {
                    target.FlipGravity();
                }
                else
                {
                    Debug.Log("This target cannot be gravity flipped.");
                }
                break;

            case AbilityManager.Ability.TimeFreeze:
                if (target.CanTimeFreeze)
                {
                    target.ToggleTimeFreeze();
                }
                else
                {
                    Debug.Log("This target cannot be time frozen.");
                }
                break;

            case AbilityManager.Ability.PhaseShift:
                if (target.CanPhaseShift) target.PhaseShift();
                else Debug.Log("This target cannot be phase shifted.");
                break;
        }
    }
}
