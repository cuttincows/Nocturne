using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakerUI : MonoBehaviour
{
    [SerializeField] RectTransform SpeakerNameBox = default;
    [SerializeField] TextMeshProUGUI SpeakerName = default;
    [SerializeField] public Image SpeakerPortrait = default;
    [SerializeField] public TextMeshProUGUI DialogueBox = default;
    public void HideSpeaker()
    {
        HideName();
        SpeakerPortrait.gameObject.SetActive(false);
    }

    virtual public void HideName()
    {
        SpeakerNameBox.gameObject.SetActive(false);
        SpeakerName.gameObject.SetActive(false);
    }

    virtual public void ShowName()
    {
        SpeakerNameBox.gameObject.SetActive(true);
        SpeakerName.gameObject.SetActive(true);
    }

    virtual public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            HideName();
            return;
        }
        SpeakerName.SetText(name);
        ShowName();
    }

    public void SetPortrait(Sprite sprite)
    {
        if (sprite == null) return;
        SpeakerPortrait.gameObject.SetActive(true);
        SpeakerPortrait.sprite = sprite;
    }
}