using System;
using UnityEngine;

[Serializable]
public class InterpreterState {
    [SerializeField]
    public int LineNum = 0;
    [SerializeField]
    public string ActiveBlockName = "";
    [SerializeField]
    public SerializableDictionary<string, bool> BoolDict = new SerializableDictionary<string, bool>();
    [SerializeField]
    public SerializableDictionary<string, string> StringDict = new SerializableDictionary<string, string>();
    [SerializeField]
    public SerializableDictionary<string, float> FloatDict = new SerializableDictionary<string, float>();

    public InterpreterState() { }
    public InterpreterState(string startingBlock) { ActiveBlockName = startingBlock; }
    public InterpreterState(int lineNum,
        SerializableDictionary<string, bool> boolDict,
        SerializableDictionary<string, string> stringDict,
        SerializableDictionary<string, float> floatDict) {
        LineNum = lineNum;
        BoolDict = boolDict;
        StringDict = stringDict;
        FloatDict = floatDict;
    }
}