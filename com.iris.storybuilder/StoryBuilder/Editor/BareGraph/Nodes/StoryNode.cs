using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;
using static StoryBuilderWindow;

public abstract class StoryNode : Node
{
	protected static StyleSheet styleSheet;

    protected static int GlobalIndex = 1;
    protected TextField titleField;

    protected Label titleLabel;
    protected Label typeLabel;
	protected Image titleIcon;

    protected virtual string DefaultTitle => GetType().ToString().Replace("Node", "");

    protected bool collapsedSelf;

    public string NodeTitle
    {
        get => titleLabel.text;
        set
        {
	        titleLabel.text = value;

	        if (titleField != null)
	        {
		        titleField.value = value;
			}
        }
    }

    protected StoryNode(bool isRenamable = true, bool isCollapsible = true)
    {
		string nodeGuid = Guid.NewGuid().ToString();
		
		capabilities |= Capabilities.Movable;
        if (isRenamable)
        {
	        capabilities |= Capabilities.Renamable;
		}

        if (isCollapsible)
        {
	        capabilities |= Capabilities.Collapsible;
        }
        
		viewDataKey = nodeGuid;
        name = nodeGuid;

		titleContainer.RemoveAt(0); //Remove old title label
        titleContainer.AddToClassList("title-container");
		
		titleIcon = new Image();
		titleContainer.Add(titleIcon);

		VisualElement titleTextContainer = new VisualElement { name = "title-text-container" };
		titleContainer.Add(titleTextContainer);

		titleLabel = new Label { name = "title-label" };
		titleTextContainer.Add(titleLabel);
		
		typeLabel = new Label { name = "type-label" };
		titleTextContainer.Add(typeLabel);

		if (isRenamable)
		{
			titleField = new TextField
				             {
					             name = "title-field",
					             label = "Title"
				             };

			titleField.RegisterValueChangedCallback(OnTitleChanged);

			AddElementToExtensionContainer(titleField);
		}
		
		NodeTitle = DefaultTitle + GlobalIndex++;

        if (styleSheet == null)
        {
	        styleSheet = Resources.Load<StyleSheet>("Graphs/Stylesheets/NodeStylesheet");
        }
		styleSheets.Add(styleSheet);

		//TODO Remove this hack once GraphView makes not being able to collapse on all ports connected configurable
		var hiddenInputNode = InstantiatePort(
			Orientation.Horizontal,
			Direction.Input,
			Port.Capacity.Single,
			typeof(ControlFlow));

		hiddenInputNode.name = "secret-input-port";
		inputContainer.Add(hiddenInputNode);

#if UNITY_2020_1_OR_NEWER
		m_CollapseButton.RegisterCallback<ClickEvent>(OnCollapseButtonClicked);
#endif
    }

    public IEnumerable<StoryNode> GetNextNodes()
    {
        return outputContainer.Query<Port>().Where(
            p => p.portType == typeof(ControlFlow)
            ).ToList().Select(
            p => p.connections.FirstOrDefault()?.input.node as StoryNode);
    }

    public IEnumerable<TextNode> GetComments()
    {
        return inputContainer.Query<Port>().Where(
            p => p.connected
            ).ToList().Select(
            p => p.connections
            ).SelectMany(
            connections => connections
            ).Select(
            p => p.output.node as TextNode
            ).Where(
            n => n != null && n.type == TextNode.TextType.Comment);
    }

    public virtual void SetCollapsedGlobal(bool collapsed)
    {
		if (collapsed != collapsedSelf)
		{
			collapsedSelf = collapsed;
			ToggleCollapse();
	    }
    }

    protected void SetIcon(string resourcePath)
    {
	    titleIcon.name = "title-icon";
		titleIcon.image = Resources.Load<Texture>(resourcePath);
    }

    protected void AddElementToExtensionContainer(VisualElement element)
    {
		extensionContainer.Add(element);
		RefreshExpandedState();
	}

	private void OnTitleChanged(ChangeEvent<string> evt)
    {
	    titleLabel.text = evt.newValue;
    }

#if UNITY_2020_1_OR_NEWER
	private void OnCollapseButtonClicked(ClickEvent clickEvent)
	{
		collapsedSelf = !collapsedSelf;
	}
#endif
}