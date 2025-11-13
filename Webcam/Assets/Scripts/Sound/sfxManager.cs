using UnityEngine;

public class sfxManager : MonoBehaviour
{
    public static sfxManager Instance { get; private set; }

    public AudioClip[] sounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(int i, GameObject sender)
    {
        AudioSource player = sender.GetComponent<AudioSource>();

        if (sounds != null && i >= 0 && i < sounds.Length && sounds[i] != null && player != null)
        {
            player.PlayOneShot(sounds[i]);
        }
    }
}

