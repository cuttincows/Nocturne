using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TextNode : StoryNode
{
    private readonly TextField textField;

    public enum TextType
    {
        Dialogue,
        Comment,
        Print,
        Jump,
    };

    public TextType type { get; }

    public string Text
    {
        get => textField.value;
        set => textField.value = value;
    }

    protected override string DefaultTitle => GetTypeName();

    public TextNode(string dialogue, TextType nodeType): base(nodeType != TextType.Comment)
    {
        type = nodeType;
        NodeTitle = DefaultTitle + (GlobalIndex - 1);

		if (type != TextType.Comment)
        {
            var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StoryBuilderWindow.ControlFlow));
            inputPort.portName = "In";
            inputPort.name = "in";

            inputContainer.Add(inputPort);
		}

        var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, (type == TextType.Comment) ? typeof(object) : typeof(StoryBuilderWindow.ControlFlow));
        outputPort.portName = "Out";
        outputPort.name = "out";
        outputPort.portColor = (type == TextType.Comment) ? new Color(0.341f, 0.651f, 0.290f) : Color.white;

        outputContainer.Add(outputPort);

        textField = new TextField
        {
            value = dialogue,
            multiline = (nodeType != TextType.Jump) && (nodeType != TextType.Print),
        };

		AddElementToExtensionContainer(textField);

        InitTitleContainer();
        typeLabel.text = GetTypeName();
    }

	private void InitTitleContainer()
	{
		switch (type)
		{
			case TextType.Dialogue:
				titleContainer.AddToClassList("dialogue-node-title-container");
				SetIcon("Graphs/Icons/dialogue");
				return;
			case TextType.Comment:
				titleContainer.AddToClassList("comment-node-title-container");
				SetIcon("Graphs/Icons/comment");
				return;
			case TextType.Print:
				titleContainer.AddToClassList("print-node-title-container");
				SetIcon("Graphs/Icons/print");
				return;
			case TextType.Jump:
				titleContainer.AddToClassList("jump-node-title-container");
				SetIcon("Graphs/Icons/jump");
				return;
			default:
				throw new NotImplementedException("Unimplemented type: " + type);
		}
	}

	private string GetTypeName()
	{
		switch (type)
		{
			case TextType.Dialogue: return "Dialogue";
			case TextType.Comment: return "Comment";
			case TextType.Print: return "Print";
			case TextType.Jump: return "Jump";
			default:
				throw new NotImplementedException("Unimplemented type: " + type);
		}}
}
