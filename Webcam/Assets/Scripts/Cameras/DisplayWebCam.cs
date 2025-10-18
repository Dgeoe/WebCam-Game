using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayWebCam : MonoBehaviour
{
    private WebCamTexture webcamTexture;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogWarning("No webcam devices found!");
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Device found: " + devices[i].name);
        }

        // The first camera will be the main one used (Ie the web cam)
        string deviceName = devices[0].name;

        //Assign Webcam to texture instance
        webcamTexture = new WebCamTexture(deviceName);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;

        //Allign Camera with Real Life
        FaceMeCoward();
        webcamTexture.Play();

    }

    void FaceMeCoward()
    {
        // Flip the current X scale of the texture, so your actions irl line up with in the game
        //Like a mirror Not like a camera
        Vector2 currentScale = GetComponent<Renderer>().material.mainTextureScale;
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(currentScale.x * -1, currentScale.y);
        Vector2 currentOffset = GetComponent<Renderer>().material.mainTextureOffset;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1 - currentOffset.x, currentOffset.y);
    }
}
