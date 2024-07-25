using Godot;
using System;

[GlobalClass]
public partial class RouteDebugger : Node
{
    // FUNCTIONS //
    // Godot Defaults
    public override void _Ready()
    {
        InstructionsUI.instance.AddInstruction(this, "Press P to do a random Route test!");
        base._Ready();
    }

    public override void _UnhandledInput(InputEvent receivedEvent)
    {
        if (receivedEvent is InputEventKey keyboardInput)
        {
            if (keyboardInput.Keycode == Key.P && keyboardInput.IsPressed())
            {

                GD.Print("Lol its pressed");

                // Find two random nodes and make a route between them
                NavGraphContainer graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetParent(), true);
                RandomNumberGenerator rng = new RandomNumberGenerator();
                NavCheckpoint[] checkpoints = graph.GetTwoRandomCheckpoints(rng);
                if (checkpoints.Length != 2)
                {
                    GD.PrintErr("There are less than 2 checkpoints in existence!");
                    return;
                }

                NavCheckpoint origin = checkpoints[0];
                NavCheckpoint destination = checkpoints[1];

                Route createdRoute = Route.CreateRouteBFS(origin.GlobalSnappedPos, destination.GlobalSnappedPos, graph);

                // If no route available, try going the other way (probably because of directions lol)
                if (createdRoute == null)
                {
                    createdRoute = Route.CreateRouteBFS(destination.GlobalSnappedPos, origin.GlobalSnappedPos, graph);
                }

                // Only draw the line if we found a route either direction
                if (createdRoute != null)
                {
                    Debugger3D.main.RouteEffectDefault(createdRoute, 5);
                }

                // If no route, we show in red.
                else
                {
                    Debugger3D.main.SphereEffect(origin.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                    Debugger3D.main.SphereEffect(destination.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                }
                
            }

        }

        base._UnhandledInput(receivedEvent);
    }
}
