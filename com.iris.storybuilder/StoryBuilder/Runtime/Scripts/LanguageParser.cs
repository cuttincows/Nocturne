using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class BlockStringData
{
    public string BlockName;
    public List<string> Lines;
}

public class BlockLanguageCommandData
{
    public string BlockName;
    public List<LanguageCommandData> Commands;
}

public class LanguageData
{
    public List<BlockLanguageCommandData> Blocks;
    public Dictionary<string, Dictionary<string, int>> Labels;
}

public static class LanguageParser
{
    /***************************************************************
     *  Generally speaking, you shouldn't need anything from here  *
     *  to add functionality, but feel free to look through if     *
     *  you want to know how it all works.                         *
     ***************************************************************/

    //TODO: Skip to endblock + 1 on fallthrough

    #region regex
    private const string blockRegex = @"(?=\s{0,10})block\s(.{1,128})";
    private const string endBlockRegex = @"(?=\s{0,10})endblock";
    #endregion

    public static LanguageData ParseLanguageData(List<TextAsset> scriptFiles)
    {
        var blockStrings = SplitText(scriptFiles);
        var labels = ParseLabels(blockStrings);
        var blocks = ParseBlocks(blockStrings);

        return new LanguageData {Blocks = blocks, Labels = labels};
    }
    
    public static LanguageData ParseLanguageData(List<BlockStringData> blockStringData)
    {
        var labels = ParseLabels(blockStringData);
        var blocks = ParseBlocks(blockStringData);

        return new LanguageData {Blocks = blocks, Labels = labels};
    }

    private static string FirstWord(string line)
    {
        line = ClearTabs(line);
        return line.IndexOf(" ") != -1 ? line.Substring(0, line.IndexOf(" ")) : line;
    }

    #region Line by line script reading

    public static List<BlockLanguageCommandData> ParseBlocks(List<BlockStringData> blocks)
    {
        var retVal = new List<BlockLanguageCommandData>(blocks.Count);
        foreach (var block in blocks)
        {
            var parents = new Stack<LanguageCommandData>();
            var blockData = new BlockLanguageCommandData {BlockName = block.BlockName, Commands = new List<LanguageCommandData>()};
            var blockLines = block.Lines;
            for (var i = 0; i < blockLines.Count; i++)
            {
                var lineText = blockLines[i];
                int indentLevel = IndentLevel(lineText);

                while(parents.Count > 0 && parents.Count >= indentLevel)
                    parents.Pop();
                
                LanguageCommandData languageCommandData = null;
                LanguageCommandData commandParent = null;
                if (parents.Count > 0)
                    commandParent = parents.Peek();

                if (commandParent is ChoiceCommandData)
                {
                    languageCommandData = new ChoiceOptionCommandData();
                    parents.Push(languageCommandData);
                }
                else
                {
                    string fWord = FirstWord(lineText);
                    if (!string.IsNullOrEmpty(fWord) && fWord[fWord.Length - 1] == ':')
                        fWord = fWord.Substring(0, fWord.Length - 1);

                    switch (fWord)
                    {
                        case "":
                            break;
                        case "block":
                            break;
                        case "if":
                            languageCommandData = new IfCommandData();
                            parents.Push(languageCommandData);
                            break;
                        case "jump":
                            languageCommandData = new JumpCommandData();
                            break;
                        case "say":
                            languageCommandData = new SayCommandData(false);
                            break;
                        case "sayToggle":
                            languageCommandData = new SayCommandData(true);
                            break;
                        case "trace":
                        case "print":
                        case "cout":
                        case "echo":
                        case "Log":
                        case "Debug.Log":
                            languageCommandData = new PrintCommandData(fWord);
                            break;
                        case "function":
                        case "fun":
                        case "run":
                            languageCommandData = new RunCommandData(fWord);
                            break;
                        case "end":
                        case "endscript":
                        case "endblock":
                            languageCommandData = new EndCommandData(fWord);
                            break;
                        case "choice":
                            languageCommandData = new ChoiceCommandData();
                            parents.Push(languageCommandData);
                            break;
                        case "var":
                            languageCommandData = new VarCommandData();
                            break;
                        case "label":
                            break;
                        case "goto":
                            languageCommandData = new GoToCommandData();
                            break;
                        case "setBackground":
                            languageCommandData = new SetBackgroundCommandData();
                            break;
                        case "playSound":
                            languageCommandData = new PlaySoundCommandData();
                            break;
                        // case "pause":
                        //     languageCommandData = new PauseCommandData()
                        //     break;
                        case "//": case "#":
                            break;
                        default:
                            Debug.LogError("Command " + fWord + " not found");
                            break;
                    }
                }

                if (languageCommandData == null)
                    continue;
                
                languageCommandData.Indent = indentLevel;
                languageCommandData.Deserialize(blockLines, ref i);
                blockData.Commands.Add(languageCommandData);
            }
            
            retVal.Add(blockData);
        }

        return retVal;
    }
    #endregion

