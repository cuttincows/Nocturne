using System.Collections.Generic;

public class IfCommandData : LanguageCommandData
{
    public string Variable;
    public string Sign;
    public string Value;

    public override string CommandName => "if";

    public IfCommandData()
    {
    }

    public IfCommandData(List<string> lines, ref int lineIndex)
    {
        Deserialize(lines, ref lineIndex);
    }

    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string statement = lines[lineIndex].TrimEnd(':');
        List<string> args = LanguageParser.ParseArgs(statement);

        //if (args.Count == 1) // Single value
        //{
        //    if (args[0] == "True" || args[0] == "False")
        //    {
        //        Variable = null;
        //        Sign = null;
        //        Value = args[0];
        //        return true;
        //    }
        //}
        if (args.Count == 2)
        {
            Variable = null;
            Sign = null;
            Value = args[1];
        }
        else if (args.Count == 3) //Bare boolean check [√]
        {
            Variable = args[1];
            Sign = null;
            Value = null;
        }
        else if (args.Count == 4) //Equality / greater than / less than
        {
            Variable = args[1];
            Sign = args[2];
            Value = args[3];
        }
        
        return true;
    }

    // public bool Deserialize(string rawText)
    // {
    //     // Remove the trailing colon we use to identify if and say syntax
    //     string statement = rawText.TrimEnd(':');
    // 
    //     List<string> args = LanguageParser.ParseArgs(statement);
    // 
    //     if (args.Count == 2) //Bare boolean check [√]
    //     {
    //         Variable = null;
    //         Sign = null;
    //         Value = args[1];
    //     }
    //     else //Equality / greater than / less than
    //     {
    //         Variable = args[1];
    //         Sign = args[2];
    //         Value = args[3];
    //     }
    // 
    //     return true;
    // }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        Variable = interpreter.ReplaceVars(Variable);
        Sign = interpreter.ReplaceVars(Sign);
        Value = interpreter.ReplaceVars(Value);
    }

    protected override string SerializeInternal()
    {
        if (string.IsNullOrEmpty(Variable))
            return $"{CommandName} {Value}:";

        return $"{CommandName} {Variable} {Sign} {Value}:";
    }
}
