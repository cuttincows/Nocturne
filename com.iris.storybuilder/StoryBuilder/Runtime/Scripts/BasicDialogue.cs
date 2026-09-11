using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICreateableDialogue
{
    void SetSpeakers(List<Speaker> speakers);
    void SetLayout(BaseDialogueLayout dialogueLayout);
    void AssignAssetsToDialogue();
    void SetSounds(List<AudioClip> clips);
    void SetBackgrounds(List<Sprite> sprites);
}

public class BasicDialogue : Dialogue, ICreateableDialogue
{
    [SerializeField] public BaseDialogueLayout layout;
    public Image defaultBackground;

    protected TextMeshProUGUI currentTypingField;

    private List<string> _trackedStats = new List<string>();

    protected bool typing;
    protected Speaker primarySpeaker;

    public int minCharacterWidth = 500;  // Out of 1000
    public float maxCharacterWidthPercent = 0.5f;
    public List<Sprite> backgrounds;

    protected override void Start()
    {
        if (Application.isPlaying)
            base.Start();
    }



    public void SetLayout(BaseDialogueLayout dialogueLayout)
    {
        layout = dialogueLayout;
    }

    protected override void DoSay(Speaker speaker, string[] args)
    {
        // The statement being said is always the last args element
        string statement = args[args.Length - 1];
        typing = true;

        if (args.Length >= 2) {  // 2 args is speakerEmotion, statement
            layout.SetSpeakerEmotion(speaker, args[0]);
        }

        layout.GetDialogueBox().TypeText(statement, TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
        {
            typing = false;
        });
        layout.ShowSpeaker(speaker);
        currentTypingField = layout.GetDialogueBox();
        layout.GetChoicesContainer().gameObject.SetActive(false);
    }

    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        //_nameField.text = speakerName;
        typing = true;
        layout.GetDialogueBox().TypeText($"{choicePrompt}", TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
        {
            typing = false;
        });
        currentTypingField = layout.GetDialogueBox();
        ClearOptions();
        int i = 0;
        foreach (var choiceText in choiceTexts)
        {
            Transform choiceObject = Instantiate(layout.GetChoiceButton(i, choiceTexts.Count)).transform;
            choiceObject.SetParent(layout.GetChoicesContainer());
            choiceObject.Find("Text").GetComponent<TMP_Text>().text = choiceText;

            choiceObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                // TODO: Here there is a bit of user pain as EventSystem "Send Navigation Events" has to be off to prevent this to double send input
                HandleInput(choiceText);
            });
            i++;
        }
        layout.GetChoicesContainer().gameObject.SetActive(true);
    }

    private void ClearOptions()
    {
        for (int i = 0; i < layout.GetChoicesContainer().childCount; i++)
        {
            Destroy(layout.GetChoicesContainer().GetChild(i).gameObject);
        }
    }

    public override void HandleInput(string input = null)
    {
        if (!CanContinue()) return;


        if (TextTyper.IsTyping(layout.GetDialogueBox()))
        {
            currentTypingField.CompleteTyping();
            currentTypingField = null;
            typing = false;
            return;
        }

        base.HandleInput(input);
    }

    public override void StopUsing()
    {
        gameObject.SetActive(false);
    }

    public override void SetBackground(string input, float duration)
    {

        foreach (Sprite background in backgrounds)
        {
            if (background.name == input)
            {
                backgroundSwapCoroutine = StartCoroutine(SwapBackground(layout.GetBackgroundImage(), background, duration));
                return;
            }
        }
        Debug.LogError("Background with name " + input + " not found");
    }

    // TODO: Rewrite w/o CrossFadeColor to allow skipping with input, instead of needing to block input
    private Coroutine backgroundSwapCoroutine = null;
    private IEnumerator SwapBackground(Image backgroundImage, Sprite newBGSprite, float duration)
    {
        float halfDuration = duration / 2f;

        backgroundImage.CrossFadeColor(Color.black, halfDuration, true, true);
        yield return new WaitForSeconds(duration);

        backgroundImage.overrideSprite = newBGSprite;
        backgroundImage.CrossFadeColor(Color.white, halfDuration, true, true);
        yield return new WaitForSeconds(duration);

        backgroundSwapCoroutine = null;
    }

    public bool CanContinue()
    {
        return backgroundSwapCoroutine == null;
    }

    public void SetPrimary(string speakerName)
    {
        primarySpeaker = GetSpeakerByName(speakerName);
    }

    public void TrackStatVar(string varName)
    {
        _trackedStats.Add(varName);
    }

    public void SetSpeakers(List<Speaker> speakers)
    {
        this.Speakers.AddRange(speakers);
    }

    public void SetSounds(List<AudioClip> clips)
    {
        sounds = clips;
    }

    public void SetBackgrounds(List<Sprite> sprites)
    {
        backgrounds = sprites;
    }

    public void AssignAssetsToDialogue()
    {
        layout.SetSpeakerImages(Speakers);
    }
}
