using System.Collections.Generic;
using UnityEngine;

public class ObjectRecognition : MonoBehaviour
{
    [SerializeField] private Camera photoCaptureCamera; // Assign the camera used for capturing photos
    [SerializeField] private RectTransform photoCaptureArea; // The UI boundary for photo capture

    // Object lists
    private List<GameObject> tagAObjects = new List<GameObject>();
    private List<GameObject> tagBObjects = new List<GameObject>();
    private List<GameObject> tagCObjects = new List<GameObject>();

    // Counts
    public int TagACount { get; private set; }
    public int TagBCount { get; private set; }
    public int TagCCount { get; private set; }

    public void RecognizeObjects()
    {
        // Reset counts and lists
        TagACount = 0;
        TagBCount = 0;
        TagCCount = 0;

        tagAObjects.Clear();
        tagBObjects.Clear();
        tagCObjects.Clear();

        // Define the world boundaries of the photo capture area
        Vector3[] worldCorners = new Vector3[4];
        photoCaptureArea.GetWorldCorners(worldCorners);
        Bounds captureBounds = new Bounds();
        captureBounds.center = (worldCorners[0] + worldCorners[2]) / 2; // Center of the area
        captureBounds.extents = new Vector3(
            Mathf.Abs(worldCorners[0].x - worldCorners[2].x) / 2,
            Mathf.Abs(worldCorners[0].y - worldCorners[2].y) / 2,
            Mathf.Abs(worldCorners[0].z - worldCorners[2].z) / 2
        );

        // Find all objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Check each object for its tag and position
        foreach (GameObject obj in allObjects)
        {
            if (captureBounds.Contains(obj.transform.position))
            {
                if (obj.CompareTag("TagA"))
                {
                    tagAObjects.Add(obj);
                    TagACount++;
                }
                else if (obj.CompareTag("TagB"))
                {
                    tagBObjects.Add(obj);
                    TagBCount++;
                }
                else if (obj.CompareTag("TagC"))
                {
                    tagCObjects.Add(obj);
                    TagCCount++;
                }
            }
        }

        Debug.Log($"TagA Count: {TagACount}, TagB Count: {TagBCount}, TagC Count: {TagCCount}");
    }

    // Example of how to access objects
    public GameObject[] GetTagAObjects() => tagAObjects.ToArray();
    public GameObject[] GetTagBObjects() => tagBObjects.ToArray();
    public GameObject[] GetTagCObjects() => tagCObjects.ToArray();
}
