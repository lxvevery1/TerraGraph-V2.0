using TMPro;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class DialogueSystem : MonoBehaviour
{
    public event Action<int> OnDialogueStart;
    public event Action<int> OnDialogueEnd;


    [SerializeField] private TMP_Text speechField;
    [SerializeField] private GuideAnim guideAnim;


    private bool alreadyStart = false;
    private bool isDisplaying = false;
    private List<string> replicas;


    private void Awake()
    {
        replicas = new()
        {
            "Hi, my name is John, I'm your guide today!",
            "I think you want to solve some system analysis task...",
            "I can help you with it!",
            "You can place your matrix here, see?",
            "While you finish just press that button",
            "It's easy, isnt it?",
            "But don't forget...",
            "Matrix must be square (nxn) and non-negative!",
            "Adjacency Matrix -> G<sup>+</sup> + G<sup>-</sup>"
        };
    }


    public void DisplayDialogue()
    {
        if (!isDisplaying && !alreadyStart)
        {
            alreadyStart = true;
            StartCoroutine(DisplayDialogue(replicas.ToArray()));
        }
    }

    private IEnumerator DisplayDialogue(string[] dialogue)
    {
        isDisplaying = true;


        for (int i = 0; i < dialogue.Length; i++)
        {
            OnDialogueStart?.Invoke(i);

            SplitString(dialogue[i]);

            string[] chars = new string[dialogue[i].Length];

            yield return new WaitForSeconds((chars.Length + 45) * 0.075f);

            OnDialogueEnd?.Invoke(i);
        }
    }

    private void SplitString(string sentence)
    {
        speechField.text = "";
        string[] chars = new string[sentence.Length];

        for (int i = 0; i < sentence.Length; i++)
        {
            chars[i] = Convert.ToString(sentence[i]);
        }

        StartCoroutine(StringDisplayDelay(chars));
    }

    private IEnumerator StringDisplayDelay(string[] chars)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            speechField.text += chars[i];
            yield return new WaitForSeconds(0.025f);
        }

        isDisplaying = false;
    }

}
