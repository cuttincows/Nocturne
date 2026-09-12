using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingGameLayout : DialogueLayout
{
    public Image speakerPortrait;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerNameBG;

    protected override void OnEnable()
    {
        base.OnEnable();
        speakerPortrait.enabled = false;
    }

    public override void ShowSpeaker(Speaker speaker)
    {
        if (speaker == null)
        {
            speakerPortrait.enabled = false;
            speakerName.enabled = false;
            return;
        }
         speakerPortrait.enabled = true;
        speakerName.enabled = true;
        speakerNameBG.enabled = true;
    }

    public void HideSpeaker(string _)
    {
        speakerPortrait.enabled = false;
        speakerName.enabled = false;
        speakerNameBG.enabled = false;
    }

    public override void SetSpeakerEmotion(Speaker speaker, string emotion)
    {
        if (speaker == null)
        {
            speakerName.enabled = false;
            speakerPortrait.enabled = false;
            speakerNameBG.enabled = false;
            return;
        }

        speakerName.enabled = true;
        speakerPortrait.enabled = true;
        speakerNameBG.enabled = true;
        foreach (Sprite curEmotion in speaker.emotions) 
        {
            if (curEmotion.name.ToLower() == emotion.ToLower()) 
            {
                speakerPortrait.sprite = curEmotion;
                break;
            }
        }
    }
}
