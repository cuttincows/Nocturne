using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

public enum SpeakerList { Speaker1, Speaker2, Speaker3, Speaker4 }

public class StoryBuilderPrefabUtility : EditorWindow
{
    public Texture HeaderIcon;
    public Texture SpeakerIcon;
    public Texture BackgroundIcon;
    public Texture SoundIcon;
    public Texture Layout;
    public StyleSheet styleSheet;
    public VisualTreeAsset xmlDoc;
    [MenuItem("Story Graph/Prototype/StoryBuilder Prefab Utility")]
    public static void ShowExample()
    {
        StoryBuilderPrefabUtility wnd = GetWindow<StoryBuilderPrefabUtility>();
        wnd.titleContent = new GUIContent("StoryBuilder Prefab Utility");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/StoryBuilderPrefabUtility.uss");
        root.styleSheets.Add(styleSheet);

        // Import UXML
        VisualElement f = xmlDoc.CloneTree();
        root.Add(f);

        AssignImage(root, "header-icon", HeaderIcon);
        AssignImage(root, "speaker-icon", SpeakerIcon);
        AssignImage(root, "background-icon", BackgroundIcon);
        AssignImage(root, "sound-icon", SoundIcon);
        AssignImage(root, "layout1", Layout);
        AssignImage(root, "layout2", Layout);
        AssignImage(root, "layout3", Layout);

        MakeFakeList(root, "story-list", SelectionType.Single);
        MakeFakeList(root, "speaker-list");
        MakeFakeList(root, "background-list");
        MakeFakeList(root, "sound-list");


        EnumField speakers = root.Query<EnumField>("popup");
        speakers.Init(SpeakerList.Speaker1);
        speakers.value = SpeakerList.Speaker1;

        //root.Query<Image>("asset-speakers").ForEach<Image>((i) => {
        //});
        //root.Query<Box>("asset-speakers").Children<Image>().ForEach((i) => {            
        //    i.image = (counter % 2 == 0)
        //       ? checkMark
        //       : redX;
        //    counter++;
        //});
    }

    void MakeFakeList(VisualElement root, string listName, SelectionType selectionType = SelectionType.None)
    {
        ListView fakeList = root.Query<ListView>(listName);

        List<string> items = new List<string>();
        int total = 5;
        string filename = "asset_file";
        if (selectionType == SelectionType.Single)
        {
            total = 20;
            filename = "story_file";
            fakeList.selectedIndex = 0;
        }
        for (int i = 0; i < total; i++)
        {
            items.Add(filename + i);
        }
        List<string> itemsToList = new List<string>() {
            "StoryScript.sgl",
            "OtherStoryScript.sgl",
            "StoryScript.sgl",
            "OtherStoryScript.sgl",
            "StoryScript.sgl",
            "OtherStoryScript.sgl",
        };
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];
        fakeList.makeItem = makeItem;
        fakeList.bindItem = bindItem;
        fakeList.itemsSource = items;
        fakeList.selectionType = selectionType;

        // Callback invoked when the user double clicks an item
        //fakeList.onItemChosen += obj => Debug.Log(obj);

        // Callback invoked when the user changes the selection inside the ListView
        //fakeList.onSelectionChanged += objects => Debug.Log(objects);
    }

    void AssignImage(VisualElement root, string element, Texture image)
    {
        Image xmlImage = root.Query<Image>(element);
        if (xmlImage != null)
        {
            xmlImage.image = image;
        }
        else
        {
            Debug.Log("Element " + element + " not found");
        }
    }
}