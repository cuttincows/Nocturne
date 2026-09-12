using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingGameDialogue : BasicDialogue
{
    public AudioSource blipSource;

    protected override void Start()
    {
        base.Start();
        // We use the same NextScene val as PlushieGameDialogue
        // if (PlushieGameDialogue.NextScene != string.Empty)
        // {
        //     CurInterpreter.Jump(PlushieGameDialogue.NextScene);
        //     OnPress();
        // }
        TextTyper.blipSource = blipSource;
    }
    public override void OnPress()
    {
        if (typing)
        {
            TextTyper.CompleteAll();
        }
        else
        {
            base.OnPress();  // Base calls Execute
        }
    }

    public bool WaitForTrigger(string triggerName)
    {
        print(triggerName);
        //CurInterpreter.o
        ToggleVisibleElements();
        return false;
    }

    // not static so it resets each new minigame, set this from the instance 
    private string triggerWereWaitingFor = string.Empty;

    public void Update()
    {
        if (triggerWereWaitingFor == string.Empty)
        {
            return;
        }


    }

    public void SetNextScene(string nextScene)
    {
    //    PlushieGameDialogue.NextScene = nextScene;
    }

    protected override void DoSay(Speaker speaker, string[] args)
    {
        // The statement being said is always the last args element
        string statement = args[args.Length - 1];

        if (args.Length >= 2)
        {  // 2 args is speakerEmotion, statement
            layout.SetSpeakerEmotion(speaker, args[0]);
        }
        else
        {
            layout.SetSpeakerEmotion(speaker, "default");
        }


        layout.GetDialogueBox().TypeText(statement, TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
        {
            typing = false;
        });
        currentTypingField = layout.GetDialogueBox();
        layout.GetChoicesContainer().gameObject.SetActive(false);
    }
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