    #region Parsing functions
    
    public static List<string> ParseArgs(string statement) {
        List<string> parsedArgs = new List<string>();
        statement = statement.Trim();

        // --Parse out the values for function
        while (statement.Length > 0) {
            // Handles strings surrounded by quotes
            int curQuoteIndex = NextIndexOfUnescapedQuote(statement, 0);
            if (statement.IndexOf("\"") == 0) {
                statement = statement.Substring(1);
				
                int nextIndex = NextIndexOfUnescapedQuote(statement, curQuoteIndex);
                //Debug.Log($"{nextIndex}, {statement.Length}, {statement}");
                if (nextIndex == -1)
                {
                    //Debug.Log("Hmmmm, endblock.");
                    continue;
                }
                parsedArgs.Add(statement.Substring(0, nextIndex));
                statement = statement.Substring(nextIndex + 1);
                if (statement.Length > 0 && statement.Substring(0, 1) == " ") //Clears spaces between arguments
                    statement = statement.Substring(1);
                continue;
            }
            //Handles standard vars / nums
            else if (statement.IndexOf(" ") == -1) {
                parsedArgs.Add(statement);
                statement = "";
            } else {
                parsedArgs.Add(statement.Substring(0, statement.IndexOf(" ")));
                statement = statement.Substring(statement.IndexOf(" ") + 1);
            }
        }

        return parsedArgs;
    }
    
    private static int NextIndexOfUnescapedQuote(string s, int startIndex) {
        int curQuoteIndex = s.IndexOf("\"", startIndex);
        if (curQuoteIndex < 1) {
            return curQuoteIndex;
        }
        while (s[curQuoteIndex - 1] == '\\') {
            curQuoteIndex = s.IndexOf("\"", curQuoteIndex + 1);
        }
        return curQuoteIndex;
    }

    // TODO: handle multiline comments as single line for saving
    public static List<BlockStringData> SplitText(List<TextAsset> scriptFiles)
    {
        if (scriptFiles.Count == 0) {
            Debug.LogException(new Exception("Cannot run an interpreter without an input file"));
        }

        var blockDictionary = new Dictionary<string, List<string>>();
        var blocks = new List<BlockStringData>();
        Regex blankLineRegex = new Regex(@"^[ \t\r\n]*$");

        foreach (TextAsset scriptFile in scriptFiles) {

            //if (m_blocks.ContainsKey(scriptFile.name))
                //throw new Exception("The language interpreter cannot handle multiple input files with the same name");
            //m_blocks[scriptFile.name] = new List<string>();
            string[] physicalLines = scriptFile.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < physicalLines.Length; i++) {
                string physicalLine = physicalLines[i];
                physicalLine = physicalLine.Replace("    ", "\t");

                if (physicalLine == " ") continue;
                if (physicalLine == "" || ClearTabs(physicalLine) == "" || blankLineRegex.Match(physicalLine).Success || FirstWord(physicalLine)[0] == '#')
                    continue;
                if (physicalLine == "endblock" || ClearTabs(physicalLine) == "endblock")
                {
                    break;  // NOTE: This will enforce 1 block per script (?)
                }

                // Clears comments, ignoring escaped hashes
                while (physicalLine.LastIndexOf("#") != -1 && (physicalLine[physicalLine.LastIndexOf("#") - 1] != '\\')) {
                    physicalLine = physicalLine.Substring(0, physicalLine.LastIndexOf("#"));
                    while (physicalLine[physicalLine.Length - 1] == ' ')
                        physicalLine = physicalLine.Substring(0, physicalLine.Length - 1);
                }
                while (physicalLine.IndexOf("\\#") != -1) {
                    physicalLine = physicalLine.Substring(0, physicalLine.IndexOf("\\#"))
                        + (physicalLine.IndexOf("\\#") != physicalLine.Length ? physicalLine.Substring(physicalLine.IndexOf("\\#") + 1) : "");
                }

                // Add the stripped line to the statement registry
                //m_statements[scriptFile.name].Add(physicalLine);

                if (physicalLine.IndexOf("\"\"\"") != -1) {
                    i++;
                    // Add lines until the ending triple quotes are found
                    // [Q] Should I throw an error if no ending quotes are found, or 
                    while (physicalLine.IndexOf("\"\"\"") == -1) {
                        blockDictionary[scriptFile.name][blockDictionary[scriptFile.name].Count - 1]
                            += (physicalLine);
                        i++;
                    }
                }
            }

            // Once the lines are processed, split them by blocks
            List<BlockStringData> splitRes = SplitBlocks(physicalLines);
            foreach(var block in splitRes) {
                if (blockDictionary.ContainsKey(block.BlockName)) {
                    Debug.LogError("Duplicate block name " + block.BlockName + " found - this ambiguity is not allowed");
                    continue;
                }
                blockDictionary[block.BlockName] = block.Lines;
                blocks.Add(block);
            }
        }

