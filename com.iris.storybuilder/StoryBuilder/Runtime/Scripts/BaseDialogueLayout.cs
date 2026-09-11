using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum SpeakerSide { Left, Right }

public abstract class BaseDialogueLayout : MonoBehaviour
{
    //public Dialogue _dialogue;
    //public virtual void SetDialogue(Dialogue dialogue)
    //{
    //    _dialogue = dialogue;
    //}
    public abstract void SetPrimarySpeaker(Speaker speaker, SpeakerSide speakerSide);
    public abstract void SetSpeakerImages(List<Speaker> speakers);
    public abstract void ShowSpeaker(Speaker speaker);
    public abstract void SetSpeakerEmotion(Speaker speaker, string emotion);
    public abstract Image GetBackgroundImage();
    public abstract TextMeshProUGUI GetDialogueBox();
    public abstract RectTransform GetChoicesContainer();
    public abstract GameObject GetChoiceButton(int choiceNumber, int totalChoices);
}