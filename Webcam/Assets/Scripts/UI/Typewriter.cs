using TMPro;
using UnityEngine;
using System.Collections;

public class Typewriter : MonoBehaviour
{
    private TMP_Text textComponent;
    [TextArea] public string fullText;
    public float delay = 0.02f;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        StartCoroutine(TypeText(fullText));
    }

    IEnumerator TypeText(string text)
    {
        textComponent.text = "";
        int i = 0;

        while (i < text.Length)
        {
            //for special blemishes 
            if (text[i] == '<')
            {
                int closeIndex = text.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    textComponent.text += text.Substring(i, closeIndex - i + 1);
                    i = closeIndex + 1;
                    continue;
                }
            }

            textComponent.text += text[i];
            i++;
            yield return new WaitForSeconds(delay);
        }
    }
}
