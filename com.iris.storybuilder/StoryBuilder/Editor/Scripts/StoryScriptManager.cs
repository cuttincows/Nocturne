using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

//public enum InitialBackgroundOption { ChooseABackground, SolidColor, UseFirstBackgroundInScript }
public enum InitialBackgroundOption { ChooseABackground, UseFirstBackgroundInScript }
public enum LayoutOption { CharacterDialogue, Scrolling }

public class StoryScriptManager : EditorWindow
{
    public GameObject scrollingPrefab;
    public string scrollingDescription = "";
    public GameObject dialoguePrefab;
    public string characterDialogueDescription = "";
    TextAsset currentScript = null;
    Sprite background = null;
    InitialBackgroundOption backgroundOption = InitialBackgroundOption.UseFirstBackgroundInScript;
    LayoutOption layoutChoice = LayoutOption.CharacterDialogue;
    List<Speaker> speakersToAdd = new List<Speaker>();
    Speaker primarySpeaker = null;
    int speakerHolderIndex;
    StoryBuilderAssets assets = null;
    private void OnEnable()
    {


        //Prefab Fallback
        if (scrollingPrefab == null || dialoguePrefab == null)
        {
            string dir = "Packages/com.unity.story-graph/StoryBuilder/Runtime/UI/Prefabs/Layouts/";
            string scrollingPath = dir + "ScrollLayout.prefab";
            string dialoguePath = dir+ "DefaultLayout.prefab";
            scrollingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(scrollingPath);
            dialoguePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(dialoguePath);
        }

        speakerHolderIndex = -1;
        // Reference to the root of the window.
        VisualElement root = rootVisualElement;
        AddScriptField(root);
        AddLayoutField(root);
        AddPrimarySpeakerField(root);
        AddBackgroundFields(root);
        AddCreateButton(root);
    }

    #region Fields
    void AddScriptField(VisualElement root)
    {
        var storyScript = new ObjectField("Story Script")
        {
            objectType = typeof(TextAsset)
        };
        if (currentScript != null)
        {
            storyScript.value = currentScript;
        }
        storyScript.RegisterValueChangedCallback<UnityEngine.Object>(script => {
            currentScript = (TextAsset)script.newValue;
            assets = StoryBuilderAssetLister.GetAssetList((TextAsset)script.newValue);
            AddPrimarySpeakerField(root);
        });
        root.Add(storyScript);
    }

    void AddLayoutField(VisualElement root)
    {
        var layouts = Enum.GetValues(typeof(LayoutOption));
        List<string> layoutNames = new List<string>();
        foreach(var layout in layouts)
        {
            layoutNames.Add(layout.ToString());
        }
        var layoutField = new PopupField<string>("Dialogue Layout", layoutNames, 0);
        layoutField.RegisterValueChangedCallback<string>(layout => {
            layoutChoice = (LayoutOption) Enum.Parse(typeof(LayoutOption), layout.newValue);
        });
        root.Add(layoutField);
    }
    #region speaker

    void AddPrimarySpeakerField(VisualElement root)
    {
        if(currentScript == null)
        {
            AddPlaceholderPrimarySpeaker(root);
        }
        else
        {
            ShowPrimarySpeakerField(root);
        }
    }
    void AddPlaceholderPrimarySpeaker(VisualElement root)
    {
        if(speakerHolderIndex > 0)
        {
            var speakerHolder = new Label("Primary Speaker");
            root.Insert(speakerHolderIndex, speakerHolder);
            root.RemoveAt(speakerHolderIndex + 1);
        }
        else
        {
            var speakerHolder = new Label("Primary Speaker");
            root.Add(speakerHolder);
            speakerHolderIndex = root.IndexOf(speakerHolder);
        }
    }

    void CheckAssets()
    {

    }

