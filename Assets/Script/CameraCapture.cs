using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;

    private Texture2D screenCapture;
    private bool viewingPhoto;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!viewingPhoto)
                StartCoroutine(CapturePhoto());
            else
                RemovePhoto();
        }
    }

    IEnumerator CapturePhoto()
    {
        viewingPhoto = true;

        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
    }

    void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        photoFrame.SetActive(true);
    }

    void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        //camera UI to true;
    }
}
