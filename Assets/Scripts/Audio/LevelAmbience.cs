
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelAmbience : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private AudioClip ambienceClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.4f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.clip = ambienceClip;
        audioSource.volume = volume;
    }

    private void Start()
    {
        if (ambienceClip != null)
            audioSource.Play();
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