        return blocks;
    }

    // Splits a text file into the blocks that make it up
    private static List<BlockStringData> SplitBlocks(string[] lines) {
        var blocks = new List<BlockStringData>();

        Regex blockRegexObj = new Regex(blockRegex);
        Regex endBlockRegexObj = new Regex(endBlockRegex);
        int i = 0;
        while (i < lines.Length) {
            // Find the start of a block
            Match blockStartMatch = blockRegexObj.Match(lines[i]);
            i++;  // Start at the first line of the block
            if (!blockStartMatch.Success) continue;
            
            string blockName = blockStartMatch.Groups[1].Value.TrimEnd(':');
            var blockLines = new List<string>();

            Match blockEndMatch = endBlockRegexObj.Match(lines[i]);
            // Find the end of a block
            while (!blockEndMatch.Success && i < lines.Length) {
                blockLines.Add(lines[i]);
                blockEndMatch = blockRegexObj.Match(lines[i]);
                i++;
            }
            i--;  // Adjust for moving past the block start
            blocks.Add(new BlockStringData {BlockName = blockName, Lines = blockLines});
        }

        return blocks;
    }

    public static Dictionary<string, Dictionary<string, int>> ParseLabels(List<BlockStringData> statements) {
        Dictionary<string, Dictionary<string, int>> parsedLabels = new Dictionary<string, Dictionary<string, int>>();

        foreach (var block in statements) {
            for (int lineNum = 0; lineNum < block.Lines.Count; lineNum++) {
                string lineVal = block.Lines[lineNum];
                string fWord = FirstWord(lineVal);
                if (fWord == "label") {
                    if (parsedLabels.ContainsKey(fWord)) {
                        Debug.LogError("Duplicate label " + fWord + " found. This scares and confuses StoryBuilder.");
                    }
                    if (!parsedLabels.ContainsKey(block.BlockName)) {
                        parsedLabels[block.BlockName] = new Dictionary<string, int>();
                    }
                    parsedLabels[block.BlockName][lineVal.Substring(lineVal.IndexOf(" ") + 1)] = lineNum;
                }
            }
        }
        return parsedLabels;
    }

    public static string TrimQuotes(string value)
    {
        if (value == "")  // Probably an error, debug print
        {
            // This should be active if we're typing at all
            //var interpreterData = GameObject.FindObjectOfType<LanguageInterpreter>(true).SaveData as InterpreterState;
            //Debug.LogError($"Failed TrimQuotes: {interpreterData.ActiveBlockName}: {interpreterData.LineNum}");
            return "";
        }
        // Check for and cut surrounding qoutes
        if (value[0] == '"' && value[value.Length - 1] == '"') {
            value = value.Substring(1, value.Length - 2);
        }
        // All quotes left are containing or escaped quotes
        while (value.IndexOf("\\\"") != -1)
            value = value.Substring(0, value.IndexOf("\\\"")) + value.Substring(value.IndexOf("\\\"") + 1);
        return value;
    }

    private static int IndentLevel(string line)
    {
        for (int i = 0; i < line.Length; i++)
            if (line[i] != '\t')
                return i;
        return 0;
    }

    private static string ClearTabs(string line)
    {
        if (line != "")
        {
            while (line.Length > 0 && line[0] == '\t')
                line = line.Substring(1);
        }
        return line;
    }

    #endregion

}
