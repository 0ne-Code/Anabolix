using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntityAudio : MonoBehaviour
{
    [Header("Entity Audio")]
    [SerializeField] private AudioClip appearanceSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnEnable()
    {
        if (audioSource != null && appearanceSound != null)
        {
            audioSource.PlayOneShot(appearanceSound);
        }
    }
}