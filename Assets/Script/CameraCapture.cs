using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CameraCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private RectTransform photoFrameBG; // The Photo Frame Background to move
    [SerializeField] private RectTransform photoCaptureArea; // Define the boundary for photo capture
    [SerializeField] private Image photoDisplayArea; // The UI Image to display the captured photo
    [SerializeField] private GameObject photoFrame; // The UI Frame for displaying photos
    [SerializeField] private FlashlightEffect flashlightEffect;
    public GameObject OnCamera;

    [Header("Save Settings")]
    [SerializeField] private string saveFolderName = "CapturedPhotos"; // Folder name inside Assets to save photos
    [SerializeField] private CameraMovement CameraMovement;

    [Header("User Input")]
    [SerializeField] private TMP_InputField descriptionInputField; // TextMeshPro input field for user description
    [SerializeField] private TextMeshProUGUI currentTextDisplay; // TextMeshPro UI to show dynamic text

    [Header("Tools")]
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private GameObject BloomPostProcessingVolume;
    BlogManager blogManager;

    

    

    private List<(Texture2D photo, string description, string time, int views)> capturedPhotos = new List<(Texture2D, string, string, int)>();

    private Vector2 originalPosition; // Original position of the photoFrameBG
    private Vector2 centerPosition; // Center position of the screen

    private Texture2D screenCapture; // Stores the captured photo
    //private bool viewingPhoto = false; // Tracks whether the player is currently viewing a photo
    //private bool waitingForDescription = false; // Tracks if the system is waiting for user input
    private bool isProcessingInput = false; // New flag to prevent repeated input calls

    public enum CMode
    {
        TakePhoto, ViewPhoto
    }
    public CMode currentCMode = CMode.TakePhoto;

    private void Start()
    {
        blogManager = FindObjectOfType<BlogManager>();
        blogManager.UpdatePhotoList(capturedPhotos);

        // Initialize photo frame positions
        originalPosition = photoFrameBG.anchoredPosition;
        centerPosition = new Vector2(0f, 30f);

        // Hide UI initially
        if (photoFrame != null) photoFrame.SetActive(false);
        descriptionInputField.gameObject.SetActive(false);
        currentTextDisplay.gameObject.SetActive(false);

        // Create folder for captured photos
        string folderPath = Path.Combine(Application.dataPath, saveFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"Created folder at: {folderPath}");
        }
        UpdatePhotocount();
    }

    private void Update()
    {
        //test
        //if (Input.GetKey(KeyCode.P)) ShowPhoto(capturedPhotos[0].photo);
        

        if (isProcessingInput)
        {
            return; // Prevent repeated processing during transitions
            print("is processing input");
        }

        if (CameraMovement.currentMode == CameraMovement.CameraMode.Exploration)
        {
            RemovePhoto();
            return;
        }

        // take picture
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && currentCMode == CMode.TakePhoto)
        {

            switchMode(CMode.ViewPhoto);
        }
    }

    private void switchMode(CMode newCMode)
    {
        if (newCMode == currentCMode) return;
        switch (newCMode)
        {
            case CMode.ViewPhoto:
                StartCoroutine(CapturePhoto());
                break;
            case CMode.TakePhoto:
                descriptionInputField.gameObject.SetActive(false);
                currentTextDisplay.gameObject.SetActive(false);
                RemovePhoto();
                break;
        }
        currentCMode = newCMode;
        UpdatePhotocount();
    }

    IEnumerator CapturePhoto()
    {
        isProcessingInput = true; // Prevent further inputs
        BloomPostProcessingVolume.SetActive(false);
        
        //viewingPhoto = true;
        flashlightEffect.TriggerFlashlight(4f);
        screenShake.TriggerShake();
        yield return new WaitForSeconds(0.1f);
        OnCamera.SetActive(false);
        yield return new WaitForEndOfFrame();

        // Capture the photo
        Vector3[] worldCorners = new Vector3[4];
        photoCaptureArea.GetWorldCorners(worldCorners);
        Rect captureRegion = new Rect(worldCorners[0].x, worldCorners[0].y, worldCorners[2].x - worldCorners[0].x, worldCorners[2].y - worldCorners[0].y);
        

        screenCapture = new Texture2D((int)captureRegion.width, (int)captureRegion.height, TextureFormat.RGB24, false);
        screenCapture.ReadPixels(captureRegion, 0, 0);
        screenCapture.Apply();

        OnCamera.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        ShowPhoto();
        UpdatePhotocount();
        yield return new WaitForSeconds(0.5f);
        ShowDescriptionInput(); 
        BloomPostProcessingVolume.SetActive(true);
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        if (photoFrame != null) photoFrame.SetActive(true);
        StartCoroutine(MoveToPosition(photoFrameBG, centerPosition, 0.5f));
    }
    void ShowPhoto(Texture2D _photo)
    {
        Sprite photoSprite = Sprite.Create(_photo, new Rect(0.0f, 0.0f, _photo.width, _photo.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        if (photoFrame != null) photoFrame.SetActive(true);
        StartCoroutine(MoveToPosition(photoFrameBG, centerPosition, 0.5f));
    }
    void ShowDescriptionInput()
    {
        // Enable description input
        descriptionInputField.gameObject.SetActive(true);
        currentTextDisplay.gameObject.SetActive(true);
        descriptionInputField.text = string.Empty;
        currentTextDisplay.text = string.Empty;

        //descriptionInputField.onValueChanged.AddListener(UpdateDynamicText);
        descriptionInputField.onSubmit.AddListener(SaveDescription);

        isProcessingInput = false; // Allow inputs again
    }
    
    void SaveDescription(string description)
    {
        capturedPhotos.Add((screenCapture, description, System.DateTime.Now.ToString("hh:mm:ss tt"),CalculateViews()));
        //SavePhoto(screenCapture, description);

        descriptionInputField.onSubmit.RemoveListener(SaveDescription);
        switchMode(CMode.TakePhoto);
        //waitingForDescription = false;
    }
    
    void RemovePhoto()
    {
        isProcessingInput = true; // Prevent repeated inputs
        //viewingPhoto = false;
        if (photoFrame != null) photoFrame.SetActive(false);
        StartCoroutine(MoveToPosition(photoFrameBG, originalPosition, 0.5f));
        isProcessingInput = false; // Allow inputs again
    }

    //tool
    private void SavePhoto(Texture2D photo, string description)
    {
        string fileName = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string folderPath = Path.Combine(Application.dataPath, saveFolderName);
        string filePath = Path.Combine(folderPath, fileName);

        byte[] photoBytes = photo.EncodeToPNG();
        File.WriteAllBytes(filePath, photoBytes);

        Debug.Log($"Photo saved to: {filePath} with description: {description}");

        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

        blogManager.UpdatePhotoList(capturedPhotos);
    }

    //general small tools
    IEnumerator MoveToPosition(RectTransform rectTransform, Vector2 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }


    public  bool canMove()
    {
        return currentCMode == CMode.TakePhoto;
    }

    private int CalculateViews()
    {
        return Mathf.FloorToInt(Random.Range(0, 100));
    }

    private void UpdatePhotocount()
    {
        OnCamera.transform.Find("PhotoCount").GetComponent<TextMeshProUGUI>().text = capturedPhotos.Count.ToString() + " / 12";
        if (capturedPhotos.Count == 12) TriggerEnding();
    }

    private void TriggerEnding()
    {

    }
}
