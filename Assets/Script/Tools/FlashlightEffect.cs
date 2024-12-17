using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class FlashlightEffect : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light2D flashlightLight; // The 2D Light component
    [SerializeField] private float flashDuration = 1.0f; // Total duration of the flash
    [SerializeField] private AnimationCurve intensityCurve; // Optional curve to control the intensity

    private void Awake()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponentInChildren<Light2D>();
            if (flashlightLight == null)
            {
                Debug.LogError("No Light2D found. Attach a Light2D component to the camera or a child object.");
                enabled = false;
            }
        }
        flashlightLight.intensity = 0f; // Ensure light starts at 0 intensity
    }

    /// <summary>
    /// Triggers the flashlight effect.
    /// </summary>
    /// <param name="maxIntensity">Maximum intensity of the flash.</param>
    public void TriggerFlashlight(float FOV)
    {
        if (flashlightLight == null) return; // Ensure the light exists
        StartCoroutine(FlashLightCoroutine(FOV));
    }

    private void setIntensity(float fallOffIntensity)
    {
        flashlightLight.falloffIntensity = fallOffIntensity;
    }

    private IEnumerator FlashLightCoroutine(float FOV)
    {
        float maxIntensity = FOV;
        float halfDuration = flashDuration / 2f; // Split duration for increase and decrease
        float elapsedTime = 0f;

        // Increase intensity
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            // Use curve if defined, otherwise linear interpolation
            flashlightLight.intensity = intensityCurve != null ?
                intensityCurve.Evaluate(t) * maxIntensity : Mathf.Lerp(0, maxIntensity, t);

            yield return null;
        }

        elapsedTime = 0f;

        // Decrease intensity
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            flashlightLight.intensity = intensityCurve != null ?
                intensityCurve.Evaluate(1 - t) * maxIntensity : Mathf.Lerp(maxIntensity, 0, t);

            yield return null;
        }

        flashlightLight.intensity = 0f; // Ensure light is off at the end
    }
}
