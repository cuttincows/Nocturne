using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingGameLayout : DialogueLayout
{
    public GameObject speakerPortrait;
    public Image speakerImage;
    public TextMeshProUGUI speakerName;

    protected override void OnEnable()
    {
        base.OnEnable();
        speakerPortrait.SetActive(false);
    }

    public override void ShowSpeaker(Speaker speaker)
    {
        if (speaker == null)
        {
            speakerPortrait.SetActive(false);
            speakerName.enabled = false;
            return;
        }
         speakerPortrait.SetActive(true);
        speakerName.enabled = true;
    }

    public void HideSpeaker(string _)
    {
        speakerPortrait.SetActive(false);
        speakerName.enabled = false;
    }

    public override void SetSpeakerEmotion(Speaker speaker, string emotion)
    {
        if (speaker == null)
        {
            speakerName.enabled = false;
            speakerPortrait.SetActive(false);
            return;
        }

        speakerName.enabled = true;
        speakerName.text = speaker.fullName;

        speakerPortrait.SetActive(true);
        foreach (Sprite curEmotion in speaker.emotions) 
        {
            if (curEmotion.name.ToLower() == emotion.ToLower()) 
            {
                speakerImage.sprite = curEmotion;
                break;
            }
        }
    }
}
