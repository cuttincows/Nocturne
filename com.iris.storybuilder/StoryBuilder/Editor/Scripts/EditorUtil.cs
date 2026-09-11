using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorUtil
{
    public static Dictionary<string, T> GetAssets<T>() where T : Object
    {
        Dictionary<string, T> results = new Dictionary<string, T>();
        T[] assets = Resources.FindObjectsOfTypeAll<T>();
        for (int i = 0; i < assets.Length; i++)
        {
            if (!results.ContainsKey(assets[i].name))
            {
                results.Add(assets[i].name, assets[i]);
            }
        }
        return results;
    }

    public static Dictionary<string, T> GetAssetsOfType<T>() where T : Object
    {
        Dictionary<string, T> results = new Dictionary<string, T>();
        T[] assets = Resources.FindObjectsOfTypeAll<T>();
        for (int i = 0; i < assets.Length; i++)
        {
            if (!results.ContainsKey(assets[i].name))
            {
                results.Add(assets[i].name, assets[i]);
            }
        }
        return results;
    }

    public static Dictionary<string, T> GetAllInstances<T>() where T : Object
    {
        Dictionary<string, T> result = new Dictionary<string, T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (!result.ContainsKey(asset.name))
            {
                result.Add(asset.name, asset);
            }
        }
        return result;
    }
}
