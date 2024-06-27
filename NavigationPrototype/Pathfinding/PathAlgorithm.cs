using Godot;
using System;

[GlobalClass]
public partial class PathAlgorithm : Node
{
    // Godot Defaults
    public override void _Input(InputEvent receivedEvent)
    {
        if (receivedEvent is InputEventKey keyboardInput)
        {
            if (keyboardInput.Keycode == Key.P && keyboardInput.IsPressed())
            {
                GD.Print("Lol its pressed");

                // Find two random nodes and make a route between them
                NavGraph graph = Simplifications.GetFirstChildOfType<NavGraph>(GetParent(), true);
                RandomNumberGenerator rng = new RandomNumberGenerator();
                NavNode origin = graph.nodes[rng.RandiRange(0, graph.nodes.Count - 1)];
                NavNode destination = graph.nodes[rng.RandiRange(0, graph.nodes.Count - 1)];
                Route createdRoute = Route.CreateRouteBFS(origin, destination);

                Debugger3D.instance.SphereEffect(origin.GlobalPosition, 1, Colors.RebeccaPurple, 1, 10);
                Debugger3D.instance.SphereEffect(destination.GlobalPosition, 1, Colors.RebeccaPurple, 1, 10);
                foreach(NavSegment segment in createdRoute.Segments)
                {
                    Debugger3D.instance.LineEffect(segment.Start.GlobalPosition+Vector3.Up, segment.End.GlobalPosition + Vector3.Up, Colors.LimeGreen, 10);
                }
            }
        }
    }
}