    void ShowPrimarySpeakerField(VisualElement root)
    {
        speakersToAdd = GetAssetFromScript<Speaker>(assets.Speakers);
        //StoryBuilderAssets = StoryBuilderAssetLister.GetAssetList()
        //speakersToAdd = GetAssetFromScript<Speaker>(GetArgumentFromInstruction("say"));
        if(speakersToAdd.Count == 0)
        {
            AddPlaceholderPrimarySpeaker(root);
            return;
        }

        List<string> speakerNames = new List<string>(speakersToAdd.Count);

        foreach(Speaker speaker in speakersToAdd)
        {
            speakerNames.Add(speaker.name);
        }
        var speakerHolder = new PopupField<string>("Primary Speaker", speakerNames, 0);
        speakerHolder.RegisterValueChangedCallback<string>(speaker => {
            if(speakersToAdd.Count > 0)
            {
                primarySpeaker = speakersToAdd[speakerNames.IndexOf(speaker.newValue)];
                Debug.Log("New Speaker: " + speakersToAdd[speakerNames.IndexOf(speaker.newValue)]);
            }
        });

        if(speakersToAdd.Count > 0)
        {
            primarySpeaker = speakersToAdd[0];
        }

        if (speakerHolderIndex > 0)
        {
            root.Insert(speakerHolderIndex, speakerHolder);
            root.RemoveAt(speakerHolderIndex + 1);
        }
        else
        {
            root.Add(speakerHolder);
            speakerHolderIndex = root.IndexOf(speakerHolder);
        }
    }
    #endregion

    #region background

    void AddBackgroundFields(VisualElement root)
    {
        var backgroundOptionLabel = new Label("Set Initial Background");

        var startBackgroundOption = new EnumField(InitialBackgroundOption.UseFirstBackgroundInScript);

        var startingBackground = new ObjectField("Starting Background")
        {
            objectType = typeof(Sprite),
            visible = false
        };
        startingBackground.RegisterValueChangedCallback(choice =>
        {
            background = (Sprite)choice.newValue;
        });
        var backgroundColor = new ColorField("Solid Color")
        {
            visible = false
        };

        startBackgroundOption.RegisterValueChangedCallback(choice =>
        {
            ToggleBackgroundOptions((InitialBackgroundOption)choice.newValue, startingBackground, backgroundColor);
        });

        root.Add(backgroundOptionLabel);
        root.Add(startBackgroundOption);
        root.Add(startingBackground);
        root.Add(backgroundColor);
    }

    void ToggleBackgroundOptions(InitialBackgroundOption selection, VisualElement backgroundSelector, VisualElement colorSelector)
    {
        backgroundOption = selection;
        switch (selection)
        {
            case InitialBackgroundOption.ChooseABackground:
                backgroundSelector.visible = true;
                colorSelector.visible = false;
                break;
            //case InitialBackgroundOption.SolidColor:
            //    backgroundSelector.visible = false;
            //    colorSelector.visible = true;
            //    break;
            case InitialBackgroundOption.UseFirstBackgroundInScript:
                backgroundSelector.visible = false;
                colorSelector.visible = false;
                break;
            default:
                break;
        }
    }

    void AddBackgroundColorField()
    {

    }
    #endregion

    void AddCreateButton(VisualElement root)
    {
        var createDialogueButton = new Button
        {
            text = "Create Dialogue"
        };
        createDialogueButton.style.width = 160;
        createDialogueButton.style.height = 30;
        createDialogueButton.clickable.clicked += CreateNewDialogue;

        root.Add(createDialogueButton);
    }

    #endregion
    [MenuItem("Story Graph/Create StoryBuilder Dialogue")]
    public static void ShowWindow()
    {
        // Opens the window, otherwise focuses it if it's already open.
        EditorWindow window = GetWindow<StoryScriptManager>();

        // Adds a title to the window.
        window.titleContent = new GUIContent("New StoryBuilder Dialogue");

        // Sets a minimum size to the window.
        window.minSize = new Vector2(250, 50);
    }

