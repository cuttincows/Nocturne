using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishingGameDialogue : BasicDialogue
{
    public AudioSource blipSource;
    public PlayerCharacterController playerController;

    public bool hasBeenUsedBefore;

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

    public void OnEnable()
    {
        if (hasBeenUsedBefore)
        {
            OnPress();
        }
        hasBeenUsedBefore = true;
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

    float lastChatterIndex = 0;
    public void Update()
    {
        if (triggerWereWaitingFor == string.Empty)
        {
            return;
        }

        float chatterSpeed = 0.1f;
        int chatterIndex = Mathf.FloorToInt(Time.realtimeSinceStartup / chatterSpeed);

        if (TextTyper.IsTyping(layout.GetDialogueBox()) && chatterIndex != lastChatterIndex)
        {
            blipSource.time = UnityEngine.Random.Range(0, currentSpeaker.chatter.length);
            lastChatterIndex = chatterIndex;
            blipSource.Play();
        }
    }

    public void SetNextScene(string nextScene)
    {
    //    PlushieGameDialogue.NextScene = nextScene;
    }

    Speaker currentSpeaker = null;
    protected override void DoSay(Speaker speaker, string[] args)
    {
        // The statement being said is always the last args element
        string statement = args[args.Length - 1];

        currentSpeaker = speaker;

        if (args.Length >= 2)
        {  // 2 args is speakerEmotion, statement
            layout.SetSpeakerEmotion(speaker, args[0]);
        }
        else
        {
            layout.SetSpeakerEmotion(speaker, "default");
        }

        blipSource.Stop();
        if (speaker != null && speaker.chatter != null)
        {
            blipSource.clip = speaker.chatter;
            blipSource.time = UnityEngine.Random.Range(0, speaker.chatter.length);
            blipSource.Play();
        }

        layout.GetDialogueBox().TypeText(statement, TextTyper.DEFAULT_TYPE, TextTyper.DEFAULT_SPEED, () =>
        {
            typing = false;
        });
        currentTypingField = layout.GetDialogueBox();
        layout.GetChoicesContainer().gameObject.SetActive(false);
    }

    List<string> currentChoices;
    protected override void DoChoice(string choicePrompt, List<string> choiceTexts)
    {
        //base.DoChoice(choicePrompt, choiceTexts);
        currentChoices = new List<string>();
        RectTransform choiceContainer = layout.GetChoicesContainer();

        //while (choiceContainer.childCount > 0)
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        //for (int i = 0; i < choiceTexts.Count; i++) 
        foreach (string choiceText in choiceTexts) 
        {
            GameObject choice = Instantiate(layout.GetChoiceButton(0, choiceTexts.Count), parent:choiceContainer);
            (choice.transform as RectTransform).sizeDelta = new Vector2(1200, 100);
                choice.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;//choiceTexts[i];
            choice.GetComponent<Button>().onClick.AddListener(() => {
                HandleInput(choiceText);
            });
        }
        layout.GetChoicesContainer().gameObject.SetActive(true);
    }
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    [Header("Friendbot screen")]
    public TextMeshPro friendBotText;
    public void SetBotScreen(string screenText)
    {
        friendBotText.text = screenText;
    }

    public override void StopUsing()
    {
        base.StopUsing();
        InteractWithObject.InteractionLocked = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        InGameMenuManager.ControllingCursor = true;
        playerController.enabled = true;
    }
}
