using UnityEngine;
using System;
using System.Collections;

public class ColorDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public Color targetColor = Color.red;
    [Range(0f, 1f)] public float colorTolerance = 0.1f;
    public Rect detectorRegion = new Rect(0.4f, 0.4f, 0.2f, 0.2f); // normalized

    [Header("Performance")]
    public int downscaleFactor = 4; // skip some pixels for speed

    [Header("Debug")]
    public bool showDebugRegion = true;
    public bool logDetection = true;
    public bool sceneChange;
    public bool Point;
    public ScenePlay player;

    public event Action<Color> OnColorDetected;

    private Camera cam;
    private Texture2D tex;

    void Start()
    {
        cam = GetComponent<Camera>();
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame(); // wait until frame is ready
            DetectColor();
        }
    }

    void DetectColor()
    {
        // Convert normalized rect to pixel space
        int x = Mathf.FloorToInt(detectorRegion.x * Screen.width);
        int y = Mathf.FloorToInt(detectorRegion.y * Screen.height);
        int w = Mathf.FloorToInt(detectorRegion.width * Screen.width);
        int h = Mathf.FloorToInt(detectorRegion.height * Screen.height);

        // Clamp to valid range
        x = Mathf.Clamp(x, 0, Screen.width - 1);
        y = Mathf.Clamp(y, 0, Screen.height - 1);
        w = Mathf.Clamp(w, 1, Screen.width - x);
        h = Mathf.Clamp(h, 1, Screen.height - y);

        if (tex == null || tex.width != w || tex.height != h)
        {
            if (tex != null) Destroy(tex);
            tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        }

        tex.ReadPixels(new Rect(x, y, w, h), 0, 0);
        tex.Apply(false);

        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i += downscaleFactor)
        {
            if (IsColorMatch(pixels[i], targetColor))
            {
                if (logDetection) Debug.Log($"YAY {targetColor}");
                if (sceneChange == true)
                {
                    player.PlayNextScene();
                }
                OnColorDetected?.Invoke(targetColor);
                return;
            }
        }
    }

    bool IsColorMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < colorTolerance &&
               Mathf.Abs(a.g - b.g) < colorTolerance &&
               Mathf.Abs(a.b - b.b) < colorTolerance;
    }

    void OnGUI()
    {
        if (!showDebugRegion) return;

        Rect pixelRect = new Rect(
            detectorRegion.x * Screen.width,
            (1 - detectorRegion.y - detectorRegion.height) * Screen.height,
            detectorRegion.width * Screen.width,
            detectorRegion.height * Screen.height
        );

        GUI.color = new Color(1, 0, 0, 0.3f);
        GUI.DrawTexture(pixelRect, Texture2D.whiteTexture);
    }
}