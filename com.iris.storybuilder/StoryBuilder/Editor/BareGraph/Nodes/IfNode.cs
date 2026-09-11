using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class IfNode : StoryNode
{
    private readonly TextField m_conditionField;

    public string condition
    {
        get
        {
            return m_conditionField.text;
        }
    }

    public IfNode()
    {
        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StoryBuilderWindow.ControlFlow));
        inputPort.portName = "In";
        inputPort.name = "in";
		
        var conditionContainer = new VisualElement();
        var conditionLabel = new Label("Condition");
        conditionLabel.style.marginLeft = 4;
        conditionLabel.style.marginRight = 24;
        m_conditionField = new TextField() { value = "a == b" };
        m_conditionField.style.flexGrow = 1;
        conditionContainer.Add(conditionLabel);
        conditionContainer.Add(m_conditionField);

		extensionContainer.Add(conditionContainer);

        var outputTruePort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(StoryBuilderWindow.ControlFlow));
        outputTruePort.portName = "True";
        outputTruePort.name = "true";

        var outputFalsePort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(StoryBuilderWindow.ControlFlow));
        outputFalsePort.portName = "False";
        outputFalsePort.name = "false";

        inputContainer.Add(inputPort);
        outputContainer.Add(outputTruePort);
        outputContainer.Add(outputFalsePort);

        typeLabel.text = "If";
        SetIcon("Graphs/Icons/if");

		RefreshExpandedState();
	}
}

