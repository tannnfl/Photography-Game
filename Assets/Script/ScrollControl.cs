using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect; // Reference to the Scroll Rect
    [SerializeField] private float scrollSpeed = 0.1f; // Speed of scrolling with mouse/keyboard
    [SerializeField] private float smoothScrollDuration = 0.2f; // Smooth scroll duration

    private Coroutine smoothScrollCoroutine;

    private void Update()
    {
        HandleMouseScroll();
        HandleKeyboardInput();
    }

    // Handle mouse scroll wheel input
    private void HandleMouseScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Detect mouse wheel input
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            float targetPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + scrollInput * scrollSpeed, 0f, 1f);
            SmoothScrollTo(targetPosition, smoothScrollDuration);
        }
    }

    // Handle keyboard input (W/S or Arrow Keys)
    private void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            float targetPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + scrollSpeed * Time.deltaTime, 0f, 1f);
            SmoothScrollTo(targetPosition, smoothScrollDuration);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            float targetPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollSpeed * Time.deltaTime, 0f, 1f);
            SmoothScrollTo(targetPosition, smoothScrollDuration);
        }
    }

    // Smooth scrolling to a target position
    public void SmoothScrollTo(float targetPosition, float duration)
    {
        if (smoothScrollCoroutine != null)
        {
            StopCoroutine(smoothScrollCoroutine);
        }

        smoothScrollCoroutine = StartCoroutine(ScrollCoroutine(targetPosition, duration));
    }

    private IEnumerator ScrollCoroutine(float targetPosition, float duration)
    {
        float start = scrollRect.verticalNormalizedPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, targetPosition, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPosition;
    }
}
