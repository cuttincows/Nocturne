using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShortcutManagement;

using UnityEngine;
using UnityEngine.UIElements;

public class StoryBuilderWindow : EditorWindow, ISearchWindowProvider
{
    // Empty class for ports and edges to be compatible
    public class ControlFlow { }

    private StoryBuilderView m_StoryBuilderView;
    private StartNode m_StartNode;

    private enum NodeType
    {
        Dialogue,
        Choice,
        Comment,
        If,
        Print,
        Variable,
        Jump,
    }

    private class PendingConnection
    {
        public StoryNode first { get; set; }
        public StoryNode second { get; set; }
    }

    [MenuItem("Story Graph/Show Graph Window")]
    public static void ShowWindow()
    {
        GetWindow<StoryBuilderWindow>();
    }

    private void OnEnable()
    {
        m_StoryBuilderView = new StoryBuilderView();

        m_StoryBuilderView.name = "theView";
        m_StoryBuilderView.viewDataKey = "theView";
        m_StoryBuilderView.StretchToParentSize();
        m_StoryBuilderView.graphViewChanged = GraphViewChanged;

        m_StoryBuilderView.nodeCreationRequest += OnRequestNodeCreation;

        rootVisualElement.Add(m_StoryBuilderView);

        var loadFileButton = new Button(OnLoadClick) { text = "Load", };
        var saveFileButton = new Button(OnSaveClick) { text = "Save", };
        var checkConsistencyButton = new Button(OnCheckConsistencyClick) { text = "Check Graph Consistency", };
        var clearCheckButton = new Button(OnCheckClearClick) { text = "Clear Check", };

        var buttonContainer = new VisualElement();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.Add(loadFileButton);
        buttonContainer.Add(saveFileButton);
        buttonContainer.Add(checkConsistencyButton);
        buttonContainer.Add(clearCheckButton);

        rootVisualElement.Add(buttonContainer);

		rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("Graphs/Stylesheets/GraphWindowStylesheet"));

