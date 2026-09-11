using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StoryBuilderAssets
{
    public List<string> Speakers = new List<string>();
    public List<string> BackgroundSprites = new List<string>();
    public List<string> SoundClips = new List<string>();
    public List<string> RunCommands = new List<string>();
    public override string ToString()
    {
        StringBuilder assetList = new StringBuilder();
        assetList.Append("Speakers: ");
        assetList.Append(Speakers.Count);
        assetList.Append("\nSprites: ");
        assetList.Append(BackgroundSprites.Count);
        assetList.Append("\nAudioClips: ");
        assetList.Append(SoundClips.Count);
        assetList.Append("\nCustomMethods: ");
        assetList.Append(RunCommands.Count);
        return assetList.ToString();
    }
}

public static class StoryBuilderAssetLister
{
    public static StoryBuilderAssets GetAssetList(TextAsset story)
    {
        StoryBuilderAssets assets = new StoryBuilderAssets();
        var result = LanguageParser.ParseLanguageData(new List<TextAsset>() { story });
        foreach (BlockLanguageCommandData block in result.Blocks)
        {
            foreach (LanguageCommandData command in block.Commands)
            {
                switch (command.CommandName)
                {
                    case "say":
                        SayCommandData sayCommandData = (SayCommandData)command;
                        if (sayCommandData == null) break;
                        if (string.IsNullOrEmpty(sayCommandData.SpeakerName)) break;
                        if (assets.Speakers.Contains(sayCommandData.SpeakerName)) break;
                        assets.Speakers.Add(sayCommandData.SpeakerName);
                        break;
                    case "playSound":
                        PlaySoundCommandData soundData = (PlaySoundCommandData)command;
                        if (soundData == null) break;
                        if (string.IsNullOrEmpty(soundData.SoundName)) break;
                        if (assets.SoundClips.Contains(soundData.SoundName)) break;
                        assets.SoundClips.Add(soundData.SoundName);
                        break;
                    case "run":
                        RunCommandData runData = (RunCommandData)command;
                        if (runData == null) break;
                        if (string.IsNullOrEmpty(runData.FunctionName)) break;
                        if (assets.RunCommands.Contains(runData.FunctionName)) break;
                        assets.RunCommands.Add(runData.FunctionName);
                        break;
                    case "setBackground":
                        SetBackgroundCommandData backgroundData = (SetBackgroundCommandData)command;
                        if (backgroundData == null) break;
                        if (string.IsNullOrEmpty(backgroundData.SpriteName)) break;
                        if (assets.BackgroundSprites.Contains(backgroundData.SpriteName)) break;
                        assets.BackgroundSprites.Add(backgroundData.SpriteName);
                        break;
                    default:
                        break;
                }
            }
        }
        return assets;
    }
}