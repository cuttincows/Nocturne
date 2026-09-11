using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlatDialogue : Dialogue
{
    private string textHistory = "";
    private List<string> choiceTexts;
    
    public TextMeshProUGUI textField;
    
    private void PrintOptions(int highlight = -1)
    {
        for (int i = 0; i<choiceTexts.Count;i++)
        {
            textField.text += "["+(i == highlight ? "*" : " " )+"] "+choiceTexts[i]+"\n";
        }
    }

    protected override void DoSay(Speaker speaker, string[] args)
    {
        string statement = args[0];
        string speakerName = speaker!=null ? speaker.speakerName : "";
        AddLine($"{speakerName}{(string.IsNullOrEmpty(speakerName)?"":": ")} {statement}");
    }

    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        AddLine($"**{choicePrompt}**");
        this.choiceTexts = choiceTexts;
        PrintOptions();
    }

    internal override void HighlightChoice(int choiceIndex)
    {
        textField.text = textHistory;
        PrintOptions(choiceIndex);
    }

    public override void HandleInput(string input = null)
    {
        if (input != null)
        {
            AddLine(input);
            AddLine("****");
        }
        base.HandleInput(input);
    }

    public override void StopUsing()
    {
        gameObject.SetActive(false);
    }

    public void AddLine(string line)
    {
        textHistory += line+"\n";
        textField.text = textHistory;
    }
}
