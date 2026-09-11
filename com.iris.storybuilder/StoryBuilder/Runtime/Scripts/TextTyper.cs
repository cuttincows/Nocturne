using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class TextTyper
{
    public const TypingType DEFAULT_TYPE = TypingType.ByLetter;
    public const float DEFAULT_SPEED = 2000f;

    private static HashSet<TextMeshProUGUI> forcedFields = new HashSet<TextMeshProUGUI>();
    private static Dictionary<TextMeshProUGUI, Coroutine> typingCoRMap = new Dictionary<TextMeshProUGUI, Coroutine>();
    private static Dictionary<TextMeshProUGUI, string> typingTextMap = new Dictionary<TextMeshProUGUI, string>();

    public static AudioSource blipSource;

    public static List<AudioClip> pips = new List<AudioClip>();

    private static List<TextMeshProUGUI> currentlyTypingTextMeshes = new List<TextMeshProUGUI>();

    public static bool GlobalPause { get; set; } = false;

    public static bool IsTyping(this TextMeshProUGUI textField)
    {
        return typingCoRMap[textField] != null;
    }

    public static bool IsTypingAny()
    {
        foreach(Coroutine c in typingCoRMap.Values)
        {
            if (c != null) return true;
        }
        return false;
    }

    public static void TypeText(this TextMeshProUGUI textField, string text, TypingType type = DEFAULT_TYPE, float speed = DEFAULT_SPEED, Action OnFinish = null)
    {
        if (!textField || !textField.gameObject.activeInHierarchy) return;
        if (!currentlyTypingTextMeshes.Contains(textField))
        {
            currentlyTypingTextMeshes.Add(textField);
        }
        textField.StopAllCoroutines();
        typingTextMap[textField] = text;
        typingCoRMap[textField] = textField.StartCoroutine(TypeCoroutine(textField, text, type, speed, OnFinish));
    }

    public static void CompleteAll()
    {
        foreach(var typingText in currentlyTypingTextMeshes)
        {
            typingText.CompleteTyping();
        }
    }

    public static void Finish()
    {
        foreach (var typingMesh in currentlyTypingTextMeshes)
        {
            if (typingMesh == null) return; 
            typingMesh.text = typingTextMap[typingMesh];
            if (!typingCoRMap.ContainsKey(typingMesh) || typingCoRMap[typingMesh] == null) return;
            typingMesh.StopCoroutine(typingCoRMap[typingMesh]);
        }

    }

    public static void ForceStop(this TextMeshProUGUI textField)
    {
        textField.StopCoroutine(typingCoRMap[textField]);
        textField.text = typingTextMap[textField];
    } 

    public static void CompleteTyping(this TextMeshProUGUI textField)
    {
        blipSource.Stop();
        forcedFields.Add(textField);
    }

    private static IEnumerator TypeCoroutine(TextMeshProUGUI textField, string statementText, TypingType type, float speed, Delegate OnFinish)
    {
        Color textColor = textField.color;
        textColor.a = 255;
        float wps;
        switch (type)
        {
                case TypingType.Immediate:
                    textField.text = statementText;
                    break;
                case TypingType.ByLetter: {
                    textField.text = statementText;
                    Color transparentColor = textColor;
                    transparentColor.a = 0;
                    textField.color = transparentColor;
                    textField.ForceMeshUpdate();
                    wps = speed / 60.0f;
                    List<string> openTagsStack = new List<string>();

                    TMP_TextInfo textInfo = textField.textInfo;
                    
                    // Iterate over letters in the full statement to render
                    for (int i = 0; i < statementText.Length; i++)
                    {
                        int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;
                        // emojis get tinted black if the text color is black, so for workaround it
                        // set color white for meshIndexes 1
                        if (vertexColors == null) continue;  // I think this means the text is disabled??
                        vertexColors[vertexIndex + 0] = meshIndex==1?Color.black:textColor;
                        vertexColors[vertexIndex + 1] = meshIndex==1?Color.black:textColor;
                        vertexColors[vertexIndex + 2] = meshIndex == 1 ? Color.black : textColor;
                        vertexColors[vertexIndex + 3] = meshIndex==1?Color.black:textColor;

                        textField.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        //if (blipSource != null && statementText[i] != ' ')
                        //{
                        //    PlayBlip();
                        //} 

                        while (GlobalPause) { yield return null; }  // Stall CoRoutine while paused but keep it active
                        yield return new WaitForSeconds(1.0f / wps);
                        if (forcedFields.Contains(textField)) {
                            textField.text = statementText;
                            typingCoRMap[textField] = null;
                            forcedFields.Remove(textField);
                            LayoutGroup layout = textField.GetComponentInParent<LayoutGroup>();
                            if(layout)
                                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
                            break;
                        }
                    }
                    blipSource.Stop();
                    break;
                }
                case TypingType.ByWord:
                    textField.text = "";
                    string[] words = statementText.Split(new string[] {" "}, StringSplitOptions.None);
                    wps = speed / 60.0f;
                    foreach (string word in words) {
                        textField.text += word + " ";
                        yield return new WaitForSeconds(1.0f / wps);
                        if (forcedFields.Contains(textField))
                        {
                            textField.text = statementText;
                            forcedFields.Remove(textField);
                            LayoutGroup layout = textField.GetComponentInParent<LayoutGroup>();
                            if(layout)
                                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
                            break;
                        }
                    }
                    break;
        }
        
        textField.color = textColor;
        typingCoRMap[textField] = null;
        OnFinish?.DynamicInvoke();
    }

    //private static void PlayBlip()
    //{
    //    if (pips.Count > 0)
    //    {
    //        int randIndex = UnityEngine.Random.Range(0, pips.Count);
    //        blipSource.clip = pips[randIndex];
    //    }
    //    blipSource.Play();
    //}
}
 