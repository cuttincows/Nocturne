using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollDialogue : Dialogue
{

    public GameObject textBoxRightPrefab;
    public GameObject textBoxLeftPrefab;

    public GameObject narrativeBoxPrefab;
    
    public GameObject choiceButtonPrefab;
    public GameObject choiceButtonOddPrefab;
    
    private RectTransform _content;
    private RectTransform _choices;
    private RectTransform _choicesStatement;
    private RectTransform _choicesContainer;
    private ScrollRect _scrollRect;

    private Dictionary<string, string> speakerPositions = new Dictionary<string, string>();

    private bool typing;
    private TextMeshProUGUI currentTypingField;

    protected override void Start()
    {
        _content = transform.Find("Layout/Canvas/Scroll View/Viewport/Content") as RectTransform;
        _choices = transform.Find("Layout/Canvas/Choices") as RectTransform;
        _choicesStatement = transform.Find("Layout/Canvas/Choices/Statement") as RectTransform;
        _choicesContainer = transform.Find("Layout/Canvas/Choices/Choices Container") as RectTransform;
        _scrollRect = _content.GetComponentInParent<ScrollRect>();
        ClearBoxes();
        base.Start();
    }

    protected override void DoSay(Speaker speaker, string[] args)
    {
        string statement = args[0];

        _choices.gameObject.SetActive(false);
        _content.gameObject.SetActive(true);
        
        typing = true;

        GameObject textBox;
        if (speaker != null)
        {
            string speakerName = speaker.speakerName;
            string position;
            if (!speakerPositions.TryGetValue(speakerName, out position))
                position = "left";
            GameObject textBoxPrefab = position == "left" ? textBoxLeftPrefab : textBoxRightPrefab;
            textBox = Instantiate(textBoxPrefab,_content);
        }
        else
        {
            textBox = Instantiate(narrativeBoxPrefab, _content);
        }
        currentTypingField = textBox.GetComponentInChildren<TextMeshProUGUI>();
        
        currentTypingField.TypeText(statement, TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
            {
                typing = false;
            });
        if (speaker != null)
        {
            textBox.transform.Find("Image").GetComponent<Image>().sprite = speaker.portrait;
        }
    }

    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        _choices.gameObject.SetActive(true);
        _content.gameObject.SetActive(false);
        
        ClearOptions();
        if (!string.IsNullOrWhiteSpace(choicePrompt))
        {
            typing = true;
            _choicesStatement.gameObject.SetActive(true);
            currentTypingField = _choicesStatement.GetComponentInChildren<TextMeshProUGUI>();
            currentTypingField.TypeText($"{choicePrompt}",TextTyper.DEFAULT_TYPE,TextTyper.DEFAULT_SPEED, () => AddChoices(choiceTexts));
        }
        else
        {
            _choicesStatement.gameObject.SetActive(false);
            AddChoices(choiceTexts);
        }
        
        
    }

    private void AddChoices(List<string> choiceTexts)
    {
        int i = 0;
        foreach (var choiceText in choiceTexts)
        {
            Transform choiceObject = Instantiate(i%2==0?choiceButtonPrefab:choiceButtonOddPrefab).transform;
            choiceObject.SetParent(_choicesContainer);
            choiceObject.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            choiceObject.GetComponent<Button>().onClick.AddListener(()=>
            {
                // TODO: Here there is a bit of user pain as EventSystem "Send Navigation Events" has to be off to prevent this to double send input
                HandleInput(choiceText);
            });
            i++;
        }
        _choicesContainer.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_choices);

    }

    public override void HandleInput(string input = null)
    {
        if (typing)
        {
            currentTypingField.CompleteTyping();
            currentTypingField = null;
            typing = false;
            return;
        }

        base.HandleInput(input);
    }
    
    internal override void HighlightChoice(int optionIndex)
    {
        _choicesContainer.GetChild(optionIndex).GetComponent<Button>().Select();
    }

    private void ClearBoxes()
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }
    
    //TODO: pool the options?
    private void ClearOptions()
    {
        for (int i = 0; i < _choicesContainer.childCount; i++)
        {
            Destroy(_choicesContainer.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        if (typing)
        {
            _scrollRect.verticalNormalizedPosition = 0;
        }
    }

    // Script methods

    public void RegisterSpeakerPosition(string speaker, string position)
    {
        speakerPositions.Add(speaker,position);
    }
}
