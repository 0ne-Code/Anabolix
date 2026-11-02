
using UnityEngine;

public class AbilityTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraEffects cameraEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (player == null)
            return;

        // Check whether the collider belongs to the assigned player.
        if (other.gameObject != player.gameObject &&
            other.GetComponentInParent<PlayerController>() != player)
            return;

        player.EnablePlayerGravityMode();

        Debug.Log("PLAYER GRAVITY MODE ACTIVATED!");

        if (cameraEffects != null)
            cameraEffects.ShakeCamera();
    }
}
