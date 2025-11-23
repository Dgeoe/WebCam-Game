using UnityEngine;

public class WebcamManager : MonoBehaviour
{
    public static WebcamManager Instance;
    public WebCamTexture WebcamTexture { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("No webcam devices found!");
            return;
        }

        WebcamTexture = new WebCamTexture(devices[0].name);
        WebcamTexture.Play();
    }
}
