using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Speaker;

public class DialogueLayout : BaseDialogueLayout
{
    [SerializeField] public SpeakerUI LeftSpeakerUI = default;
    [SerializeField] public SpeakerUI RightSpeakerUI = default;
    [SerializeField] public Speaker LeftSpeaker = default;
    [SerializeField] public Speaker RightSpeaker = default;
    [SerializeField] TextMeshProUGUI _dialogueTextField = default;
    [SerializeField] RectTransform _choicesContainer = default;
    [SerializeField] Image _backgroundImage = default;
    [SerializeField] GameObject choiceButtonEven = default;
    [SerializeField] GameObject choiceButtonOdd = default;

    protected Speaker primarySpeaker = null;

    protected virtual void OnEnable()
    {
        if (LeftSpeakerUI != null)
        {
            LeftSpeakerUI.HideSpeaker();
        }
        if (RightSpeakerUI != null)
        {
            RightSpeakerUI.HideSpeaker();
        }
    }

    public override void SetPrimarySpeaker(Speaker speaker, SpeakerSide speakerSide)
    {
        primarySpeaker = speaker;
    }

    public override void SetSpeakerImages(List<Speaker> speakers)
    {
        //if primary speaker exists
        if (primarySpeaker != null)
        {
            //set as LeftSpeaker
            LeftSpeakerUI.SetPortrait(primarySpeaker?.portrait);

            //loop through speakers and find first non-primary and set as right.
            foreach (Speaker speaker in speakers)
            {
                if(speaker != primarySpeaker)
                {
                    RightSpeakerUI.SetPortrait(speaker?.portrait);
                    break;
                }
            }
        }
        else
        {
            //set first as left, second as right
            if (speakers.Count > 0)
            {
                LeftSpeakerUI.SetPortrait(speakers[0]?.portrait);
            }
            if (speakers.Count > 1)
            {
                RightSpeakerUI.SetPortrait(speakers[1]?.portrait);
            }
        }
    }

    public override void ShowSpeaker(Speaker speaker)
    {
        string speakerName = speaker == null ? "" : speaker.fullName != "" ? speaker.fullName : speaker.name;

        // Assume the first to speak is the primary speaker
        if (primarySpeaker == null && speakerName != "")
        {
            primarySpeaker = speaker;
        }

        if (speaker == primarySpeaker)
        {
            LeftSpeakerUI.SetName(speakerName);
            LeftSpeakerUI.SetPortrait(speaker?.portrait);
            RightSpeakerUI.HideName();
        }
        else
        {
            RightSpeakerUI.SetName(speakerName);
            RightSpeakerUI.SetPortrait(speaker?.portrait);
            LeftSpeakerUI.HideName();
        }
    }

    public override Image GetBackgroundImage()
    {
        return _backgroundImage;
    }

    public override TextMeshProUGUI GetDialogueBox()
    {
        return _dialogueTextField;
    }

    public override RectTransform GetChoicesContainer()
    {
        return _choicesContainer;
    }

    public GameObject GetChoiceButton()
    {
        return choiceButtonEven;
    }

    public GameObject GetOddChoiceButton()
    {
        return choiceButtonOdd;
    }

    public override GameObject GetChoiceButton(int choiceNumber, int totalChoices)
    {
        return (choiceNumber % 2 == 0) ? choiceButtonEven : choiceButtonOdd;

    }

    public override void SetSpeakerEmotion(Speaker speaker, string emotion) {
        Sprite portrait = null;
        foreach (Sprite curEmotion in speaker.emotions) {
            if (curEmotion.name.ToLower() == emotion.ToLower()) {
                portrait = curEmotion;
                break;
            }
        }
        if (portrait == null) {
            Debug.LogException(
                new System.Exception("Could not find portrait of " + emotion + " for " + speaker.name)
            );
        }

        if (speaker == primarySpeaker || primarySpeaker == null) {
            print(LeftSpeakerUI.SpeakerPortrait.name);
            print(portrait.name);
            LeftSpeakerUI.SpeakerPortrait.overrideSprite = portrait;
            print(LeftSpeakerUI.SpeakerPortrait.sprite);
        } else {
            RightSpeakerUI.SpeakerPortrait.sprite = portrait;
        }
    }
}