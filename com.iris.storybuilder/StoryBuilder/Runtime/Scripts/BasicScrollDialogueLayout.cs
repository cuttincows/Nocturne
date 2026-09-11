using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Speaker;

public class BasicScrollDialogueLayout : BaseDialogueLayout
{
    [SerializeField] ScrollRect _scrollRect = default;
    [SerializeField] Transform _dialogueBoxContainer = default;
    //[SerializeField] TextMeshProUGUI _dialogueTextField = default;
    [SerializeField] GameObject _choiceStatementBox = default;
    [SerializeField] TextMeshProUGUI _choicesStatement = default;
    [SerializeField] RectTransform _choicesContainer = default;
    [SerializeField] Image _backgroundImage = default;
    [SerializeField] GameObject _choiceButtonEven = default;
    [SerializeField] GameObject _choiceButtonOdd = default;
    public SpeakerUI leftPrefab;
    public SpeakerUI rightPrefab;
    public GameObject textBoxRightPrefab;
    public GameObject textBoxLeftPrefab;
    public GameObject narrativeBoxPrefab;

    void OnEnable()
    {
        _choiceStatementBox.SetActive(true);
        //_choicesStatement.transform.parent.gameObject.SetActive(false);
    }
    #region BaseDialogueLayout
    public ScrollRect GetScrollRect()
    {
        return _scrollRect;
    }

    public override Image GetBackgroundImage()
    {
        return _backgroundImage;
    }

    public override GameObject GetChoiceButton(int choiceNumber, int totalChoices)
    {
        return (choiceNumber % 2 == 0) ? _choiceButtonEven : _choiceButtonOdd;
    }

    public override RectTransform GetChoicesContainer()
    {
        return _choicesContainer;
    }

    public override TextMeshProUGUI GetDialogueBox()
    {
        return null;
    }
    public override void SetPrimarySpeaker(Speaker speaker, SpeakerSide speakerSide)
    {
        //int primaryIndex = -1;
        //for (int i = 0; i < _dialogue.speakers.Count; i++)
        //{
        //    if (_dialogue.speakers[i].Equals(speaker))
        //    {
        //        primaryIndex = i;
        //    }
        //}
        //if(primaryIndex > 0)
        //{
        //    _dialogue.playerSpeaker = primaryIndex;
        //}
    }

    public override void SetSpeakerImages(List<Speaker> speakers)
    {
    }

    public override void ShowSpeaker(Speaker speaker)
    {
    }
    #endregion

    public GameObject GetTheDialogueBox()
    {
        return _dialogueBoxContainer.gameObject;
    }

    public TextMeshProUGUI GenerateDialogueBox(Speaker speaker = null, string position = null)
    {
        GameObject textBox;
        if (speaker != null)
        {
            GameObject textBoxPrefab = (position == "left")
                ? textBoxLeftPrefab
                : textBoxRightPrefab;
            textBox = Instantiate(textBoxPrefab, _dialogueBoxContainer);
        }
        else
        {
            textBox = Instantiate(narrativeBoxPrefab, _dialogueBoxContainer);
        }

        //Fix make SpeakerUI to get without  GetComponeents
        //Add SpeakerUI to NarrativePrefab
        SpeakerUI ui = textBox.GetComponent<SpeakerUI>();
        if (ui != null)
        {
            ui.SetPortrait(speaker?.portrait);
        }

        return textBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ClearChoices()
    {
        for (int i = 0; i < _choicesContainer.childCount; i++)
        {
            Destroy(_choicesContainer.GetChild(i).gameObject);
        }
    }

    public void PrepareSay()
    {
        _choiceStatementBox.SetActive(false);
        _choicesContainer.gameObject.SetActive(false);
        _dialogueBoxContainer.gameObject.SetActive(true);
    }

    public void PrepareChoice()
    {
        _dialogueBoxContainer.gameObject.SetActive(false);
        _choicesContainer.gameObject.SetActive(false);
        _choiceStatementBox.SetActive(true);
        ClearChoices();
    }

    public void AddChoices(List<string> choiceTexts, System.Action<string> callback = null)
    {
        int i = 0;
        foreach (var choiceText in choiceTexts)
        {
            Transform choiceObject = Instantiate(i % 2 == 0 ? _choiceButtonEven : _choiceButtonOdd).transform;
            choiceObject.SetParent(_choicesContainer);
            choiceObject.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            choiceObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                // TODO: Here there is a bit of user pain as EventSystem "Send Navigation Events" has to be off to prevent this to double send input
                callback?.DynamicInvoke(choiceText);
                _choiceStatementBox.SetActive(false);
                //_choicesContainer.gameObject.SetActive(false);
            });
            i++;
        }
        _choicesContainer.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_choicesContainer);
    }

    public TextMeshProUGUI GetChoicesTextBox()
    {
        return _choicesStatement;
    }

    public override void SetSpeakerEmotion(Speaker speaker, string emotion) {
        Debug.LogException(
            new System.Exception("SetSpeakerEmotion not implemented for BasicScrollDialogueLayout")
        );
    }
}