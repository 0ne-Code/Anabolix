
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    private const string SensitivityKey = "MouseSensitivity";

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private MouseLook mouseLook;

    private void Start()
    {
        if (sensitivitySlider == null)
        {
            Debug.LogError("Sensitivity Slider is not assigned!", this);
            return;
        }

        sensitivitySlider.minValue = 0.01f;
        sensitivitySlider.maxValue = 0.5f;

        float savedSensitivity =
            PlayerPrefs.GetFloat(SensitivityKey, 0.1f);

        sensitivitySlider.SetValueWithoutNotify(savedSensitivity);

        if (mouseLook != null)
            mouseLook.SetSensitivity(savedSensitivity);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        Debug.Log("SLIDER VALUE: " + value);

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        Debug.Log("SAVED VALUE: " +
                  PlayerPrefs.GetFloat(SensitivityKey));

        if (mouseLook != null)
        {
            mouseLook.SetSensitivity(value);
            Debug.Log("MouseLook reference FOUND. Value sent: " + value);
        }
        else
        {
            Debug.LogError("MouseLook reference is MISSING!");
        }
    }

    private void OnDestroy()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
    }
}
