using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIColor : MonoBehaviour
{
    [SerializeField] private Image matrix;
    [SerializeField] private Image genButton;


    private Color defImageColor;
    private Color defTextColor;
    private Color darkerImageColor;
    private Color darkerTextColor;
    private DialogueSystem dialogueSys;
    private ErrorMessage errorMessage;


    private void Awake()
    {
        dialogueSys = FindObjectOfType<DialogueSystem>();
        errorMessage = FindObjectOfType<ErrorMessage>();

        defImageColor = matrix.color;
        defTextColor = errorMessage.StreamableTextUI.color;

        darkerImageColor = Color.HSVToRGB(239, 58+15, 54+15);
        darkerTextColor = Color.grey;

        
        genButton.color = defImageColor;

        dialogueSys.OnDialogueStart += HandleDialogueStart;
        errorMessage.OnThrowErrorAction += HandleThrowError;
    }

    private void HandleDialogueStart(int dialogueNumber)
    {
        if (dialogueNumber == 3)
        {
            StartCoroutine(DoColorBlink(matrix));
        }
        else if (dialogueNumber == 4)
        {
            StartCoroutine(DoColorBlink(genButton));
        }
    }

    private void HandleThrowError()
    {
        StartCoroutine(DoColorBlink(errorMessage.StreamableTextUI));
    }

    private IEnumerator DoColorBlink(Image image)
    {
        float duration = 1.5f;
        int iterations = 9;

        for (int i = 0; i < iterations; i++)
        {
            yield return SmoothTransitionCoroutine(image, i % 2 == 0 ? defImageColor : darkerImageColor, duration / 2f);
        }
    }

    private IEnumerator SmoothTransitionCoroutine(Image image, Color targetValue, float duration)
    {
        Color startColor = image.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            image.color = Color.Lerp(startColor, targetValue, t);
            yield return new WaitForEndOfFrame();
        }

        image.color = targetValue;
    }

    private IEnumerator DoColorBlink(TMP_Text tmp_text)
    {
        float duration = 1.5f;
        int iterations = 9;

        for (int i = 0; i < iterations; i++)
        {
            yield return SmoothTransitionCoroutine(tmp_text, i % 2 == 0 ? defTextColor : darkerTextColor, duration / 2f);
        }
    }

    private IEnumerator SmoothTransitionCoroutine(TMP_Text text, Color targetValue, float duration)
    {
        Color startColor = text.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            text.color = Color.Lerp(startColor, targetValue, t);
            yield return new WaitForEndOfFrame();
        }

        text.color = targetValue;
    }
}
