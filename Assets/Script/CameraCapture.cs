using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // For file handling

public class CameraCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private RectTransform photoCaptureArea; // Define the boundary for photo capture
    [SerializeField] private Image photoDisplayArea; // The UI Image to display the captured photo
    [SerializeField] private GameObject photoFrame; // The UI Frame for displaying photos

    [Header("Save Settings")]
    [SerializeField] private string saveFolderName = "CapturedPhotos"; // Folder name inside Assets to save photos

    private Texture2D screenCapture; // Stores the captured photo
    private bool viewingPhoto; // Tracks whether the player is currently viewing a photo

    private void Start()
    {
        // Create the folder inside the Assets directory if it doesn't exist
        string folderPath = Path.Combine(Application.dataPath, saveFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"Created folder at: {folderPath}");
        }

        // Ensure the photo frame is hidden at the start
        if (photoFrame != null)
            photoFrame.SetActive(false);
    }

    private void Update()
    {
        // Left mouse button captures or toggles the photo view
        if (Input.GetMouseButtonDown(0))
        {
            if (!viewingPhoto)
                StartCoroutine(CapturePhoto());
            else
                RemovePhoto();
        }
    }

    IEnumerator CapturePhoto()
    {
        viewingPhoto = true;

        // Wait until the end of the current frame to capture the screen
        yield return new WaitForEndOfFrame();

        // Capture only the area within the photoCaptureArea
        Vector3[] worldCorners = new Vector3[4];
        photoCaptureArea.GetWorldCorners(worldCorners);

        Rect captureRegion = new Rect(
            worldCorners[0].x,
            worldCorners[0].y,
            worldCorners[2].x - worldCorners[0].x,
            worldCorners[2].y - worldCorners[0].y
        );

        // Render the specified region
        screenCapture = new Texture2D((int)captureRegion.width, (int)captureRegion.height, TextureFormat.RGB24, false);
        screenCapture.ReadPixels(captureRegion, 0, 0);
        screenCapture.Apply();

        // Save the photo to the Assets folder
        SavePhoto(screenCapture);

        ShowPhoto();
    }

    void ShowPhoto()
    {
        // Convert the captured texture into a sprite and assign it to the UI Image
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        // Display the photo frame
        if (photoFrame != null)
            photoFrame.SetActive(true);
    }

    void RemovePhoto()
    {
        // Close the photo view
        viewingPhoto = false;

        if (photoFrame != null)
            photoFrame.SetActive(false);
    }

    private void SavePhoto(Texture2D photo)
    {
        // Generate a unique filename
        string fileName = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string folderPath = Path.Combine(Application.dataPath, saveFolderName);
        string filePath = Path.Combine(folderPath, fileName);

        // Save the photo as a PNG file
        byte[] photoBytes = photo.EncodeToPNG();
        File.WriteAllBytes(filePath, photoBytes);

        Debug.Log($"Photo saved to: {filePath}");

        // Refresh the Asset Database to make the saved file appear in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}