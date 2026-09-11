using System.Collections.Generic;

public class GoToCommandData : LanguageCommandData
{
    public string LabelName;
    
    public override string CommandName => "goto";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string lineText = lines[lineIndex];
        LabelName = lineText.Substring(lineText.IndexOf(" ") + 1);
        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        LabelName = interpreter.ReplaceVars(LabelName);
    }

    protected override string SerializeInternal()
    {
        return $"{CommandName} {LabelName}";
    }
}
