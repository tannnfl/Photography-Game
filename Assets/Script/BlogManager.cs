using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlogManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject blogUI; // Reference to the Blog UI
    [SerializeField] private Transform contentArea; // Content area of the scroll view
    [SerializeField] private GameObject postPrefab; // Prefab for a single post

    private List<(Texture2D photo, string description)> capturedPhotos = new List<(Texture2D, string)>(); // Reference to the photo list

    /// <summary>
    /// Populates the blog UI with photos and descriptions.
    /// </summary>
    public void PopulateBlog()
    {
        // Clear existing posts
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Get the latest 12 photos, in reverse order (latest first)
        int maxPosts = Mathf.Min(12, capturedPhotos.Count);
        for (int i = capturedPhotos.Count - 1; i >= capturedPhotos.Count - maxPosts; i--)
        {
            print(capturedPhotos[i].photo + capturedPhotos[i].description);
            AddPost(capturedPhotos[i].photo, capturedPhotos[i].description);
        }
    }

    /// <summary>
    /// Adds a single post to the blog UI.
    /// </summary>
    /// <param name="photo">The photo for the post</param>
    /// <param name="description">The description for the post</param>
    private void AddPost(Texture2D photo, string description)
    {
        // Instantiate the post prefab
        GameObject newPost = Instantiate(postPrefab, contentArea);

        // Set the photo in the Image component
        Image photoImage = newPost.transform.Find("Image").GetComponent<Image>();
        if (photoImage != null)
        {
            photoImage.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        }
        print(1);
        // Set the description in the Text component
        Text descriptionText = newPost.transform.Find("Description").GetComponent<Text>();
        print(2);
        if (descriptionText != null)
        {
            print(3);
            descriptionText.text = description;
            print(4);
        }
        print("add post finished, now return");
    }

    /// <summary>
    /// Updates the captured photos list and refreshes the blog UI.
    /// </summary>
    /// <param name="photos">The list of captured photos and descriptions</param>
    public void UpdatePhotoList(List<(Texture2D photo, string description)> photos)
    {
        capturedPhotos = photos;
        PopulateBlog();
    }
}

