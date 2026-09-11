using UnityEditor;
using UnityEngine;

public static class StoryBuilderMenus
{
    [MenuItem("GameObject/Story Graph/Characters Dialogue", false, 10)]
    private static void CreateCharactersDialogue(MenuCommand menuCommand)
    {
        var prefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.unity.story-graph/StoryBuilder/Runtime/UI/Prefabs/CharactersDialogue.prefab");

        InstantiateGameObject(prefab, menuCommand);
    }

    private static void InstantiateGameObject(GameObject prefab, MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        Object result = PrefabUtility.InstantiatePrefab(prefab, parent != null ? parent.transform : null);
        Undo.RegisterCreatedObjectUndo(result, $"Create {prefab.name}");
        Selection.activeObject = result;
    }
}
