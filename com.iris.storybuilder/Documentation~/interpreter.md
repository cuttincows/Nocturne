# Interpreter

The Interpreter processes dialogue files for the DialogueRenderer. It also manages the states of the dialogue files, Speakers, and variables.

StoryBuilder includes LanguageInterpreter, which handles a custom scripting language called StoryScript. However, you can create your own scripting language as long as you write an Interpreter for it. Then attach both your Interpreter and its DialogueRenderer to a GameObject.

To implement your own Interpreter, you’ll need to implement the following abstract methods.

|**Method name** |**Description** |
|:---|:---|
|__Initialize__| After you set up the DialogueRenderer, it calls this method. This is similar to the [Start](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html) and [Awake](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html) functions in Unity, but Initialize lets you assume that all the DialogueRenderer systems are configured by the time it's called. |
|__Execute__ | Call this method to run the Interpreter until the next stopping point. A common stopping point is after a command like **say** or **choice** because the system requires user input to continue. |
|__Choose__ | The DialogueRenderer calls this method when the user makes a choice. It passes the choice's text value to this method, which lets you determine how to manage forking paths. |
|__SaveData__ | Stores data that the system requires to recreate the state at the point when you save the game. This could include data like character positions, or the most recently set background. |
|__Save__ | Manages how SaveData is stored in the system. |
|__Load__ | Manages loading a saved state from the system, and resuming the DialogueRenderer from that state. |
