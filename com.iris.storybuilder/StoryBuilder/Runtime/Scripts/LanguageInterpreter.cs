using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class LanguageInterpreter : Interpreter
{
    [HideInInspector] public new const string SAVE_PREFIX = "dialogueSave";
    /***************************************************************
     *  Generally speaking, you shouldn't need anything from here  *
     *  to add functionality, but feel free to look through if     *
     *  you want to know how it all works.                         *
     ***************************************************************/

    //TODO: Skip to endblock + 1 on fallthrough

    public List<TextAsset> scriptFiles;

    public bool saveOnQuit = false;

    public static string SaveName = "";

    [HideInInspector]
    public Dialogue m_Dialogue = null;

    protected Dictionary<string, List<LanguageCommandData>> m_blocks;
    
    // <block_name, (label_name, statement_index)>
    protected Dictionary<string, Dictionary<string, int>> m_labels;

    protected InterpreterState curState;
    public override object SaveData { get { return curState; } }
    
    [HideInInspector]
    public bool shouldBreak = false;
    public bool shouldLoad = false;

    protected bool stoppingScript = false;

    #region regex
    protected string blockRegex = @"(?=\s{0,10})block\s(.{1,128})";
    protected string endBlockRegex = @"(?=\s{0,10})endblock";
    #endregion

    #region Setup Initialization and public functionality

    public bool Initialized { get { return curState != null; } }

    public override void Initialize()
    {
        m_blocks = new Dictionary<string, List<LanguageCommandData>>();
        if (m_Dialogue == null)
            m_Dialogue = GetComponent<Dialogue>();
        string startingBlock = ParseScripts();

        // Loads Interpreter state from file
        // TODO: Resumes previously left off location 
        if (shouldLoad)
            Load();
        else
            curState = new InterpreterState(startingBlock);
    }

    /// <summary>
    /// Parses the current contents of this.scriptFiles
    /// Returns: The first block, not always needed
    /// </summary>
    public string ParseScripts()
    {
        m_blocks.Clear();  // Rest, fresh read from files
        LanguageData languageData = LanguageParser.ParseLanguageData(scriptFiles);
        string startingBlock = languageData.Blocks[0].BlockName;

        foreach (var blockCommandData in languageData.Blocks)
        {
            m_blocks.Add(blockCommandData.BlockName, blockCommandData.Commands);
        }

        m_labels = languageData.Labels;
        return startingBlock;
    }

    /// <summary>
    /// Sorts in blocks and labels from current script files
    /// </summary>
    private void ParseLanguageData()
    {
        LanguageData languageData = LanguageParser.ParseLanguageData(scriptFiles);

        foreach (var blockCommandData in languageData.Blocks)
        {
            m_blocks.Add(blockCommandData.BlockName, blockCommandData.Commands);
        }

        m_labels = languageData.Labels;
    }

    public void AddBlocks(TextAsset textAsset)
    {
        scriptFiles.Add(textAsset);
    }

    public void Pause()
    {
        EndScript();
        // TODO: Check why this was running -= 1
        Save();
    }

    #endregion

    public void EndScript()
    {
        // Moves forward 1 line, to prevent repeating
        //curState.LineNum++;
        stoppingScript = true;
        m_Dialogue.StopUsing();
    }

    // Executes until a pause / break is triggered
    public override void Execute()
    {
        while (ReadCurLine() && !stoppingScript) { };
        stoppingScript = false;
    }

    public void ResetState() {
        // Resets the interpreter state
        curState = new InterpreterState();
    }

    public object GetValue(string varName)
    {
        if (curState.StringDict.ContainsKey(varName)) return curState.StringDict[varName];
        if (curState.FloatDict.ContainsKey(varName)) return curState.FloatDict[varName];
        return curState.BoolDict.ContainsKey(varName) ? curState.BoolDict[varName] : false;
    }
    
    public void SetValue(string varName, bool value, bool triggerWatcher = false)
    {
        curState.BoolDict[varName] = value;
    }
    public void SetValue(string varName, float value, bool triggerWatcher = false)
    { curState.FloatDict[varName] = value; }
    public void SetValue(string varName, string value, bool triggerWatcher = false)
    { curState.StringDict[varName] = value; }

    #region Bools used for verifying current script context

    private bool InChoice()
    {
        int i = curState.LineNum;
        int curIndent = m_blocks[curState.ActiveBlockName][i].Indent;

        while (m_blocks[curState.ActiveBlockName][i].Indent == curIndent)
            if (--i < 0) return false;

        return m_blocks[curState.ActiveBlockName][i + 1] is ChoiceOptionCommandData;
    }

    private bool InIf()
    {
        int i = curState.LineNum;
        int curIndent = m_blocks[curState.ActiveBlockName][curState.LineNum].Indent;
        while (m_blocks[curState.ActiveBlockName][i].Indent == curIndent)
        {
            i--;
            if (i < 0)
                return false;
        }

        return m_blocks[curState.ActiveBlockName][i] is IfCommandData;
    }

    #endregion

    #region Line by line script reading

    public bool ReadCurLine()
    {
        if (shouldBreak)
        {
            EndScript();
            shouldBreak = false;
            return false;
        }

        // Break early if out of out of lines
        if (curState.LineNum >= m_blocks[curState.ActiveBlockName].Count)
        {
            //Debug.Log("Out of lines");
            EndScript();
            return false;
        }

        LanguageCommandData languageCommandData = m_blocks[curState.ActiveBlockName][curState.LineNum];
        languageCommandData.ReplaceVars(this);

        bool tryForNext = true;

        if (languageCommandData is IfCommandData ifCommandData)
        {
            return HandleIfStatement(ifCommandData);
        }

        if (languageCommandData is JumpCommandData jumpCommandData)
        {
            Jump(jumpCommandData.BlockName);
            return true;
        }

        if (languageCommandData is SayCommandData sayCommandData)
        {
            HandleSayStatement(sayCommandData);
            tryForNext = sayCommandData.Toggle;
        }
        else if (languageCommandData is PrintCommandData printCommandData)
        {
            Debug.Log(printCommandData.Message);
        }
        else if (languageCommandData is RunCommandData runCommandData)
        {
            if (HandleRunStatement(runCommandData) == false)
            {
                curState.LineNum++;
                tryForNext = false;
                //return false;
            }
        }
        else if (languageCommandData is EndCommandData)
        {
            EndScript();
            return false;
        }
        else if (languageCommandData is ChoiceCommandData choiceCommandData)
        {
            HandleChoiceStatement(choiceCommandData);
            curState.LineNum++;
            return false;
        }
        else if (languageCommandData is ChoiceOptionCommandData)
        {
            //ignore
            //while (m_blocks[curState.ActiveBlockName][++curState.LineNum] != )
            curState.LineNum++;
            return true;
        }
        else if (languageCommandData is VarCommandData varCommandData)
        {
            HandleVarStatement(varCommandData);
        }
        else if (languageCommandData is GoToCommandData goToCommandData)
        {
            HandleGoto(goToCommandData);
        }
        else if (languageCommandData is SetBackgroundCommandData setBackgroundCommandData)
        {
            HandleSetBackground(setBackgroundCommandData);
        }
        else if (languageCommandData is PlaySoundCommandData playSoundCommandData)
        {
            HandlePlaySound(playSoundCommandData);
        }
        else
        {
            Debug.LogError("Command type " + typeof(LanguageCommandData) + " not found");
        }

        return NextLine(tryForNext);
    }

    private void HandlePlaySound(PlaySoundCommandData playSoundCommandData)
    {
        var soundName = playSoundCommandData.SoundName;
        var volumeString = playSoundCommandData.Volume;
        m_Dialogue.PlaySound(soundName, !string.IsNullOrEmpty(volumeString) ? float.Parse(volumeString) : 1f);
    }

    private void HandleSetBackground(SetBackgroundCommandData setBackgroundCommandData)
    {
        var spriteName = setBackgroundCommandData.SpriteName;
        var durationString = setBackgroundCommandData.Duration;
        m_Dialogue.SetBackground(spriteName, !string.IsNullOrEmpty(durationString) ? float.Parse(durationString) : 0f);
    }

    private void HandleGoto(GoToCommandData goToCommandData)
    {
        string statement = goToCommandData.LabelName;
        print(statement);
        print(m_labels[curState.ActiveBlockName][statement]);
        curState.LineNum = m_labels[curState.ActiveBlockName][statement];
    }

    private bool NextLine(bool tryForNext)
    {
        if (curState.LineNum + 1 >= m_blocks[curState.ActiveBlockName].Count)
        {
            return false;
        }

        int curIndent = m_blocks[curState.ActiveBlockName][curState.LineNum].Indent;
        int nextIndent = m_blocks[curState.ActiveBlockName][curState.LineNum + 1].Indent;

        if (curIndent == nextIndent)
        {
            curState.LineNum++;
            return tryForNext;
        }

        bool inChoice = InChoice();
        if (inChoice && curIndent < nextIndent)
        {
            int i = curState.LineNum;
            while (curIndent - 1 > m_blocks[curState.ActiveBlockName][i].Indent)
                i--;
            i--;
            return true;
            // What does this do???
            //return curIndent - m_blocks[curState.ActiveBlockName][i].Indent == 2;
        }

        if (InIf() && curIndent > nextIndent)
        {
            curState.LineNum++;
            return tryForNext;
        }

        // 58 -> "Go talk to the Landlord and drop off rent":
        if (curIndent < m_blocks[curState.ActiveBlockName][curState.LineNum + 1].Indent && m_blocks[curState.ActiveBlockName][curState.LineNum + 1].CommandName != "endblock")
            Debug.LogError("Unknown indentation used on line " + (curState.LineNum + 1) + ", " + m_blocks[curState.ActiveBlockName][curState.LineNum + 1].CommandName);

        shouldBreak = true;
        return false;
    }

    // private int PrevLine(int baseLine)
    // {
    //     int tmpLine = baseLine == -1 ? curState.LineNum : baseLine;
    //     while (tmpLine-- > 0 && FirstWord(m_blocks[curState.ActiveBlockName][tmpLine]) != "block")
    //     {
    //         string fWord = FirstWord(m_blocks[curState.ActiveBlockName][tmpLine]);
    //         if (fWord == "block")
    //             return ++tmpLine;
    //
    //         if (m_blocks[curState.ActiveBlockName][tmpLine].IndexOf("run startScene") != -1)
    //         {
    //             return tmpLine + 1;
    //         }
    //
    //         if (fWord == "say" || fWord == "choice")
    //             return tmpLine;
    //     }
    //
    //     return 0;
    // }

    #endregion

    #region Powerhouse functions, handle specific calls from script

    public override void Choose(string choiceText)
    {
        curState.LineNum--;
        int curIndent = m_blocks[curState.ActiveBlockName][curState.LineNum].Indent;
        int tempCurLine = curState.LineNum + 1;

        while (curIndent < m_blocks[curState.ActiveBlockName][tempCurLine].Indent)
        {
            if (m_blocks[curState.ActiveBlockName][tempCurLine] is ChoiceOptionCommandData choiceOptionCommandData &&
                choiceOptionCommandData.Text == choiceText)
            {
                curState.LineNum = ++tempCurLine;
                return;
            }

            tempCurLine++;
        }
        Debug.LogError("Choice \"" + choiceText + "\" not found");
    }

    protected bool HandleRunStatement(RunCommandData runCommandData)
    {
        return m_Dialogue.RunFunction(runCommandData.FunctionName, runCommandData.Arguments.ToArray());
    }

    protected Dictionary<string, Action<object>> varWatcherDelegates = new Dictionary<string, Action<object>>();
    public void RegisterVarWatcher(string varName, Action<object> callback) {
        if (varWatcherDelegates.ContainsKey(varName)) {
            varWatcherDelegates[varName] += callback;
        } else {
            varWatcherDelegates[varName] = callback;
        }
    }
    
private void HandleVarStatement(VarCommandData varCommandData)
    {
        string varName = varCommandData.Variable;
        string sign = varCommandData.Sign;
        string value = varCommandData.Value;
        value = LanguageParser.TrimQuotes(value);

        float parsedVal;
        switch (sign)
        {
            case "=":
            case "equals":
            case "==":
                // Boolean assignment
                if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    bool varVal = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    curState.BoolDict[varName] = varVal;
                    if (varWatcherDelegates.ContainsKey(varName))
                    {
                        varWatcherDelegates[varName].Invoke(varVal);
                    }
                }
                else
                {
                    // Float assignment
                    if (float.TryParse(value, out parsedVal))
                    {
                        curState.FloatDict[varName] = parsedVal;
                        if (varWatcherDelegates.ContainsKey(varName))
                        {
                            varWatcherDelegates[varName].Invoke(parsedVal);
                        }
                    }
                    else
                    {
                        // String assignment
                        curState.StringDict[varName] = value;
                        if (varWatcherDelegates.ContainsKey(varName))
                        {
                            varWatcherDelegates[varName].Invoke(value);
                        }
                    }
                }

                break;
            case "+=":
                if (float.TryParse(value, out parsedVal))
                    curState.FloatDict[varName] += parsedVal;
                else
                    curState.StringDict[varName] = value;
                break;
            case "-=":
                if (float.TryParse(value, out parsedVal))
                    curState.FloatDict[varName] -= parsedVal;
                else
                    Debug.LogError("Cannot apply -= to bool or string");
                break;
            default:
                break;
        }
    }
    private void HandleChoiceStatement(ChoiceCommandData choiceCommandData)
    {
        int choiceIndent = choiceCommandData.Indent;
        List<string> choices = new List<string>();

        // Separates prompt from the choice line, if there is one
        string choicePrompt = choiceCommandData.Prompt;

        // Parses out the individual choice lines
        int lineNum = curState.LineNum + 1;
        while (m_blocks[curState.ActiveBlockName][lineNum].Indent > choiceIndent)
        {
            if (m_blocks[curState.ActiveBlockName][lineNum] is ChoiceOptionCommandData choiceOptionCommandData &&
                choiceOptionCommandData.Indent == choiceIndent + 1)
            {
                choices.Add(choiceOptionCommandData.Text);
            }

            lineNum++;
        }

        m_Dialogue.HandleChoice(choicePrompt, choices);
    }

    private void HandleSayStatement(SayCommandData sayCommandData)
    {
        m_Dialogue.HandleSay(sayCommandData.SpeakerName, sayCommandData.Statement);
    }

    private bool IsBool(string value)
    {
        return value == "True" || value == "False";
    }

    public bool HandleIfStatement(IfCommandData ifCommandData, bool skipFastForward = false) //[X]
    {
        bool result = false;

        string variable = ifCommandData.Variable;
        string sign = ifCommandData.Sign;
        string value = ifCommandData.Value;
        // A variable to interpret as a bool
        if (string.IsNullOrEmpty(sign) && string.IsNullOrEmpty(value))
        {
            if (variable.ToLower() == "true")
            {
                return true;
            }
            if (variable.ToLower() == "false")
            {
                return false;
            }
            result = variable[0] == '!'
                ? !(bool)GetValue(variable.Substring(1)) : (bool)GetValue(variable);
        }
        // Just a bool, sitting there
        else if (string.IsNullOrEmpty(sign) && string.IsNullOrEmpty(variable))
        {
            if (IsBool(value))
            {
                return bool.Parse(value);
            }
        }
        else //Equality / greater than / less than
        {
            float floatVal;
            bool isFloat = float.TryParse(value, out floatVal);
            if (sign != "==" && sign != "=" && value != "is" && value != "equals" && !isFloat)
                Debug.LogError("Comparison value for non-equality must be a number");
            value = LanguageParser.TrimQuotes(value).TrimEnd(':');

            if (sign == "==" && IsBool(ifCommandData.Variable) && IsBool(ifCommandData.Variable))
            {
                result = bool.Parse(ifCommandData.Variable) == bool.Parse(ifCommandData.Value);
            }
            else switch (sign)
            {
                case "<=":
                    result = (float)GetValue(variable) <= floatVal;
                    break;
                case "<":
                    result = (float)GetValue(variable) < floatVal;
                    break;
                case ">=":
                    result = (float)GetValue(variable) >= floatVal;
                    break;
                case ">":
                    result = (float)GetValue(variable) > floatVal;
                    break;
                case "==":
                case "=":
                case "is":
                case "equals":
                    if (!isFloat && (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("false", StringComparison.OrdinalIgnoreCase)))
                        result = ((bool)GetValue(variable) == (value.Equals("true", StringComparison.OrdinalIgnoreCase)));
                    else if (isFloat)
                        result = (float)GetValue(variable) == floatVal;
                    else {
                        object v = GetValue(variable);
                        string vs = (string)v;
                        result = vs == value;
                    }
                    break;
            }

        }

        // Generally we move forward the execution pointer, but if we're  dry running an if we don't want that
        if (skipFastForward) { return result; }

        if (result)
            curState.LineNum++;
        else
        {
            int originalIndentLevel = ifCommandData.Indent;
            while (m_blocks[curState.ActiveBlockName][++curState.LineNum].Indent > originalIndentLevel) { }
            if (m_blocks[curState.ActiveBlockName][curState.LineNum].CommandName == "endblock" || m_blocks[curState.ActiveBlockName][curState.LineNum].Indent == 0)
                return false;
        }

        return true;
    }

    /***********************************************************
     * "'GoTo considered harmful', so I named it Jump instead" *
     *     -Ren'Py Source Code                                 *
     ***********************************************************/
    public void Jump(string blockName)
    {
        // TODO: Check that the blockName exists
        if (m_blocks.ContainsKey(blockName)) {
            curState.LineNum = 0;
            curState.ActiveBlockName = blockName;
        } else {
            Debug.LogError("Block " + blockName + " not found");
            EndScript();
        }
    }

    #endregion

    #region Parsing functions

    public string ReplaceVars(string lineText)
    {
        if (string.IsNullOrEmpty(lineText))
            return lineText;
        
        while (lineText.IndexOf("{") != -1 && lineText.IndexOf("}") != -1)
        {
            string formerText = lineText.Substring(0, lineText.IndexOf("{"));
            lineText = lineText.Substring(lineText.IndexOf("{") + 1);
            string varName = lineText.Substring(0, lineText.IndexOf("}"));
            if (GetValue(varName) is bool && !curState.BoolDict.ContainsKey(varName))
                Debug.Log("Var " + varName + " does not exist");
            lineText = lineText.Substring(lineText.IndexOf("}") + 1);
            lineText = formerText + GetValue(varName) + lineText;
        }
        return lineText;
    }

    #endregion

    #region Save state handling

    private void OnApplicationQuit()
    {
        if (saveOnQuit)
            Save();
    }

    public override void Save(string saveName = "")
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/" + SAVE_PREFIX + saveName + ".dat", FileMode.OpenOrCreate);

        //curState.LineNum--;
        bf.Serialize(file, curState);
        file.Close();
    }

    public static InterpreterState LoadState(string saveName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + SAVE_PREFIX + saveName + ".dat"))
        {
            InterpreterState curState;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + SAVE_PREFIX + saveName + ".dat", FileMode.Open);
            
            if (file.Length > 0) // Checks for empty save files
                curState = bf.Deserialize(file) as InterpreterState;
            else
            {
                curState = new InterpreterState(0, 
                    new SerializableDictionary<string, bool>(),
                    new SerializableDictionary<string, string>(), 
                    new SerializableDictionary<string, float>());
            }
            file.Close();

            try // Used to check if file exists, but no data for current scene exists
            {
                if (curState == null) { }
            }
            catch (KeyNotFoundException)
            {
                return new InterpreterState(0, curState.BoolDict, curState.StringDict, curState.FloatDict);
            }

            return curState;
        }
        else
        {
            throw new Exception($"Attempted to load a save that wasn't there: {saveName}");
        }
    }


    public override void Load(string saveName = "")
    {
        curState = LoadState(saveName);
        if (curState == null)
        {
            Debug.Log("No save file exists");
            curState = new InterpreterState();
        }
    }

    public void SetState(InterpreterState interpreterState)
    {
        curState = interpreterState;
        ParseScripts();
    }

    #endregion

}
