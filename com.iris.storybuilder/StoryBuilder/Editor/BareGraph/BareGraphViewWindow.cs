using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BareGraph
{
    public class BareGraphViewWindow : EditorWindow, ISearchWindowProvider
    {
        public BareGraphView bareGraphView { get; private set; }
        public BareModel bareModel { get; private set; }

        [MenuItem("Bare GraphView/BareGraph")]
        public static void ShowWindow()
        {
            GetWindow<BareGraphViewWindow>();
        }

        private void OnEnable()
        {
            // Load the asset.
            var assets = AssetDatabase.LoadAllAssetsAtPath(BareModelUtil.GetAssetPath());
            if (assets == null || assets.Length == 0)
            {
                BareModelUtil.ResetOrCreateAsset();
                assets = AssetDatabase.LoadAllAssetsAtPath(BareModelUtil.GetAssetPath());

                if (assets == null || assets.Length == 0)
                {
                    Debug.LogError("Could not load asset.");
                    return;
                }
            }

            // Find the asset.
            foreach (var asset in assets)
            {
                if (asset is BareModel)
                {
                    bareModel = asset as BareModel;
                    break;
                }
            }

            bareGraphView = new BareGraphView();

            bareGraphView.name = "theView";
            bareGraphView.viewDataKey = "theView";
            bareGraphView.StretchToParentSize();
            bareGraphView.graphViewChanged = GraphViewChanged;

            bareGraphView.nodeCreationRequest += OnRequestNodeCreation;

            rootVisualElement.Add(bareGraphView);

            Reload();
        }

        protected void OnRequestNodeCreation(NodeCreationContext context)
        {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), this);
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
            tree.Add(new SearchTreeEntry(new GUIContent("BareNode")) { level = 1, userData = typeof(BareNodeModel) });

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry is SearchTreeGroupEntry)
                return false;

            var nodeModel = ScriptableObject.CreateInstance(entry.userData as Type) as BareNodeModel;
            nodeModel.name = nodeModel.assetName;
            nodeModel.title = "New Node";

            bareModel.Add(nodeModel);

            var nodeUI = CreateNode(nodeModel);

            Vector2 pointInWindow = context.screenMousePosition;
            Vector2 pointInGraph = nodeUI.parent.WorldToLocal(pointInWindow);

            nodeUI.SetPosition(new Rect(pointInGraph, Vector2.zero));
            nodeModel.position = pointInGraph;

            return true;
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is Node)
                        bareModel.Remove(element.userData as BareNodeModel);
                    else if (element is Edge)
                        bareModel.Remove(element.userData as BareEdgeModel);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    var edgeModel = ScriptableObject.CreateInstance<BareEdgeModel>();
                    edgeModel.input = edge.input.node.userData as BareNodeModel;
                    edgeModel.output = edge.output.node.userData as BareNodeModel;
                    edgeModel.name = edgeModel.assetName;

                    edge.viewDataKey = edgeModel.guid;
                    edge.userData = edgeModel;

                    bareModel.Add(edgeModel);
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (GraphElement element in graphViewChange.movedElements)
                {
                    if (element is Node)
                        (element.userData as BareNodeModel).position = element.GetPosition().position;
                }
            }

            return graphViewChange;
        }

        private Node CreateNode(BareNodeModel nodeModel)
        {
            var nodeUI = new Node();
            nodeUI.title = nodeModel.title;
            nodeUI.capabilities |= Capabilities.Movable;
            nodeUI.viewDataKey = nodeModel.guid;
            nodeUI.name = nodeModel.guid;
            nodeUI.userData = nodeModel;
            nodeUI.SetPosition(new Rect(nodeModel.position, Vector2.zero));

            var inputPort = Port.Create<Edge>(
                Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Input";
            inputPort.name = "input";

            var outputPort = Port.Create<Edge>(
                Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Output";
            outputPort.name = "output";

            nodeUI.inputContainer.Add(inputPort);
            nodeUI.outputContainer.Add(outputPort);

            bareGraphView.AddElement(nodeUI);

            return nodeUI;
        }

        private void CreateEdge(BareEdgeModel edgeModel)
        {
            var inputGuid = edgeModel.input.guid;
            var outputGuid = edgeModel.output.guid;

            var inputNode = bareGraphView.Q(inputGuid);
            var outputNode = bareGraphView.Q(outputGuid);

            var inputPort = inputNode.Q<Port>("input");
            var outputPort = outputNode.Q<Port>("output");

            var edge = inputPort.ConnectTo(outputPort);
            edge.viewDataKey = edgeModel.guid;
            edge.userData = edgeModel;

            bareGraphView.AddElement(edge);
        }

        public void Reload()
        {
            if (bareGraphView == null)
                return;

            if (bareModel == null)
                return;

            foreach (var nodeModel in bareModel.nodes)
                CreateNode(nodeModel);

            foreach (var edgeModel in bareModel.edges)
                CreateEdge(edgeModel);

            // Add the minimap.
            var miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(0, 372, 200, 176));
            bareGraphView.Add(miniMap);
        }
    }
}
