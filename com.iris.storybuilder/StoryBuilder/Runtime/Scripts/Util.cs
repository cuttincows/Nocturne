using System;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static List<int> AllIndexesOf(this string str, string value)
    {
        List<int> indexes = new List<int>();
        if (String.IsNullOrEmpty(value)) return indexes;

        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }

    public static Dictionary<string, T> GetAssets<T>() where T : UnityEngine.Object
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
}