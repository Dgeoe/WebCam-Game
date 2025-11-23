using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayWebCamUI : MonoBehaviour
{
    private RawImage rawImage;
    private AspectRatioFitter aspectFitter;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        aspectFitter = GetComponent<AspectRatioFitter>();

        rawImage.texture = WebcamManager.Instance.WebcamTexture;

        StartCoroutine(AdjustAspectWhenReady());
    }

    private IEnumerator AdjustAspectWhenReady()
    {
        var cam = WebcamManager.Instance.WebcamTexture;
        while (cam.width < 100)
            yield return null;

        aspectFitter.aspectRatio = (float)cam.width / cam.height;
    }
}
