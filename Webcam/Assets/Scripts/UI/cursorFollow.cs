using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorFollow : MonoBehaviour
{
    public Sprite[] sprites;
    public bool hideCursor = false;
    public int clickSFX = 0;

    public int xOffSet;
    public int yOffSet;

    private Image image;
    private int currentIndex = 0;

    void Awake()
    {
        image = GetComponent<Image>();

        if (hideCursor)
        {
            UnityEngine.Cursor.visible = false;
        }

        if (sprites != null && sprites.Length > 0)
        {
            image.sprite = sprites[currentIndex];
        }
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 cursorPos = Mouse.current.position.ReadValue();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, cursorPos, null, out localPoint);
        (transform as RectTransform).anchoredPosition = localPoint + new Vector2(xOffSet, yOffSet);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            MousePress();
        }
    }

    void MousePress()
    {
        currentIndex = (currentIndex + 1) % sprites.Length;
        image.sprite = sprites[currentIndex];

        if (sfxManager.Instance != null)
        {
            sfxManager.Instance.PlaySound(clickSFX, gameObject);
        }
    }
}
