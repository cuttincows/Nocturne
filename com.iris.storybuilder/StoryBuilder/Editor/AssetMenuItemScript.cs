using System.IO;
using System.Linq;
using UnityEditor;

public static class AssetMenuItemScript
{
    [MenuItem("Assets/Create/StoryBuilder/New Script")]
    public static void CreateAsset()
    {
        string path = AssetDatabase.GetAssetPath (Selection.activeObject);
        var files = Directory.GetFiles(path);
        var fileNames = files.Where(f => Path.GetExtension(f) == ".sgl").Select(Path.GetFileNameWithoutExtension).ToList();

        var fileNameNewInit = "New Script";
        var fileNameNew = fileNameNewInit;
        int collisionCount = 0;
        for (int i = 0; i < fileNames.Count; i++)
        {
            if (fileNames[i].Equals(fileNameNew))
            {
                i = 0;
                collisionCount++;
                fileNameNew = fileNameNewInit + collisionCount;
            }
        }

        File.Create($"{path}/{fileNameNew}.sgl");
    }
}