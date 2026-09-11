using System.Collections.Generic;

public class EndCommandData : LanguageCommandData
{
    public override string CommandName { get; }

    public EndCommandData(string commandName)
    {
        CommandName = commandName;
    }
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
    }

    protected override string SerializeInternal()
    {
        return CommandName;
    }
}
