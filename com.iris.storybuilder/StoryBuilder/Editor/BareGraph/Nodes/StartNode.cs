using UnityEditor.Experimental.GraphView;

public class StartNode : StoryNode
{
	public StartNode(): base(false, false)
    {
	    capabilities &= ~Capabilities.Deletable;
	    capabilities &= ~Capabilities.Collapsible;

	    name = "start-node";
		NodeTitle = "Start";

		titleContainer.name = "start-title-container";
		titleContainer.Clear();
		titleContainer.Add(titleLabel);
		
        var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(StoryBuilderWindow.ControlFlow));
        outputPort.portName = "Out";
        outputPort.name = "out";

        outputContainer.Add(outputPort);
    }

	public override void SetCollapsedGlobal(bool collapsed) {}
}