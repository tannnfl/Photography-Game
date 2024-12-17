using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlogManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject blogUI; // Reference to the Blog UI
    [SerializeField] private Transform contentArea; // Content area of the scroll view
    [SerializeField] private GameObject postPrefab; // Prefab for a single post
    public GameObject noPost;

    private List<(Texture2D photo, string description, string time, int views)> capturedPhotos = new List<(Texture2D, string, string, int)>(); // Reference to the photo list

    /// <summary>
    /// Populates the blog UI with photos and descriptions.
    /// </summary>
    public void PopulateBlog()
    {
        if(capturedPhotos.Count != 0)
        {
            noPost.GetComponent<TextMeshProUGUI>().enabled = false;
        }
        else
        {
            noPost.GetComponent<TextMeshProUGUI>().enabled = true;
        }
        // Clear existing posts
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Get the latest 12 photos, in reverse order (latest first)
        int maxPosts = Mathf.Min(12, capturedPhotos.Count);
        for (int i = capturedPhotos.Count - 1; i >= capturedPhotos.Count - maxPosts; i--)
        {
            AddPost(capturedPhotos[i].photo, capturedPhotos[i].description, capturedPhotos[i].time, capturedPhotos[i].views);
        }
    }

    /// <summary>
    /// Adds a single post to the blog UI.
    /// </summary>
    /// <param name="photo">The photo for the post</param>
    /// <param name="description">The description for the post</param>
    private void AddPost(Texture2D photo, string description, string time, int views)
    {
        // Instantiate the post prefab
        GameObject newPost = Instantiate(postPrefab, contentArea);

        // Set the photo in the Image component
        Image photoImage = newPost.transform.Find("Image").GetComponent<Image>();
        if (photoImage != null)
        {
            photoImage.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        }

        // Set the description in the TextMeshPro component
        TextMeshProUGUI descriptionText = newPost.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        // Set the time
        newPost.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = time;
        print(time);

        // Set the Views
        newPost.transform.Find("Views").GetComponent<TextMeshProUGUI>().text = views.ToString() + " views";
        print(newPost.transform.Find("Time").GetComponent<TextMeshProUGUI>().text);

    }
    /// <summary>
    /// Updates the captured photos list and refreshes the blog UI.
    /// </summary>
    /// <param name="photos">The list of captured photos and descriptions</param>
    public void UpdatePhotoList(List<(Texture2D photo, string description, string time, int view)> _photos)
    {
        capturedPhotos = _photos;
        PopulateBlog();
    }
}

