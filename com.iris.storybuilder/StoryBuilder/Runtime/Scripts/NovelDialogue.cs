using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NovelDialogue : Dialogue
{
    private TextMeshProUGUI _title;
    private TextMeshProUGUI _subtitle;
    private TextMeshProUGUI _textField;
    private Transform _choicesContainer;
    private Transform _statsContainer;
    private RectTransform _content;
    private Button _next;
    private List<string> _trackedStats = new List<string>();

    public GameObject choiceButtonPrefab;
    public GameObject statBoxPrefab;
    
    private bool typing;
    private TextMeshProUGUI currentTypingField;
    Coroutine typingCoroutine;
    Action typingCallback = null;


    protected override void Start()
    {
        _title = transform.Find("Layout/Canvas/Title").GetComponent<TextMeshProUGUI>();
        _subtitle = transform.Find("Layout/Canvas/Subtitle").GetComponent<TextMeshProUGUI>();
        _textField = transform.Find("Layout/Canvas/Scroll View/Viewport/Content/Text").GetComponent<TextMeshProUGUI>();
        _choicesContainer = transform.Find("Layout/Canvas/Scroll View/Viewport/Content/Choices");
        _statsContainer = transform.Find("Layout/Canvas/Scroll View/Viewport/Content/Stats");
        _content = transform.Find("Layout/Canvas/Scroll View/Viewport/Content") as RectTransform;
        _next = transform.Find("Layout/Canvas/Scroll View/Viewport/Content/Next/Button").GetComponent<Button>();
        base.Start();
    }

    protected override void DoSay(Speaker speaker, string[] args)
    {
        string statement = args[0];

        _choicesContainer.gameObject.SetActive(false);
        string speakerName = speaker?.speakerName;
        string stringToSay = (string.IsNullOrEmpty(speakerName))
            ? statement
            : speakerName + " : " + statement;
        TypeTheText(stringToSay);
        _next.gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        UpdateStats();
    }

    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        //typing = true;
        //_textField.TypeText($"{choicePrompt}", TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
        //{
        //    typing = false;
        //});
        //currentTypingField = _textField;
        ClearOptions();
        _choicesContainer.gameObject.SetActive(false);
        foreach (var choiceText in choiceTexts)
        {
            Transform choiceObject = Instantiate(choiceButtonPrefab).transform;
            choiceObject.SetParent(_choicesContainer);
            choiceObject.GetComponentInChildren<Text>().text = choiceText;
            choiceObject.GetComponent<Button>().onClick.AddListener(()=>
            {
                // TODO: Here there is a bit of user pain as EventSystem "Send Navigation Events" has to be off to prevent this to double send input
                HandleInput(choiceText);
            });
        }
        TypeTheText(choicePrompt, () => {
            _choicesContainer.gameObject.SetActive(true);
        });

        _next.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        UpdateStats();
    }

    //TODO: pool the options?
    private void ClearOptions()
    {
        for (int i = 0; i < _choicesContainer.childCount; i++)
        {
            Destroy(_choicesContainer.GetChild(i).gameObject);
        }
    }

    private void UpdateStats()
    {
        for (int i = 0; i < _statsContainer.childCount; i++)
        {
            Destroy(_statsContainer.GetChild(i).gameObject);
        }

        foreach (var trackedStat in _trackedStats)
        {
            Transform statBox = Instantiate(statBoxPrefab).transform;
            statBox.SetParent(_statsContainer);
            statBox.GetComponentInChildren<TextMeshProUGUI>().text = CurInterpreter.GetValue(trackedStat).ToString();
        }
    }

    internal override void HighlightChoice(int optionIndex)
    {
        _choicesContainer.GetChild(optionIndex).GetComponent<Button>().Select();
    }
    
    public override void StopUsing()
    {
        //gameObject.SetActive(false);
    }
    
    public override void HandleInput(string input = null)
    {
        if (typing)
        {
            StopCoroutine(typingCoroutine);
            _textField.maxVisibleCharacters = _textField.textInfo.characterCount;
            typingCoroutine = null;
            typingCallback?.DynamicInvoke();
            typingCallback = null;
            //currentTypingField.CompleteTyping();
            //currentTypingField = null;
            typing = false;
            return;
        }

        base.HandleInput(input);
    }
    
    
    
    // Script Functions ...

    public void SetTitle(string title)
    {
        _title.text = title;
    }

    public void SetSubtitle(string subtitle)
    {
        _subtitle.text = subtitle;
    }

    public void TrackStatVar(string varName)
    {
        _trackedStats.Add(varName);
    }

    public void PickFood(string foodName) {

    }

    void TypeTheText(string statement, Action callback = null)
    {
        typingCallback = callback;
        typingCoroutine = StartCoroutine(TypeText(statement, callback));
    }

    IEnumerator TypeText(string statement, Action callback = null)
    {
        typing = true;
        _textField.SetText(statement);
        _textField.ForceMeshUpdate();
        _textField.maxVisibleCharacters = 0;
        int total = _textField.textInfo.characterCount;
        for (int i = 0; i < total; i++)
        {
            _textField.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.1f);
        }
        callback?.DynamicInvoke();
        typingCallback = null;
        typingCoroutine = null;
        typing = false;
    }

}
