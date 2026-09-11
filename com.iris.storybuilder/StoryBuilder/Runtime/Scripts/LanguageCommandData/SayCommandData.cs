using System;
using System.Collections.Generic;
using System.Text;

public class SayCommandData : LanguageCommandData
{
    public string SpeakerName;
    public string[] Statement;
    public bool Toggle;

    public SayCommandData(bool toggle)
    {
        Toggle = toggle;
    }

    public override string CommandName => Toggle ? "sayToggle" : "say";

    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string statement = lines[lineIndex];
        List<string> args = LanguageParser.ParseArgs(statement);
        string v = args[1];
        // TODO: Implement speaker arg for triple quoted say
        if (v.Contains("\"\"\""))
        {
            do
            {
                lineIndex++;
                v += $" {lines[lineIndex].TrimStart('\t')}";
            } while (!lines[lineIndex].Contains("\"\"\""));

            SpeakerName = string.Empty;
            Statement = new[] {v.Trim('\"')};
            return true;
        }
        
        // Single line, additive lines
        while (HasMultilineSeparator(lines[lineIndex]))
        {
            v = v.Substring(0, v.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase));
            v += lines[++lineIndex].TrimStart('\t');
        }
        
        if (args.Count > 2)
        {
            SpeakerName = args[1];
            var trailingArgs = args.GetRange(2, args.Count - 2);
            trailingArgs.ForEach((s) => LanguageParser.TrimQuotes(s));
            Statement = trailingArgs.ToArray();
        }
        else
        {
            SpeakerName = string.Empty;
            Statement = new[] {LanguageParser.TrimQuotes(v)};
        }

        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        SpeakerName = interpreter.ReplaceVars(SpeakerName);
        
        for (var i = 0; i < Statement.Length; i++)
        {
            Statement[i] = interpreter.ReplaceVars(Statement[i]);
        }
    }

    protected override string SerializeInternal()
    {
        string optionalSpeakerName = string.IsNullOrEmpty(SpeakerName) ? string.Empty : $" {SpeakerName}";
        StringBuilder builder = new StringBuilder();
        builder.Append(CommandName);
        builder.Append(optionalSpeakerName);
        foreach (var s in Statement)
        {
            builder.Append($" \"{s}\"");
        }

        return builder.ToString();
    }

    private bool HasMultilineSeparator(string line)
    {
        char lastChar = line[line.Length - 1];
        return lastChar == '\\' || lastChar == '+';
    }
}
