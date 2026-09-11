using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class VariableNode : StoryNode
{
    private readonly TextField m_NameField;
    private readonly TextField m_ValueField;

    public string variableName => m_NameField.value;
    public string variableValue => m_ValueField.value;

    public VariableNode(string variableName, string variableValue)
    {
        var nameContainer = new VisualElement();
        nameContainer.style.flexGrow = 1;
        nameContainer.style.flexDirection = FlexDirection.Row;
        nameContainer.style.minWidth = 150;
        var nameLabel = new Label("Name");
        nameLabel.style.marginRight = 10;
        m_NameField = new TextField() { value = variableName };
        m_NameField.style.flexGrow = 1;
        nameContainer.Add(nameLabel);
        nameContainer.Add(m_NameField);

        var valueContainer = new VisualElement();
        valueContainer.style.flexGrow = 1;
        valueContainer.style.flexDirection = FlexDirection.Row;
        valueContainer.style.minWidth = 150;
        var valueLabel = new Label("Value");
        valueLabel.style.marginRight = 11;
        m_ValueField = new TextField() { value = variableValue };
        m_ValueField.style.flexGrow = 1;
        valueContainer.Add(valueLabel);
        valueContainer.Add(m_ValueField);

        extensionContainer.Add(nameContainer);
        extensionContainer.Add(valueContainer);

        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StoryBuilderWindow.ControlFlow));
        inputPort.portName = "In";
        inputPort.name = "in";

        inputContainer.Add(inputPort);

        var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(StoryBuilderWindow.ControlFlow));
        outputPort.portName = "Out";
        outputPort.name = "out";

        outputContainer.Add(outputPort);

        typeLabel.text = "Set Variable";
        SetIcon("Graphs/Icons/variable");

		RefreshExpandedState();
	}
}

