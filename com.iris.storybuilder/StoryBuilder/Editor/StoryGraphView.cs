using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class StoryBuilderView : GraphView
{
	public StoryBuilderView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // FIXME: add a coordinator so that ContentDragger and SelectionDragger cannot be active at the same time.
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        Insert(0, new GridBackground { name = "grid-background" });
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
	    base.BuildContextualMenu(evt);

	    if (evt.target is GraphView)
	    {
		    evt.menu.AppendAction("Collapse All", (a) => { SetCollapsedCallback(true); });
		    evt.menu.AppendAction("Expand All", (a) => { SetCollapsedCallback(false); });
		}
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(nap =>
            nap.direction != startPort.direction &&
            nap.node != startPort.node)
            .ToList();
    }

    private void SetCollapsedCallback(bool isCollapsed)
    {
	    foreach (var element in graphElements.ToList())
	    {
		    if (element is StoryNode storyNode)
		    {
			    storyNode.SetCollapsedGlobal(isCollapsed);
		    }
	    }
    }
}