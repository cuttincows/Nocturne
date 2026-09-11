using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public TextAsset currentScript;

    [ContextMenu("Parse Speakers")]
    public void ParseSpeakers()
    {
        List<string> speakers = GetArgumentFromInstruction("say");
        Debug.Log(speakers.Count);
    }

    [ContextMenu("Parse Backgrounds")]
    public void ParseBackgrounds()
    {
        List<string> backgrounds = GetArgumentFromInstruction("setBackground");
        Debug.Log(backgrounds.Count);
    }

    [ContextMenu("Parse Sounds")]
    public void ParseSounds()
    {
        List<string> sounds = GetArgumentFromInstruction("playSound");
        Debug.Log(sounds.Count);
    }

    List<string> GetArgumentFromInstruction(string instruction)
    {
        List<string> results = new List<string>();
        List<int> sets = currentScript.text.AllIndexesOf(instruction);
        foreach (int i in sets)
        {

            if (currentScript.text.Substring(i + instruction.Length + 1, 1).Equals("\"")) continue;
            if (!currentScript.text.Substring(i - 1, 1).Equals("\t")) continue;
            string instance = currentScript.text.Substring(i + instruction.Length + 1, currentScript.text.IndexOf(" ", i + instruction.Length + 1) - (i + instruction.Length));
            if (!results.Contains(instance))
            {
                results.Add(instance);
            }
        }
        return results;
    }
}