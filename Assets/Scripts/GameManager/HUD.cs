
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Sanity UI")]
    [SerializeField] private Slider sanityBar;
    [SerializeField] private SanitySystem sanitySystem;

    private void Awake()
    {
        if (sanityBar == null)
            Debug.LogError("HUD: Sanity Bar is not assigned.", this);

        if (sanitySystem == null)
            Debug.LogError("HUD: Sanity System is not assigned.", this);
    }

    private void Update()
    {
        if (sanityBar == null || sanitySystem == null)
            return;

        sanityBar.maxValue = sanitySystem.MaxSanity;
        sanityBar.value = sanitySystem.CurrentSanity;
    }
}
