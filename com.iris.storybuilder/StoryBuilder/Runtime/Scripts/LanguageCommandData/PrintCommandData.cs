using System;
using System.Collections.Generic;

public class PrintCommandData : LanguageCommandData
{
    public string Message;
    public override string CommandName { get; }

    public PrintCommandData(string commandName)
    {
        CommandName = commandName;
    }
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string lineText = lines[lineIndex];
        Message = lineText.Substring(lineText.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        Message = interpreter.ReplaceVars(Message);
    }

    protected override string SerializeInternal()
    {
        return $"{CommandName} {Message}";
    }
}
