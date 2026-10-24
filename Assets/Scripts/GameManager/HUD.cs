
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Sanity UI")]
    [SerializeField] private Slider sanityBar;
    [SerializeField] private SanitySystem sanitySystem;

    private void Update()
    {
        sanityBar.maxValue = sanitySystem.MaxSanity;
        sanityBar.value = sanitySystem.CurrentSanity;
    }
}
