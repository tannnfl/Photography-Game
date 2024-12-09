using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Screen Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f; // Duration of the shake
    [SerializeField] private float shakeMagnitude = 0.1f; // Intensity of the shake

    private Transform cameraTransform; // Reference to the camera's transform
    private Vector3 originalPosition; // Original position of the camera
    private Coroutine shakeCoroutine; // Reference to the active shake coroutine

    private void Start()
    {
        // Cache the camera transform
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Camera.main is not assigned. Make sure your main camera has the 'MainCamera' tag.");
        }

        // Store the camera's original position
        originalPosition = cameraTransform.localPosition;
    }

    /// <summary>
    /// Starts the screen shake with the configured duration and magnitude.
    /// </summary>
    public void TriggerShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shake(shakeDuration, shakeMagnitude));
    }

    /// <summary>
    /// Starts the screen shake with custom parameters.
    /// </summary>
    /// <param name="duration">Duration of the shake</param>
    /// <param name="magnitude">Intensity of the shake</param>
    public void TriggerShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;
        print("screenshake");

        while (elapsedTime < duration)
        {
            // Generate random offsets for the shake
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Apply the shake effect
            cameraTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the camera's original position
        cameraTransform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}
