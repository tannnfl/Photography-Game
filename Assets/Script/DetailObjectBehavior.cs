using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DetailObjectBehavior : MonoBehaviour
{
    [Header("camera")]
    public CinemachineVirtualCamera virtualCamera;
    public float showDetailCamSize = 8;
    public float showDetailDuration = 2;

    //set alpha
    SpriteRenderer sr;
    float camSizeRT;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); // Assign SpriteRenderer first
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer component is not attached to the GameObject.");
            return; // Exit if sr is null to avoid any further errors
        }
        
        SetAlpha(0); // Now that sr is assigned, you can safely call SetAlpha()
    }

    void Update()
    {
        //set alpha
        camSizeRT = virtualCamera.m_Lens.OrthographicSize;
        if (camSizeRT <= showDetailCamSize - showDetailDuration)
        {
            SetAlpha(1);
        }
        else if (camSizeRT <= showDetailCamSize)
        {
            float alpha = showDetailDuration / (showDetailCamSize - camSizeRT);
            SetAlpha(alpha); // Ensures alpha value is between 0 and 1
        }
        else if(camSizeRT >= showDetailCamSize)
        {
            SetAlpha(0);
        }
        //print(camSizeRT);
    }

    private void SetAlpha(float alpha)
    {
        if (sr == null) return; // Ensure sr is not null before accessing it
        
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
}
