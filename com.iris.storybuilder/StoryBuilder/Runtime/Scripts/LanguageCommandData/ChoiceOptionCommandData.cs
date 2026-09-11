using System;
using System.Collections.Generic;

public class ChoiceOptionCommandData : LanguageCommandData
{
    public string Text;
    public override string CommandName => string.Empty;

    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        Text = FormatChoice(lines[lineIndex]);

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        Text = interpreter.ReplaceVars(Text);
    }

    protected override string SerializeInternal()
    {
        return $"\"{Text}\":";
    }
    
    private static string FormatChoice(string choice)
    {
        var args = new List<string>();
        if (choice[choice.Length - 1] == ' ')
            while (choice.Length > 0 && choice[choice.Length - 1] == ' ')
                choice = choice.Substring(0, choice.Length - 1);

        while (choice.Length > 0 && choice[0] == '\t')
            choice = choice.Substring(1);
        if (choice.LastIndexOf(":", StringComparison.OrdinalIgnoreCase) == choice.Length - 1)
            choice = choice.Substring(0, choice.Length - 1);
        else
        {
            args.Add("");
            return "";
        }

        while (choice.Length > 0)
        {
            //Handles strings surrounded by quotes
            if (choice.IndexOf("\"", StringComparison.OrdinalIgnoreCase) == 0)
            {
                choice = choice.Substring(1);
                args.Add(choice.Substring(0, choice.IndexOf("\"", StringComparison.OrdinalIgnoreCase)));
                choice = choice.Substring(choice.IndexOf("\"", 1, StringComparison.OrdinalIgnoreCase) + 1);
                //Clears spaces between arguments
                if (choice.Length > 0 && choice.Substring(0, 1) == " ")
                    choice = choice.Substring(1);
                continue;
            }
        }

        return args[0];
    }
}
