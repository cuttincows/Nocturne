using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BareGraph
{
    public class BareModel : ScriptableObject
    {
        public List<BareNodeModel> nodes;
        public List<BareEdgeModel> edges;

        public BareModel()
        {
            nodes = new List<BareNodeModel>();
            edges = new List<BareEdgeModel>();
        }

        public void Add(BareNodeModel node)
        {
            nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }

        public void Add(BareEdgeModel edge)
        {
            edges.Add(edge);
            AssetDatabase.AddObjectToAsset(edge, this);
            AssetDatabase.SaveAssets();
        }

        public void Remove(BareNodeModel node)
        {
            if (node == null)
                return;

            AssetDatabase.RemoveObjectFromAsset(node);
            nodes.Remove(node);
            UnityEngine.Object.DestroyImmediate(node, true);
            AssetDatabase.SaveAssets();
        }

        public void Remove(BareEdgeModel edge)
        {
            if (edge == null)
                return;

            AssetDatabase.RemoveObjectFromAsset(edge);
            edges.Remove(edge);
            UnityEngine.Object.DestroyImmediate(edge, true);
            AssetDatabase.SaveAssets();
        }
    }
}