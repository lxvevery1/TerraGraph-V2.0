using System.Collections;
using UnityEngine;
using TMPro;

public class SwitchableTransparentTextTMP : MonoBehaviour
{
    [Header("Transparency Settings")]
    [SerializeField] protected float fadeDuration = 3.0f;
    [SerializeField] protected float displayDuration = 5.0f;
    [SerializeField, Range(0, 1)] protected float maxTextAlpha = 1.0f;
    [SerializeField, Range(0, 1)] protected float minTextAlpha = 0.0f;


    protected float timer = 0.0f;
    protected bool textIsShown;
    protected bool fadingCoroutineIsPlaying;


    private void OnValidate()
    {
        if (minTextAlpha > maxTextAlpha)
        {
            maxTextAlpha = minTextAlpha;
        }
    }


    protected void UpdateTextTransparency(TMP_Text streamableTextUI, bool textIsShown)
    {
        timer += Time.deltaTime;

        if (timer > displayDuration && textIsShown)
        {
            SwitchTextTransparency(streamableTextUI, !textIsShown);
            timer = 0;
        }
    }

    protected void SwitchTextTransparency(TMP_Text streamableTextUI)
    {
        if (fadingCoroutineIsPlaying)
        {
            return;
        }

        if (textIsShown)
        {
            StartCoroutine(StartTextFading(streamableTextUI, maxTextAlpha));
        }
        else
        {
            StartCoroutine(StartTextFading(streamableTextUI, minTextAlpha));
        }
        textIsShown = !textIsShown;
    }

    protected void SwitchTextTransparency(TMP_Text streamableTextUI, bool _textIsFaded)
    {
        if (fadingCoroutineIsPlaying)
        {
            return;
        }

        StartCoroutine(StartTextFading(streamableTextUI, _textIsFaded));
        textIsShown = !textIsShown;
    }

    private IEnumerator StartTextFading(TMP_Text streamableTextUI, float targetAlpha)
    {
        fadingCoroutineIsPlaying = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / fadeDuration);

            streamableTextUI.alpha = streamableTextUI.alpha + targetAlpha - streamableTextUI.alpha * blend;

            yield return null;
        }
        fadingCoroutineIsPlaying = false;
    }

    private IEnumerator StartTextFading(TMP_Text streamableTextUI, bool textIsFaded)
    {
        fadingCoroutineIsPlaying = true;
        float t = 0;

        var targetAlpha = textIsFaded ? 1.0f : 0.0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / fadeDuration);

            streamableTextUI.alpha = Mathf.Lerp(streamableTextUI.alpha, targetAlpha, blend);

            yield return null;
        }
        fadingCoroutineIsPlaying = false;
    }
}