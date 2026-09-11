using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;

[RequireComponent(typeof(Interpreter))]
/*[RequireComponent(typeof(Button))]*/
public abstract class Dialogue : MonoBehaviour
{
    public bool autoStart = true;

    [Space]
    [HideInInspector] public int playerSpeaker;
    public List<AudioClip> sounds;

    private LanguageInterpreter _curInterpreter;

    protected LanguageInterpreter CurInterpreter
    {
        get
        {
            if (!_curInterpreter)
                _curInterpreter = GetComponent<LanguageInterpreter>();
            return _curInterpreter;
        }
    }

    // if it's currently waiting for player choice, optionCount will be > 0
    public int optionCount { get; private set; }
    public List<Speaker> Speakers { get => speakers; set
        {
            speakers = value; 
        }
    }

    private List<string> options;
    public AudioSource audioSource;
    protected MaskableGraphic[] m_VisibleElements;

    public List<Speaker> speakers = new List<Speaker>();

    public string GetOption(int index)
    {
        return options[index];
    }
    
    public delegate void DialogueDelegate();
    public event DialogueDelegate OnSayOrChoice;

    protected virtual void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start ()
    {
        if (!Application.isPlaying)
            return;

        CurInterpreter.Initialize();

        if (autoStart)
        {
            ToggleVisibleElements(true);
            StartUsing();
        }
    }

    protected Speaker GetSpeakerByName(string speakerName) {
        //return speakers.(speakerName);
        foreach (Speaker speaker in Speakers) {
            if (speaker.speakerName.ToLower() == speakerName.ToLower()) {
                return speaker;
            }
        }
        if (speakerName != "None")
        {
            Debug.LogWarning("Could not find speaker with name " + speakerName);
        }
        return null;
    } 

    protected virtual void OnLoad() { }

    public bool RunFunction(string functionName, string[] arguments)
    {
        List<object> argList = new List<object>();

        List<Type> argTypes = new List<Type>();

        foreach(string arg in arguments)
        {
            object val;
            argTypes.Add(GetConverted(arg, out val));
            argList.Add(val);
        }

        // foreach (MethodInfo info in GetType().GetMethods())
        // {
        //     print(info.ToString());
        // }
        
        MethodInfo method = GetType().GetMethod(functionName, argTypes.ToArray());
        if (method != null)
        {
            object ret = GetType().InvokeMember(functionName, BindingFlags.InvokeMethod, null, this, argList.ToArray());
            if (ret is bool && !(bool)ret)
                return false;
        }
        else
        {
            string sArgsTypes = "";
            foreach (Type type in argTypes)
                sArgsTypes += " " + type.ToString();
            Debug.LogWarning("Function " + functionName + " with argument types\n" + sArgsTypes + " not found");
        }

        return true;
    }

    private Type GetConverted(string arg, out object value)
    {
        float tmp;
        if (float.TryParse(arg, out tmp))
        {
            value = tmp;
            return typeof(float);
        }

        if (arg == "true" || arg == "false")
        {
            value = arg == "true";
            return typeof(bool);
        }

        value = arg;
        return typeof(string);
    }

    public virtual void StartUsing()
    {
        CurInterpreter.Execute();
    }

    public void HandleSay(string speakerName, string[] args)
    {
        optionCount = 0;
        options = null;
        Speaker speaker = null;
        if (speakerName != "") {
            speaker = GetSpeakerByName(speakerName);
        }
        DoSay(speaker, args);
        OnSayOrChoice?.Invoke();
    }

    protected abstract void DoSay(Speaker speakerName, string[] args);

    internal void HandleChoice(string choicePrompt, List<string> choiceTexts)
    {
        optionCount = choiceTexts.Count();
        options = choiceTexts;
        DoChoice(choicePrompt, choiceTexts);
        OnSayOrChoice?.Invoke();
    }

    protected abstract void DoChoice(string choicePrompt, List<string> choiceTexts);

    internal virtual void HighlightChoice(int optionIndex) {}

    public virtual void StopUsing(){}
    
    public virtual void SaveToFile(string fileName = "")
    {
        CurInterpreter.Save(fileName);
    }

    // process player input
    // null received -> not choice needed, just continue
    // string received -> process choice 
    public virtual void HandleInput(string input = null)
    {
        if (optionCount > 0 && input == null)
            return;
        input = string.IsNullOrWhiteSpace(input) ? null : input;
        
        if (input == null)
            CurInterpreter.Execute();
        else
        {
            CurInterpreter.Choose(input);
            CurInterpreter.Execute();
        }
    }

    public virtual void OnPress() 
    {
        CurInterpreter.Execute();
    }

    // Override if some child elements of DialogueManager should always be shown
    public virtual void ToggleVisibleElements(bool? b = null)
    {
        if (m_VisibleElements == null || m_VisibleElements.Length == 0)
            InitVisibleElements();

        // Debug.Log(m_VisibleElements.Length);

        if (b == true || b == false)
            foreach (MaskableGraphic r in m_VisibleElements)
                r.enabled = (bool)b;
        else
            foreach (MaskableGraphic r in m_VisibleElements)
                r.enabled = !r.enabled;

    }

    private void InitVisibleElements() {
        m_VisibleElements = GetComponentsInChildren<MaskableGraphic>();
    }

    public virtual void SetBackground(string input, float v) {}

    public virtual void PlaySound(string soundName, float volume = 1f) {
        foreach (var sound in sounds) {
            if (sound.name == soundName) {
                audioSource.clip = sound;
                audioSource.PlayOneShot(sound, volume);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Dialogue), true)]
public class DialogueInspector: Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Dialogue dialogue = (Dialogue)target;
        List<string> speakerNames = new List<string>();
        foreach (Speaker speaker in dialogue.Speakers) {
            if (speaker == null) {
                continue;
            }
            speakerNames.Add(speaker.speakerName);
        }
        dialogue.playerSpeaker = EditorGUILayout.Popup(
            "Player Speaker", dialogue.playerSpeaker, speakerNames.ToArray()
        );
    }
}
#endif