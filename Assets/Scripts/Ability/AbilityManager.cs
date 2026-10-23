
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public enum Ability
    {
        GravityFlip,
        TimeFreeze,
        PhaseShift
    }

    [Header("Ability Selection")]
    [SerializeField] private Ability selectedAbility;

    private PlayerInputActions playerInput;

    public Ability SelectedAbility => selectedAbility;

    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();

        playerInput.Player.SelectGravity.performed +=
            _ => SelectAbility(Ability.GravityFlip);

        playerInput.Player.SelectTime.performed +=
            _ => SelectAbility(Ability.TimeFreeze);

        playerInput.Player.SelectPhase.performed +=
            _ => SelectAbility(Ability.PhaseShift);
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }

    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
        Debug.Log("Selected Ability: " + selectedAbility);
    }
}