    void CreateNewDialogue()
    {
        if (currentScript == null)
        {
            Debug.LogError("No StoryScript selected");
            return;
        }

        switch (layoutChoice)
        {
            case LayoutOption.Scrolling:
                CreateScrollingDialogue();
                break;
            case LayoutOption.CharacterDialogue:
            default:
                CreateCharacterDialogue();
                break;
        }
        if (!FindObjectOfType<EventSystem>())
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    void CreateScrollingDialogue()
    {
        GameObject newDialogue = new GameObject(currentScript.name);
        GameObject layoutGameObject = Instantiate(scrollingPrefab, newDialogue.transform);
        BasicScrollDialogueLayout dialogueLayout = layoutGameObject.GetComponent<BasicScrollDialogueLayout>();
        if (dialogueLayout == null)
        {
            Debug.LogError("Layout missing BaseDialogueLayout");
            //Destroy(newDialogue);
            return;
        }
        LanguageInterpreter l = newDialogue.AddComponent<LanguageInterpreter>();
        l.scriptFiles = new List<TextAsset>
        {
            currentScript
        };

        newDialogue.AddComponent<AudioSource>();
        BasicScrollDialogue dialogue = newDialogue.AddComponent<BasicScrollDialogue>();
        newDialogue.AddComponent<SimpleDialogueInput>();
        if (speakersToAdd.Count == 0)
        {
            speakersToAdd = GetAssetFromScript<Speaker>(assets.Speakers);
            //speakersToAdd = GetAssetFromScript<Speaker>(GetArgumentFromInstruction("say"));
            //AD
        }
        dialogue.SetSpeakers(speakersToAdd);

        dialogue.SetSounds(GetAssetFromScript<AudioClip>(assets.SoundClips));

        List<Sprite> backgroundsInScript = GetAssetFromScript<Sprite>(assets.BackgroundSprites);
        dialogue.SetBackgrounds(backgroundsInScript);

        dialogue.SetLayout(dialogueLayout);

        switch (backgroundOption)
        {
            case InitialBackgroundOption.ChooseABackground:
                dialogueLayout.GetBackgroundImage().sprite = background;
                break;
            case InitialBackgroundOption.UseFirstBackgroundInScript:
            default:
                if (backgroundsInScript.Count > 0)
                {
                    dialogueLayout.GetBackgroundImage().sprite = backgroundsInScript[0];
                }
                break;
        }
        if (primarySpeaker != null)
        {
            dialogueLayout.SetPrimarySpeaker(primarySpeaker, SpeakerSide.Left);
        }
        dialogue.AssignAssetsToDialogue();
        Debug.Log("Scrolling Dialogue Created");
    }

    void CreateCharacterDialogue()
    {
        GameObject newDialogue = new GameObject(currentScript.name);
        GameObject layoutGameObject = Instantiate(dialoguePrefab, newDialogue.transform);
        BaseDialogueLayout dialogueLayout = layoutGameObject.GetComponent<BaseDialogueLayout>();
        if (dialogueLayout == null)
        {
            Debug.LogError("Layout missing BaseDialogueLayout");
            //Destroy(newDialogue);
            return;
        }
        LanguageInterpreter l = newDialogue.AddComponent<LanguageInterpreter>();
        l.scriptFiles = new List<TextAsset>
        {
            currentScript
        };

        newDialogue.AddComponent<AudioSource>();
        BasicDialogue dialogue = newDialogue.AddComponent<BasicDialogue>();
        newDialogue.AddComponent<SimpleDialogueInput>();
        if (speakersToAdd.Count == 0)
        {
            speakersToAdd = GetAssetFromScript<Speaker>(assets.Speakers);
            //speakersToAdd = GetAssetFromScript<Speaker>(GetArgumentFromInstruction("say"));
            //ADD
        }
        dialogue.SetSpeakers(speakersToAdd);

        dialogue.SetSounds(GetAssetFromScript<AudioClip>(assets.SoundClips));

        List<Sprite> backgroundsInScript = GetAssetFromScript<Sprite>(assets.BackgroundSprites);
        dialogue.SetBackgrounds(backgroundsInScript);

        dialogue.SetLayout(dialogueLayout);

        switch (backgroundOption)
        {
            case InitialBackgroundOption.ChooseABackground:
                dialogueLayout.GetBackgroundImage().sprite = background;
                break;
            case InitialBackgroundOption.UseFirstBackgroundInScript:
            default:
                if (backgroundsInScript.Count > 0)
                {
                    dialogueLayout.GetBackgroundImage().sprite = backgroundsInScript[0];
                }
                break;
        }
        if (primarySpeaker != null)
        {
            dialogueLayout.SetPrimarySpeaker(primarySpeaker, SpeakerSide.Left);
        }
        dialogue.AssignAssetsToDialogue();
        Debug.Log("Character Dialogue Created");
    }

    //public List<Speaker> GetSpeakersFromScript(List<string> speakers)
    //{
    //    Dictionary<string, Speaker> allSpeakers = EditorUtil.GetAllInstances<Speaker>();
    //    List<string> missingSpeakers = new List<string>(speakers);
    //    List<Speaker> dialogueSpeakers = new List<Speaker>();
    //    foreach (string scriptSpeaker in speakers)
    //    {
    //        if (allSpeakers.ContainsKey(scriptSpeaker))
    //        {
    //            dialogueSpeakers.Add(allSpeakers[scriptSpeaker]);
    //            missingSpeakers.Remove(scriptSpeaker);
    //        }
    //        if (missingSpeakers.Contains(scriptSpeaker))
    //        {
    //            Debug.LogError("Speaker ScriptableObject missing: " + scriptSpeaker);
    //        }
    //    }
    //    return dialogueSpeakers;
    //}

    //List<Sprite> GetBackgroundsFromScript(List<string> backgrounds)
    //{
    //    List<Sprite> results = new List<Sprite>();
    //    List<string> missingBackgrounds = new List<string>(backgrounds);
    //    Dictionary<string, Sprite> allSprites = EditorUtil.GetAllInstances<Sprite>();
    //    foreach(string background in backgrounds)
    //    {
    //        if (allSprites.ContainsKey(background))
    //        {
    //            results.Add(allSprites[background]);
    //            missingBackgrounds.Remove(background);
    //        }
    //        if (missingBackgrounds.Contains(background))
    //        {
    //            Debug.LogError("Background image referenced in script not found: " + background);
    //        }
    //    }
    //    return results;
    //}

    static List<T> GetAssetFromScript<T>(List<string> assets) where T : UnityEngine.Object
    {
        List<T> results = new List<T>();
        List<string> missingAssets = new List<string>(assets);
        Dictionary<string, T> allAssets = EditorUtil.GetAllInstances<T>();
        foreach (string asset in assets)
        {
            if (allAssets.ContainsKey(asset))
            {
                results.Add(allAssets[asset]);
            }
            else
            {
                Debug.LogError(typeof(T) + " referenced in script not found: " + asset);
            }
        }
        return results;
    }

    //List<AudioClip> GetSoundsFromScript(List<string> sounds)
    //{
    //    List<AudioClip> results = new List<AudioClip>();
    //    List<string> missingSounds = new List<string>(sounds);
    //    Dictionary<string, AudioClip> allSoundClips = EditorUtil.GetAllInstances<AudioClip>();
    //    foreach (string sound in sounds)
    //    {
    //        if (allSoundClips.ContainsKey(sound))
    //        {
    //            results.Add(allSoundClips[sound]);
    //            missingSounds.Remove(sound);
    //        }
    //        if (missingSounds.Contains(sound))
    //        {
    //            Debug.LogError("AudioClip referenced in script not found: " + sound);
    //        }
    //    }
    //    return results;
    //}

    //List<string> GetArgumentFromInstruction(string instruction)
    //{
    //    List<string> results = new List<string>();
    //    List<int> sets = currentScript.text.AllIndexesOf(instruction);
    //    foreach (int i in sets)
    //    {

    //        if (currentScript.text.Substring(i + instruction.Length + 1, 1).Equals("\"")) continue;
    //        if (!currentScript.text.Substring(i - 1, 1).Equals("\t")) continue;
    //        string instance = currentScript.text.Substring(i + instruction.Length + 1, currentScript.text.IndexOf(" ", i + instruction.Length + 1) - (i + instruction.Length));
    //        if (!results.Contains(instance.Trim()))
    //        {
    //            results.Add(instance.Trim());
    //        }
    //    }
    //    return results;
    //}
}
