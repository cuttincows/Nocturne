using System;
using System.Collections.Generic;

public class SetBackgroundCommandData : LanguageCommandData
{
    public string SpriteName;
    public string Duration;

    public override string CommandName => "setBackground";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string lineText = lines[lineIndex];
        List<string> args = LanguageParser.ParseArgs(lineText.Substring(lineText.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1));
        SpriteName = args[0];
        Duration = args.Count > 1 ? args[1] : null;
        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        SpriteName = interpreter.ReplaceVars(SpriteName);
        Duration = interpreter.ReplaceVars(Duration);
    }

    protected override string SerializeInternal()
    {
        string optionalDuration = string.IsNullOrEmpty(Duration) ? string.Empty : $" {Duration}"; 
        return $"{CommandName} {SpriteName}{optionalDuration}";
    }
}
