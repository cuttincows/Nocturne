using System;
using System.Collections.Generic;

public class PlaySoundCommandData : LanguageCommandData
{
    public string SoundName;
    public string Volume;
    
    public override string CommandName => "playSound";
    
    public override bool Deserialize(List<string> lines, ref int lineIndex)
    {
        string lineText = lines[lineIndex];
        List<string> args = LanguageParser.ParseArgs(lineText.Substring(lineText.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1));
        SoundName = args[0];
        Volume = args.Count > 1 ? args[1] : null;
        return true;
    }

    public override void ReplaceVars(LanguageInterpreter interpreter)
    {
        SoundName = interpreter.ReplaceVars(SoundName);
        Volume = interpreter.ReplaceVars(Volume);
    }

    protected override string SerializeInternal()
    {
        string optionalVolume = string.IsNullOrEmpty(Volume) ? string.Empty : $" {Volume}";
        return $"{CommandName} {SoundName}{optionalVolume}";
    }
}
