using System;
using System.Collections.Generic;

public class JumpCommandData : LanguageCommandData
{
    public string BlockName;

    public override string CommandName => "jump";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string statement = lines[lineIndex];
        string clearedTabs = statement.TrimStart('\t');
        BlockName = clearedTabs.Substring(clearedTabs.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1,
            clearedTabs.Length - 5);

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        BlockName = interpreter.ReplaceVars(BlockName);
    }

    protected override string SerializeInternal()
    {
        return $"{CommandName} {BlockName}";
    }
}
