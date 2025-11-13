using UnityEngine;
using UnityEngine.UI;

public class DisplayWebCamUI : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private RawImage rawImage;
    private AspectRatioFitter aspectFitter;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("No RawImage component found on this GameObject!");
            return;
        }

        // Try to grab AspectRatioFitter from players webcam set up to maintain proportions
        aspectFitter = GetComponent<AspectRatioFitter>();
        if (aspectFitter == null)
        {
            aspectFitter = gameObject.AddComponent<AspectRatioFitter>();
        }
        aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("No webcam devices found!");
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Webcam found: " + devices[i].name);
        }

        // use the first available device
        string deviceName = devices[0].name;

        // assign wecam texture to ui raw img
        webcamTexture = new WebCamTexture(deviceName);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();

        //correct ratios
        StartCoroutine(AdjustAspectWhenReady());
        FlipHorizontal();
    }

    private void FlipHorizontal()
    {
        RectTransform rectTransform = rawImage.rectTransform;
        Vector3 scale = rectTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * -1; 
        rectTransform.localScale = scale;
    }

    private System.Collections.IEnumerator AdjustAspectWhenReady()
    {
        while (webcamTexture.width < 100)
        {
            yield return null;
        }

        float aspectRatio = (float)webcamTexture.width / webcamTexture.height;
        aspectFitter.aspectRatio = aspectRatio;

        Debug.Log($"Webcam aspect ratio set to: {aspectRatio:F2}");
    }

    void OnDisable()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}

