
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public enum Ability
    {
        GravityFlip,
        TimeFreeze,
        Rewind
    }

    [Header("Ability Selection")]
    [SerializeField] private Ability selectedAbility;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip abilitySwitchClip;

    private PlayerInputActions playerInput;

    public Ability SelectedAbility => selectedAbility;


    private void Awake()
    {
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.SelectGravity.performed += OnSelectGravity;
        playerInput.Player.SelectTime.performed += OnSelectTime;
        playerInput.Player.SelectRewind.performed += OnSelectRewind;

        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.Player.SelectGravity.performed -= OnSelectGravity;
        playerInput.Player.SelectTime.performed -= OnSelectTime;
        playerInput.Player.SelectRewind.performed -= OnSelectRewind;

        playerInput.Player.Disable();
    }

    private void OnDestroy()
    {
        playerInput?.Dispose();
        playerInput = null;
    }

    private void OnSelectGravity(InputAction.CallbackContext context)
    {
        SelectAbility(Ability.GravityFlip);
    }

    private void OnSelectTime(InputAction.CallbackContext context)
    {
        SelectAbility(Ability.TimeFreeze);
    }

    private void OnSelectRewind(InputAction.CallbackContext context)
    {
        SelectAbility(Ability.Rewind);
    }

    public void SelectAbility(Ability ability)
    {
        if (selectedAbility == ability)
            return;

        selectedAbility = ability;

        if (audioSource != null && abilitySwitchClip != null)
        {
            audioSource.PlayOneShot(abilitySwitchClip);
        }

        Debug.Log($"Selected Ability: {selectedAbility}");
    }
}