        Reload();
    }

    private void OnCheckClearClick()
    {
        rootVisualElement.Query<StoryNode>().ForEach(n => n.style.backgroundColor = Color.clear);
    }

    private void OnCheckConsistencyClick()
    {
        var liveNodes = GetLiveNodes();
        foreach (var node in liveNodes)
        {
            node.style.backgroundColor = Color.green;
        }

        foreach (var commentNode in liveNodes.SelectMany(node => node.GetComments()))
        {
            commentNode.style.backgroundColor = Color.green;
        }

        var labelsSoFar = new HashSet<string>();
        foreach (var node in liveNodes)
        {
            if (!labelsSoFar.Add(node.NodeTitle))
            {
                foreach (var badNode in liveNodes.Where(n => n.NodeTitle == node.NodeTitle))
                {
                    badNode.style.backgroundColor = Color.red;
                }
            }
        }
    }

    private HashSet<StoryNode> GetLiveNodes()
    {
        var nodesTraversed = new HashSet<StoryNode>();
        GetLiveNodesRecursive(m_StartNode, nodesTraversed);

        return nodesTraversed;
    }

    private void GetLiveNodesRecursive(StoryNode node, HashSet<StoryNode> nodesTraversed)
    {
        if (!nodesTraversed.Add(node))
        {
            // Early out, we've hit this node before
            return;
        }

        foreach (var nextNode in node.GetNextNodes().Where(n => n != null))
        {
            Debug.Log($"Next: {node.NodeTitle} -> {nextNode.NodeTitle}");
            GetLiveNodesRecursive(nextNode, nodesTraversed);
        }
    }

    private void OnSaveClick()
    {
        var saveFile = EditorUtility.SaveFilePanel("Save Story", Application.dataPath, "story", "txt");
        if (string.IsNullOrEmpty(saveFile))
        {
            return;
        }

        var nodesToSave = GetLiveNodes();
        var preStripStringBuilder = new StringBuilder();

        // Serialise every node
        foreach (var node in nodesToSave)
        {
            preStripStringBuilder.AppendLine($"label {node.NodeTitle}");

            if (node is StartNode)
            {
                continue;
            }

            foreach (var commentNode in node.GetComments())
            {
                foreach (string line in commentNode.Text.Split('\n'))
                {
                    preStripStringBuilder.AppendLine($"# {line}");
                }
            }

            var textNode = node as TextNode;
            if (textNode != null)
            {
                switch (textNode.type)
                {
                    case TextNode.TextType.Dialogue:
                        if (textNode.Text.Contains("\n"))
                        {
                            preStripStringBuilder.AppendLine($"say \"\"\"\n{textNode.Text}\n\"\"\"");
                        }
                        else
                        {
                            preStripStringBuilder.AppendLine($"say \"{textNode.Text}\"");
                        }
                        break;
                    case TextNode.TextType.Print:
                        preStripStringBuilder.AppendLine($"print \"{textNode.Text.Replace('\n', ' ')}\"");
                        break;
                    case TextNode.TextType.Jump:
                        preStripStringBuilder.AppendLine($"jump \"{textNode.Text.Replace('\n', ' ')}\"");
                        break;
                    case TextNode.TextType.Comment:
                    default:
                        throw new InvalidOperationException($"Invalid text node type: {textNode.type}");
                }
                preStripStringBuilder.AppendLine($"goto {textNode.GetNextNodes().FirstOrDefault()?.NodeTitle ?? "end"}");

                continue;
            }

            var choiceNode = node as ChoiceNode;
            if (choiceNode != null)
            {
                preStripStringBuilder.AppendLine("choice:");
                foreach (var choice in choiceNode.Choices)
                {
                    string jumpPoint = choice.output.connected ?
                        (choice.output.connections.First().input.node as StoryNode).NodeTitle
                        : "end";
                    preStripStringBuilder.AppendLine($"\t{choice.text}:\n\t\tgoto {jumpPoint}");
                }

                continue;
            }

            var ifNode = node as IfNode;
            if (ifNode != null)
            {
                var nextNodes = ifNode.GetNextNodes();
                var ifTrue = nextNodes.First();
                var ifFalse = nextNodes.ElementAt(1);

                preStripStringBuilder.AppendLine($"if {ifNode.condition}:\n\tgoto {ifTrue?.NodeTitle ?? "end"}");
                if (ifFalse != null)
                {
                    preStripStringBuilder.AppendLine($"else:\n\tgoto {ifFalse.NodeTitle}");
                }

                continue;
            }

            var variableNode = node as VariableNode;
            if (variableNode != null)
            {
                preStripStringBuilder.AppendLine($"var {variableNode.variableName} = {variableNode.variableValue}");
                preStripStringBuilder.AppendLine($"goto {variableNode.GetNextNodes().FirstOrDefault()?.NodeTitle ?? "end"}");
                continue;
            }

            throw new InvalidOperationException($"Hit some unknown node: {node.GetType()}");
        }

        preStripStringBuilder.AppendLine("label end");

        // Strip out unnecessary goto that lead to next line
        string[] preStripLines = preStripStringBuilder.ToString().Split('\n');
        var firstPassStringBuilder = new StringBuilder();
        for (int i = 0; i < preStripLines.Length; i++)
        {
            if (preStripLines[i].StartsWith("goto") && preStripLines[i + 1].StartsWith("label"))
            {
                var gotoLabel = preStripLines[i].Trim().Remove(0, 5);
                var labelLabel = preStripLines[i + 1].Trim().Remove(0, 6);
                if (gotoLabel == labelLabel)
                {
                    Debug.Log($"Stripping at line {i}: {preStripLines[i]}");
                    continue;
                }
            }

            firstPassStringBuilder.AppendLine(preStripLines[i]);
        }

        // Strip out any unused labels
        string[] firstPassLines = firstPassStringBuilder.ToString().Split('\n');
        var secondPassStringBuilder = new StringBuilder();
        for (int i = 0; i < firstPassLines.Length; i++)
        {
            if (firstPassLines[i].StartsWith("label"))
            {
                var expectedGoto = "goto " + firstPassLines[i].Trim().Remove(0, 6);
                var found = false;
                foreach (var line in firstPassLines)
                {
                    if (line.Trim() == expectedGoto)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Debug.Log($"Stripping at line {i}: {firstPassLines[i]}");
                    continue;
                }
            }

            secondPassStringBuilder.AppendLine(firstPassLines[i]);
        }

        var finalString = secondPassStringBuilder.ToString().Replace("\r\n", "\n");
        File.WriteAllText(saveFile, finalString);
    }

    private void OnLoadClick()
    {
        var filePathToLoad = EditorUtility.OpenFilePanel("Load Story", Application.dataPath, "txt");
        if (string.IsNullOrEmpty(filePathToLoad))
        {
            return;
        }

        var fileToLoad = File.OpenText(filePathToLoad);
        string[] allLines = fileToLoad.ReadToEnd().Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        bool firstNode = true;
        var commentsToHandle = new List<string>();
        var pendingConnections = new List<PendingConnection>();
        string nextNodeTitle = null;
        var nodeCount = 0;
        for (int i = 0; i < allLines.Length; i++)
        {
            if (allLines[i].Trim().StartsWith("#"))
            {
                commentsToHandle.Add(allLines[i].Trim().Remove(0, 1));
                continue;
            }

            if (allLines[i].Contains("#"))
            {
                int commentStartIndex = allLines[i].IndexOf("#");
                commentsToHandle.Add(allLines[i].Substring(commentStartIndex + 1));
                allLines[i] = allLines[i].Remove(commentStartIndex);
            }

            if (allLines[i].Trim().StartsWith("label"))
            {
                nextNodeTitle = allLines[i].Trim().Substring(6);
                continue;
            }

            StoryNode newNode;

            if (allLines[i].Trim().StartsWith("say"))
            {
                newNode = new TextNode("", TextNode.TextType.Dialogue);
                var textNode = newNode as TextNode;
                if (allLines[i].Contains("\"\"\""))
                {
                    i++;
                    while (!allLines[i].Contains("\"\"\""))
                    {
                        textNode.Text += allLines[i].Trim() + "\n";
                        i++;
                    }
                }
                else
                {
                    textNode.Text = allLines[i].Replace("say", "").Replace("\"", "").Trim();
                }
            }
            else if (allLines[i].Trim().StartsWith("print"))
            {
                newNode = new TextNode("", TextNode.TextType.Print);
                var textNode = newNode as TextNode;
                textNode.Text = allLines[i].Replace("print", "").Replace("\"", "").Trim();
            }
            else if (allLines[i].Trim().StartsWith("jump"))
            {
                newNode = new TextNode("", TextNode.TextType.Jump);
                var textNode = newNode as TextNode;
                textNode.Text = allLines[i].Replace("jump", "").Replace("\"", "").Trim();
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse string: {ToLiteral(allLines[i])}");
            }

            if (pendingConnections.LastOrDefault() != null)
            {
                pendingConnections.LastOrDefault().second = newNode;
            }

            pendingConnections.Add(new PendingConnection { first = newNode, second = null, });

            if (nextNodeTitle != null)
            {
                newNode.NodeTitle = nextNodeTitle;
                nextNodeTitle = null;
            }

            m_StoryBuilderView.AddElement(newNode);
            newNode.SetPosition(new Rect(100 + nodeCount * 400, 0, 0, 0));

            int commentCount = 0;
            foreach (var comment in commentsToHandle)
            {
                var commentNode = new TextNode(comment, TextNode.TextType.Comment);
                m_StoryBuilderView.AddElement(commentNode);
                Rect commentPosition = new Rect(-100 + nodeCount * 400, 100 + 100 * commentCount, 0, 0);
                commentNode.SetPosition(commentPosition);
                var port1 = commentNode.outputContainer.Q<Port>();
                var port2 = newNode.inputContainer.Q<Port>();
                var edge = port1.ConnectTo(port2);
                m_StoryBuilderView.AddElement(edge);
                commentCount++;
            }
            commentsToHandle.Clear();

            if (firstNode)
            {
                firstNode = false;
                var port1 = m_StartNode.outputContainer.Q<Port>();
                var port2 = newNode.inputContainer.Q<Port>();
                var edge = port1.ConnectTo(port2);
                m_StoryBuilderView.AddElement(edge);
            }

            nodeCount++;
        }

        foreach (var connection in pendingConnections)
        {
            if (connection.first != null && connection.second != null)
            {
                var port1 = connection.first.outputContainer.Q<Port>();
                var port2 = connection.second.inputContainer.Q<Port>();
                var edge = port1.ConnectTo(port2);
                m_StoryBuilderView.AddElement(edge);
            }
        }
    }

    private void OnRequestNodeCreation(NodeCreationContext context)
    {
        SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), this);
    }

    private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
    {
        return graphViewChange;
    }

    public void Reload()
    {
        if (m_StoryBuilderView == null)
            return;

        var miniMap = new MiniMap();
        m_StoryBuilderView.Add(miniMap);
		miniMap.SetPosition(new Rect(0, this.position.height - 176, 200, 176));

        m_StartNode = new StartNode();
        m_StoryBuilderView.AddElement(m_StartNode);
    }

    List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>();

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Story nodes"), 0));

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Story Flow"), 1));
        tree.Add(new SearchTreeEntry(new GUIContent("Dialogue")) { level = 2, userData = NodeType.Dialogue });
        tree.Add(new SearchTreeEntry(new GUIContent("Choice")) { level = 2, userData = NodeType.Choice });
        tree.Add(new SearchTreeEntry(new GUIContent("If")) { level = 2, userData = NodeType.If });
        tree.Add(new SearchTreeEntry(new GUIContent("Jump")) { level = 2, userData = NodeType.Jump });

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Variables"), 1));
        tree.Add(new SearchTreeEntry(new GUIContent("Variable")) { level = 2, userData = NodeType.Variable });

        tree.Add(new SearchTreeGroupEntry(new GUIContent("Debug"), 1));
        tree.Add(new SearchTreeEntry(new GUIContent("Print")) { level = 2, userData = NodeType.Print });
        tree.Add(new SearchTreeEntry(new GUIContent("Comment")) { level = 2, userData = NodeType.Comment });


