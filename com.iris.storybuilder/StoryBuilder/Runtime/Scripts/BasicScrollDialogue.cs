using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class BasicScrollDialogue : Dialogue, ICreateableDialogue
{
    [SerializeField] BasicScrollDialogueLayout _layout = default;
    public List<Sprite> backgrounds;

    private bool _isTyping;
    Coroutine _typingCoroutine = null;
    Action _typingCallback = null;
    [SerializeField] float _typingLetterDelay = 0.05f;
    Dictionary<string, string> speakerPositions = new Dictionary<string, string>();
    TextMeshProUGUI _currentTypingBox;

    #region setup
    public void AssignAssetsToDialogue()
    {
        _layout.SetSpeakerImages(Speakers);
    }

    public void SetBackgrounds(List<Sprite> sprites)
    {
        backgrounds = sprites;
    }

    public void SetLayout(BaseDialogueLayout dialogueLayout)
    {
        _layout = (BasicScrollDialogueLayout)dialogueLayout;
    }

    public void SetSounds(List<AudioClip> clips)
    {
        sounds = clips;
    }

    public void SetSpeakers(List<Speaker> speakers)
    {
        this.Speakers.AddRange(speakers);
    }
    #endregion

    #region choices
    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        _layout.PrepareChoice();
        if (!string.IsNullOrWhiteSpace(choicePrompt))
        {
            _currentTypingBox = _layout.GetChoicesTextBox();
            _typingCoroutine = StartCoroutine(TypeText(choicePrompt, () =>
            {
                _layout.AddChoices(choiceTexts, (choice) => { HandleInput(choice); });
            }));
        }
        else
        {
            _layout.AddChoices(choiceTexts, (choice) => { HandleInput(choice); });
        }
    }
    #endregion

    #region Dialogue
    protected override void DoSay(Speaker speaker, string[] args)
    {
        string statement = args[0];
        _layout.PrepareSay();


        if (speaker == null)
        {
            _currentTypingBox = _layout.GenerateDialogueBox();
        }
        else
        {
            if (speakerPositions.Count > 0)
            {
                _currentTypingBox = _layout.GenerateDialogueBox(speaker, GetSpeakerPosition(speaker));
            }
            else
            {
                _currentTypingBox = (Speakers?.IndexOf(speaker) == playerSpeaker)
                    ? _layout.GenerateDialogueBox(speaker, "left")
                    : _layout.GenerateDialogueBox(speaker, "right");
            }
        }
        _typingCoroutine = StartCoroutine(TypeText(statement));
    }

    public override void HandleInput(string input = null)
    {
        if (!CanContinue()) return;

        if (_isTyping)
        {
            StopTyping();
            return;
        }

        base.HandleInput(input);
    }

    public string GetSpeakerPosition(Speaker speaker)
    {
        string position = string.Empty;
        if (speaker == null) return position;

        position = (speakerPositions.ContainsKey(speaker?.speakerName))
            ? speakerPositions[speaker.speakerName]
            : "left";
        return position;
    }
    #endregion

    #region typing
    IEnumerator TypeText(string statement, Action callback = null)
    {
        if(_currentTypingBox != null)
        {
            _isTyping = true;
            _typingCallback = callback;
            _currentTypingBox.SetText(statement);
            _currentTypingBox.ForceMeshUpdate();
            Debug.Log(_currentTypingBox, _currentTypingBox);
            int total = _currentTypingBox.textInfo.characterCount;
            _currentTypingBox.maxVisibleCharacters = 0;
            for (int i = 0; i < total; i++)
            {
                _currentTypingBox.maxVisibleCharacters++;
                yield return new WaitForSeconds(_typingLetterDelay);
            }
            StopTyping();
        }
    }

    void StopTyping()
    {
        if(_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        _currentTypingBox.maxVisibleCharacters = _currentTypingBox.textInfo.characterCount;
        _typingCallback?.DynamicInvoke();
        _typingCoroutine = null;
        _typingCallback = null;
        _isTyping = false;
    }

    #endregion

    #region backgrounds
    public override void SetBackground(string input, float duration)
    {
        foreach (Sprite background in backgrounds)
        {
            if (background.name == input)
            {
                backgroundSwapCoroutine = StartCoroutine(SwapBackground(_layout.GetBackgroundImage(), background, duration));
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
    #endregion

    #region scripts
    public void RegisterSpeakerPosition(string speaker, string position)
    {
        speakerPositions.Add(speaker, position);
    }
    #endregion

    #region scrolly-bit
    private void Update()
    {
        if (_isTyping)
        {
            _layout.GetScrollRect().verticalNormalizedPosition = 0;
        }
    }
    #endregion
}