using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class Game : MonoBehaviour
{
    [Header("Detection Settings")]
    public Color targetColor = Color.red;
    [Range(0f, 1f)] public float colorTolerance = 0.1f;
    public Rect detectorRegion = new Rect(0.4f, 0.4f, 0.2f, 0.2f); // normalized

    [Header("Zone Spawning")]
    public float minCoord = 0f;
    public float maxCoord = 0.75f;
    public float zoneSize = 0.15f; // size of detection region

    [Header("Performance")]
    public int downscaleFactor = 4; // skip some pixels for speed

    [Header("Debug")]
    public bool showDebugRegion = true;
    public bool logDetection = true;
    public bool sceneChange;
    public bool Point;
    public ScenePlay player;

    [Header("UI")]
    public TextMeshProUGUI pointsText;

    public event Action<Color> OnColorDetected;

    private Camera cam;
    private Texture2D tex;
    private int points = 0;
    private bool canDetect = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdatePointsUI();
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
        if (!canDetect) return;

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
                if (logDetection) Debug.Log($"✅ Color detected: {targetColor}");
                OnColorDetected?.Invoke(targetColor);

                AddPoint();
                SpawnNewZone();
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

    void AddPoint()
    {
        points++;
        UpdatePointsUI();
    }

    void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = $"Points: {points}";
        }
    }

    void SpawnNewZone()
    {
        // briefly disable detection to avoid double hits in same frame
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        canDetect = false;
        yield return new WaitForSeconds(0.1f);

        float newX = UnityEngine.Random.Range(minCoord, maxCoord - zoneSize);
        float newY = UnityEngine.Random.Range(minCoord, maxCoord - zoneSize);

        detectorRegion = new Rect(newX, newY, zoneSize, zoneSize);

        if (logDetection)
        {
            Debug.Log($"🟩 New zone spawned at {detectorRegion.x:F2}, {detectorRegion.y:F2}");
        }

        canDetect = true;
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

        // Calculate pulse alpha (0.3 to 1)
        float pulse = 0.3f + 0.7f * (0.5f + 0.5f * Mathf.Sin(Time.time * 3f)); // 3f = pulse speed

        DrawRectOutline(pixelRect, 2f, new Color(0f, 1f, 0f, pulse));
    }

    /// <summary>
    /// Draws a hollow rectangle outline using GUI.Box.
    /// </summary>
    void DrawRectOutline(Rect rect, float thickness, Color color)
    {
        Color prevColor = GUI.color;
        GUI.color = color;

        // Top
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
        // Bottom
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
        // Left
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        // Right
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);

        GUI.color = prevColor;
    }

}