#if UNITY_2020_1_OR_NEWER
		tree.Add(new SearchTreeEntry(new GUIContent("Note")) {level = 1} );
#endif

		return tree;
    }

    bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
    {
        if (entry is SearchTreeGroupEntry)
            return false;

#if UNITY_2020_1_OR_NEWER
		if (entry.name == "Note")
        {
			StickyNote stickyNote = new StickyNote();
			m_StoryBuilderView.AddElement(stickyNote);

			SetGraphElementStartPosition(stickyNote, context.screenMousePosition);

			return true;
        }
#endif

		Node newNode;
        var nodeType = entry.userData as NodeType?;
        switch (nodeType)
        {
            case NodeType.Dialogue:
                newNode = new TextNode("A fool, a fool! I met a fool i' the forest,\nA motley fool; a miserable world!\nAs I do live by food, I met a fool\nWho laid him down and bask'd him in the sun,\nAnd rail'd on Lady Fortune in good terms,\nIn good set terms and yet a motley fool.\n'Good morrow, fool,' quoth I. 'No, sir,' quoth he,\n'Call me not fool till heaven hath sent me fortune:'\nAnd then he drew a dial from his poke,\nAnd, looking on it with lack-lustre eye,\nSays very wisely, 'It is ten o'clock:\nThus we may see,' quoth he, 'how the world wags:\n'Tis but an hour ago since it was nine,\nAnd after one hour more 'twill be eleven;\nAnd so, from hour to hour, we ripe and ripe,\nAnd then, from hour to hour, we rot and rot;\nAnd thereby hangs a tale.' When I did hear\nThe motley fool thus moral on the time,\nMy lungs began to crow like chanticleer,\nThat fools should be so deep-contemplative,\nAnd I did laugh sans intermission\nAn hour by his dial. O noble fool!\nA worthy fool! Motley's the only wear.", TextNode.TextType.Dialogue);
                break;
            case NodeType.Choice:
                newNode = new ChoiceNode(new[] { "Kiss me, Horace", "You always did hate that vase", "You talk a lot for a man with no hats", "And yet, here we are, in your father's flower shop" });
                break;
            case NodeType.Comment:
                newNode = new TextNode("You know what they say, commenting is useful", TextNode.TextType.Comment);
                break;
            case NodeType.If:
                newNode = new IfNode();
                break;
            case NodeType.Print:
                newNode = new TextNode("This is an important debug message, y'all!", TextNode.TextType.Print);
                break;
            case NodeType.Variable:
                newNode = new VariableNode("x", "1");
                break;
            case NodeType.Jump:
                newNode = new TextNode("newFile", TextNode.TextType.Jump);
                break;
            default:
                throw new InvalidOperationException($"Unknown node type: {nodeType?.ToString() ?? "null"}");
        }

        m_StoryBuilderView.AddElement(newNode);

		SetGraphElementStartPosition(newNode, context.screenMousePosition);

        return true;
    }

    private void SetGraphElementStartPosition(GraphElement element, Vector2 screenMousePosition)
    {
		element.SetPosition(new Rect(element.parent.WorldToLocal(screenMousePosition) - new Vector2(position.x, position.y), Vector2.zero));
	}


	private static string ToLiteral(string input)
    {
        using (var writer = new StringWriter())
        {
            using (var provider = CodeDomProvider.CreateProvider("CSharp"))
            {
                provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                return writer.ToString();
            }
        }
    }

	[ContextMenu("Hidden/test #_3")]
	static void toggleCollapseShortcut()
	{

	}
}