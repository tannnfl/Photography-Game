using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public enum CameraMode { Photography, Exploration }
    public CameraMode currentMode = CameraMode.Exploration;

    [Header("Camera Move")]
    public float speed;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Camera Zoom")]
    public CinemachineVirtualCamera virtualCamera;
    public GameObject backgroundBounds; // Assign your background in the inspector
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    public float currentZoom;

    [Header("UI")]
    public GameObject cameraOverlayUI; // The camera overlay
    public TextMeshProUGUI modeIndicatorText; // Text for mode indication
    public Button switchModeButton; // Optional button for switching modes
    [SerializeField] private GameObject blogUI;
    [SerializeField] private BlogManager blogManager;
    private bool isBlogVisible = false;

    [Header("Camera Capture")]
    public GameObject cameraCapture; // The CameraCapture GameObject to disable in Exploration mode

    [Header("Dialogue UI")]
    public GameObject worldCanvas;
    [SerializeField] private GameObject GPP;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private float cameraWidth;
    private float cameraHeight;

    private Vector3 originalCameraPosition; // Store the original camera position
    private float originalCameraZoom; // Store the original zoom level

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GPP.SetActive(false);

        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera not assigned!");
            return;
        }

        // Store the original camera position and zoom
        originalCameraPosition = transform.position;
        originalCameraZoom = virtualCamera.m_Lens.OrthographicSize;

        currentZoom = originalCameraZoom;

        if (backgroundBounds != null)
        {
            CalculateZoomLimits();
            CalculateMovementBounds();
        }
        blogUI.SetActive(false);

        // Set up UI
        UpdateUI();

        if (switchModeButton != null)
        {
            switchModeButton.onClick.AddListener(() =>
            {
                if (currentMode == CameraMode.Photography)
                {
                    SwitchMode(CameraMode.Exploration);
                }
                else
                {
                    SwitchMode(CameraMode.Photography);
                }
            });
        }
    }

    private void Update()
    {
        if (!cameraCapture.GetComponent<CameraCapture>().canMove()) return;

        // ***** When any camera move is enabled *****
        //move camera
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        // Toggle between Photography and Exploration mode
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentMode == CameraMode.Photography)
            {
                SwitchMode(CameraMode.Exploration);
            }
            else
            {
                SwitchMode(CameraMode.Photography);
            }
        }
        // Toggle the Blog UI when "B" is pressed
        
        // Hide the Blog UI when "Escape" or mouse button is pressed
        if (isBlogVisible && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)))
        {
            HideBlogUI();
        }
        // ***** When any camera move is enabled *****
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (currentMode != CameraMode.Photography)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBlogUI();
            }
        }
        else
        {
            
            if (scrollInput != 0)
            {
                currentZoom -= scrollInput * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                // Smooth transition to the target zoom
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                    virtualCamera.m_Lens.OrthographicSize,
                    currentZoom,
                    Time.deltaTime * zoomSpeed
                );

                // Update bounds after zooming
                CalculateMovementBounds();
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentMode == CameraMode.Photography || currentMode == CameraMode.Exploration)
        {
            // Move the camera
            Vector2 newPosition = rb.position + movement * speed * Time.deltaTime;

            // Clamp the position within the bounds
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            rb.MovePosition(newPosition);
        }
    }

    private void CalculateZoomLimits()
    {
        Renderer renderer = backgroundBounds.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float screenAspect = (float)Screen.width / (float)Screen.height;

            // Calculate orthographic size to fit height and width of bounds
            float boundsHeight = bounds.size.y / 2f;
            float boundsWidth = bounds.size.x / 2f / screenAspect;

            //maxZoom = Mathf.Min(boundsHeight, boundsWidth);
        }
    }

    private void CalculateMovementBounds()
    {
        if (backgroundBounds == null || virtualCamera == null) return;

        Renderer renderer = backgroundBounds.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;

            // Calculate the camera's size based on its orthographic size
            cameraHeight = virtualCamera.m_Lens.OrthographicSize;
            cameraWidth = cameraHeight * Screen.width / Screen.height;

            // Calculate movement boundaries based on the background bounds
            minBounds = new Vector2(bounds.min.x + cameraWidth, bounds.min.y + cameraHeight);
            maxBounds = new Vector2(bounds.max.x - cameraWidth, bounds.max.y - cameraHeight);
        }
    }

    public void SwitchMode(CameraMode newMode)
    {
        currentMode = newMode;

        if (currentMode == CameraMode.Exploration)
        {
            // Reset camera position and zoom
            //transform.position = originalCameraPosition;

            // Smoothly reset zoom
            StartCoroutine(ResetZoom());

            // Disable CameraCapture functionality in Exploration Mode
            if (cameraCapture != null)
                cameraCapture.SetActive(false);

            if (worldCanvas != null)
                cameraCapture.SetActive(true);

            speed = 20;
            HideBlogUI();
            GPP.SetActive(false);
        }
        else if (currentMode == CameraMode.Photography)
        {
            // Set a fixed zoom-in value for Photography Mode
            float fixedPhotographyZoom = 10f; // Adjust this value as needed
            currentZoom = fixedPhotographyZoom;

            // Smoothly apply the fixed zoom
            StartCoroutine(ApplyZoom(fixedPhotographyZoom));

            // Enable CameraCapture functionality in Photography Mode
            if (cameraCapture != null)
                cameraCapture.SetActive(true);
            if (worldCanvas != null)
                cameraCapture.SetActive(false);
            speed = 15;
            HideBlogUI();
            GPP.SetActive(true);
        }

        // Update the UI
        UpdateUI();
    }

    private IEnumerator ResetZoom()
    {
        float startZoom = virtualCamera.m_Lens.OrthographicSize;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, originalCameraZoom, t);
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = originalCameraZoom;
    }

    private void UpdateUI()
    {
        if (cameraOverlayUI != null)
        {
            cameraOverlayUI.SetActive(currentMode == CameraMode.Photography);
        }

        if (modeIndicatorText != null)
        {
            modeIndicatorText.text = $"Mode: {currentMode}";
        }
    }

    private IEnumerator ApplyZoom(float targetZoom)
    {
        if (targetZoom > maxZoom) targetZoom = maxZoom;
        if (targetZoom < minZoom) targetZoom = minZoom;

        float startZoom = virtualCamera.m_Lens.OrthographicSize;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetZoom;
    }

    private void ToggleBlogUI()
    {
        if (blogUI == null) return;

        isBlogVisible = !isBlogVisible;
        blogUI.SetActive(isBlogVisible);
        blogManager.PopulateBlog();
    }
    private void HideBlogUI()
    {
        if (blogUI == null) return;

        isBlogVisible = false;
        blogUI.SetActive(false);
    }
    public void ShowBlogUI()
    {
        if (blogUI == null) return;

        isBlogVisible = true;
        blogUI.SetActive(true);
    }
}
