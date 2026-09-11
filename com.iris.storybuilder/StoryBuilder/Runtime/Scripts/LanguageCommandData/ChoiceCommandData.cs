using System;
using System.Collections.Generic;

public class ChoiceCommandData : LanguageCommandData
{
    public string Prompt;
    
    public override string CommandName => "choice";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        // Separates prompt from the choice line, if there is one
        string choicePrompt = lines[lineIndex];
        int firstIndex = choicePrompt.IndexOf("choice", StringComparison.OrdinalIgnoreCase) + "choice".Length;

        Prompt = LanguageParser.TrimQuotes(choicePrompt.Substring(
            firstIndex,
            choicePrompt.IndexOf(":", StringComparison.OrdinalIgnoreCase) - firstIndex
        ));

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        Prompt = interpreter.ReplaceVars(Prompt);
    }

    protected override string SerializeInternal()
    {
        string optionalPrompt = string.IsNullOrEmpty(Prompt) ? string.Empty : $" \"{Prompt}\"";
        return $"{CommandName}{optionalPrompt}:";
    }
}
