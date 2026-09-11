using System;
using System.Collections.Generic;
using System.Text;

public class RunCommandData : LanguageCommandData
{
    public string FunctionName;
    public List<string> Arguments;
    public override string CommandName { get; }

    public RunCommandData(string commandName)
    {
        CommandName = commandName;
    }
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string lineText = lines[lineIndex];
        string statement = lineText.Substring(lineText.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1);
        // Parses out function name and statement, handling 0 parameter cases
        int spaceIndex = statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
        FunctionName = spaceIndex != -1 ? statement.Substring(0, spaceIndex) : statement;
        statement = spaceIndex != -1 ? statement.Substring(statement.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) : "";

        Arguments = LanguageParser.ParseArgs(statement);

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        FunctionName = interpreter.ReplaceVars(FunctionName);
        for (var i = 0; i < Arguments.Count; i++)
        {
            Arguments[i] = interpreter.ReplaceVars(Arguments[i]);
        }
    }

    protected override string SerializeInternal()
    {
        StringBuilder arguments = new StringBuilder();
        foreach (var argument in Arguments)
        {
            arguments.Append($" {argument}");
        }
        
        return $"{FunctionName} {arguments}";
    }
}
