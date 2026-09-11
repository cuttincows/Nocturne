using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class ChoiceNode : StoryNode
{
    public class Choice
    {
        public string text { get; set; }
        public Port output { get; set; }
        public VisualElement container { get; set; }
    }

    public readonly List<Choice> Choices;

    private int m_ChoiceIndex = 0;
    private Button m_AddChoiceButton;

    public ChoiceNode(IEnumerable<string> choices)
    {
        Choices = new List<Choice>();

        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StoryBuilderWindow.ControlFlow));
        inputPort.portName = "In";
        inputPort.name = "in";

        inputContainer.Add(inputPort);

        foreach (var choice in choices)
        {
            AddChoice(choice);
        }

        m_AddChoiceButton = new Button(OnAddChoiceClick);
        m_AddChoiceButton.text = "+";
        m_AddChoiceButton.style.alignSelf = Align.FlexEnd;
		m_AddChoiceButton.AddToClassList("choice-container");
        outputContainer.Add(m_AddChoiceButton);

        typeLabel.text = "Choice";
		SetIcon("Graphs/Icons/choice");

		RefreshExpandedState();
    }

    private void OnAddChoiceClick()
    {
        string choiceText = "New Option";
        AddChoice(choiceText);
    }

    protected override void ToggleCollapse()
    {
	    base.ToggleCollapse();

	    foreach (var choice in Choices)
	    {
		    choice.container.SetEnabled(expanded);
	    }

		m_AddChoiceButton.SetEnabled(expanded);
    }

    private void AddChoice(string choice)
    {
        int index = m_ChoiceIndex++;

        var choiceContainer = new VisualElement();
        choiceContainer.style.flexDirection = FlexDirection.Row;

        var choiceText = new TextField { value = choice };
        choiceText.RegisterValueChangedCallback(evt => OnChoiceValueChange(evt, index));
        choiceText.style.flexGrow = 1f;

        var choicePort = Port.Create<Edge>(
            Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(StoryBuilderWindow.ControlFlow));
        choicePort.portName = "";

        choiceContainer.Add(choiceText);
        choiceContainer.Add(choicePort);

		choiceContainer.AddToClassList("choice-container");

        outputContainer.Add(choiceContainer);

        Choices.Add(
	        new Choice
		        {
			        text = choice,
			        output = choicePort,
					container = choiceContainer
		        });

        m_AddChoiceButton?.BringToFront();
    }

    private void OnChoiceValueChange(ChangeEvent<string> evt, int index)
    {
        Choices[index].text = evt.newValue;
    }
}

