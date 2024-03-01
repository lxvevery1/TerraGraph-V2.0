using UnityEngine;
using TMPro;
using System;

public class ErrorMessage : MonoBehaviour
{
    public event Action OnThrowErrorAction;
    public TMP_Text StreamableTextUI;


    public void ThrowError(string errorText)
    {
        OnThrowErrorAction?.Invoke();

        StreamableTextUI.SetText(errorText);

        Debug.LogError(errorText);
    }
}
