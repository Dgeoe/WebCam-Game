using UnityEngine;
using UnityEngine.UI;

public class BackgroundSubtraction : MonoBehaviour
{
    public RawImage display;
    public float threshold = 0.2f;
    public bool autoCaptureBackground = true;

    private WebCamTexture webcamTexture;
    private Texture2D diffTexture;
    private Color32[] backgroundPixels;
    private Color32[] currentPixels;
    private Color32[] diffPixels;
    private bool backgroundCaptured = false;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("No webcam detected!");
            return;
        }

        webcamTexture = new WebCamTexture(devices[0].name);
        display.texture = webcamTexture;
        webcamTexture.Play();

        // Wait until webcam initializes before we make textures
        StartCoroutine(WaitForWebcamReady());
    }

    private System.Collections.IEnumerator WaitForWebcamReady()
    {
        // Wait until we have valid width/height
        while (webcamTexture.width < 100)
        {
            yield return null;
        }

        Debug.Log($"Webcam initialized at {webcamTexture.width}x{webcamTexture.height}");

        diffTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        display.texture = diffTexture;

        if (autoCaptureBackground)
            Invoke(nameof(CaptureBackground), 2f);
    }

    void CaptureBackground()
    {
        backgroundPixels = webcamTexture.GetPixels32();
        backgroundCaptured = true;
        Debug.Log("Background captured!");
    }

    void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying) return;
        if (!backgroundCaptured) return;

        currentPixels = webcamTexture.GetPixels32();

        // Safety check: rebuild buffers if webcam resized
        int pixelCount = webcamTexture.width * webcamTexture.height;
        if (diffPixels == null || diffPixels.Length != pixelCount)
        {
            diffPixels = new Color32[pixelCount];
            diffTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
            display.texture = diffTexture;
            Debug.Log("Rebuilt diffTexture due to resolution change.");
        }

        for (int i = 0; i < pixelCount; i++)
        {
            float diff = Mathf.Abs(currentPixels[i].r - backgroundPixels[i].r) / 255f +
                         Mathf.Abs(currentPixels[i].g - backgroundPixels[i].g) / 255f +
                         Mathf.Abs(currentPixels[i].b - backgroundPixels[i].b) / 255f;

            byte value = (diff > threshold) ? (byte)255 : (byte)0;
            diffPixels[i] = new Color32(value, value, value, 255);
        }

        diffTexture.SetPixels32(diffPixels);
        diffTexture.Apply();

        DetectMovementZones(diffPixels, webcamTexture.width, webcamTexture.height);
    }

    void DetectMovementZones(Color32[] diff, int w, int h)
    {
        int zoneWidth = w / 3;
        int[] zoneCounts = new int[3];

        // Defensive check in case something desyncs
        if (diff.Length < w * h)
        {
            Debug.LogWarning("Diff array smaller than expected frame size!");
            return;
        }

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (diff[y * w + x].r > 128)
                {
                    int zone = Mathf.Clamp(x / zoneWidth, 0, 2);
                    zoneCounts[zone]++;
                }
            }
        }

        if (zoneCounts[0] > 1000) Debug.Log("Movement LEFT");
        if (zoneCounts[1] > 1000) Debug.Log("Movement CENTER");
        if (zoneCounts[2] > 1000) Debug.Log("Movement RIGHT");
    }
}
