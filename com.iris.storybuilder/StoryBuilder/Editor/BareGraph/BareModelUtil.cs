using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BareGraph
{
    public static class BareModelUtil
    {
        public static string GetAssetPath()
        {
            return "Assets/Model/BareDemoModel.asset";
        }

        [MenuItem("Bare GraphView/Reset or Create Asset")]
        public static void ResetOrCreateAsset()
        {
            var model = GenerateDemoModel();

            AssetDatabase.CreateAsset(model, GetAssetPath());

            foreach (var node in model.nodes)
                AssetDatabase.AddObjectToAsset(node, model);

            foreach (var edge in model.edges)
                AssetDatabase.AddObjectToAsset(edge, model);

            AssetDatabase.SaveAssets();
        }

        public static BareModel GenerateDemoModel()
        {
            var model = ScriptableObject.CreateInstance<BareModel>();

            var node1 = ScriptableObject.CreateInstance<BareNodeModel>();
            node1.name = node1.assetName;
            node1.position = new Vector2(100, 100);
            node1.title = "Node 1";

            var node2 = ScriptableObject.CreateInstance<BareNodeModel>();
            node2.name = node1.assetName;
            node2.position = new Vector2(300, 200);
            node2.title = "Node 2";

            var node3 = ScriptableObject.CreateInstance<BareNodeModel>();
            node3.name = node1.assetName;
            node3.position = new Vector2(500, 300);
            node3.title = "Node 3";

            var edge1to2 = ScriptableObject.CreateInstance<BareEdgeModel>();
            edge1to2.name = edge1to2.assetName;
            edge1to2.output = node1;
            edge1to2.input = node2;

            var edge2to3 = ScriptableObject.CreateInstance<BareEdgeModel>();
            edge2to3.name = edge1to2.assetName;
            edge2to3.output = node2;
            edge2to3.input = node3;

            model.nodes.Add(node1);
            model.nodes.Add(node2);
            model.nodes.Add(node3);

            model.edges.Add(edge1to2);
            model.edges.Add(edge2to3);

            return model;
        }
    }
}
