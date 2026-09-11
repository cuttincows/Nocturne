using System;
using System.Collections.Generic;

public class VarCommandData : LanguageCommandData
{
    public string Variable;
    public string Sign;
    public string Value;
    
    public override string CommandName => "var";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string statement = lines[lineIndex];
        statement = statement.Substring(statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
        Variable = statement.Substring(0, statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase));
        statement = statement.Substring(statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
        Sign = statement.Substring(0, statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase));
        Value = statement.Substring(statement.IndexOf(Sign, StringComparison.OrdinalIgnoreCase) + 2);

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        Variable = interpreter.ReplaceVars(Variable);
        Sign = interpreter.ReplaceVars(Sign);
        Value = interpreter.ReplaceVars(Value);
    }

    protected override string SerializeInternal()
    {
        return $"{CommandName} {Variable} {Sign} {Value}";
    }
}
