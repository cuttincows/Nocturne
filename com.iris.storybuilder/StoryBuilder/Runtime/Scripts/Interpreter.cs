using UnityEngine;

public abstract class Interpreter : MonoBehaviour
{
    [HideInInspector]  public const string SAVE_PREFIX = "dialogueSave";

    // Init function, separate from Awake and Start; called after Dialogue's Start
    public abstract void Initialize();

    // Call to run the Interpreter's main functionality
    public abstract void Execute();

    // Called when an input button is pressed by the player
    public abstract void Choose(string choiceText);

    // ----------------------------------------------------------------------------------

    // Utilize serializable objects to be able to maintain states properly
    public abstract object SaveData { get; }

    // Override for specific saving and loading functionality
    public abstract void Save(string saveName = "");
    public abstract void Load(string saveName = "");
}
