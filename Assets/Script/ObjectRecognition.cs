using System.Collections.Generic;
using UnityEngine;

public class ObjectRecognition : MonoBehaviour
{
    [SerializeField] private Camera photoCaptureCamera; // Assign the camera used for capturing photos
    [SerializeField] private RectTransform photoCaptureArea; // The UI boundary for photo capture

    // Object lists
    private GameObject[] touristObjects = { };
    private GameObject[] secretObjects = { };
    private GameObject[] otherObjects = { };
    private bool[] touristObjectsCount = { };
    private bool[] secretObjectsCount = { };
    private bool[] otherObjectsCount = { };

    // Counts
    public int TagCount { get; private set; }

    private void Start()
    {
        InitiateObjectList("touristObject", touristObjects, touristObjectsCount);
        InitiateObjectList("secretObject", secretObjects, secretObjectsCount);
        InitiateObjectList("otherObject", otherObjects, otherObjectsCount);
    }

    public int RecognizeObjects(string tag)
    {
        GameObject[] tempObjects;

        // Reset counts and lists
        TagCount = 0;

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

        GameObject[] Objects = { };
        bool[] ObjectsCount = { };
        // Check each object for its tag and position
        foreach (GameObject obj in allObjects)
        {
            if (!captureBounds.Contains(obj.transform.position)) continue;
            if (!obj.CompareTag(tag)) continue;
            
            switch (tag)
            {
                case "touristObject":
                    Objects = touristObjects;
                    ObjectsCount = touristObjectsCount;
                    break;
                case "secretObject":
                    Objects = secretObjects;
                    ObjectsCount = secretObjectsCount;
                    break;
                case "otherObject":
                    Objects = otherObjects;
                    ObjectsCount = otherObjectsCount;
                    break;
            }
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] != obj) continue;
                if (ObjectsCount[i] == true) continue;

                ObjectsCount[i] = true;
                TagCount++;
            }
        }
        return TagCount;
    }

    private void InitiateObjectList(string _tag, GameObject[] _array, bool[] _count)
    {
        print(_array.Length);
        print(_tag);
        _array = GameObject.FindGameObjectsWithTag(_tag);
        print(_array.Length);
        if (_array.Length == 0) return;
        _count = new bool[_array.Length];
        for ( int i = 0; i <  _array.Length; i++)
        {
            _count[i] = false;
            print(i);
        }
            

    }
}
