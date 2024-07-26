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

                GD.Print("Pressed the button!");

                // Find two random nodes and make a route between them
                NavGraphContainer graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetParent(), true);
                RandomNumberGenerator rng = new RandomNumberGenerator();
                NavCheckpoint[] checkpoints = graph.GetTwoRandomCheckpoints(rng);
                if (checkpoints.Length != 2)
                {
                    GD.PrintErr("There are less than 2 checkpoints in existence!");
                    return;
                }

                GD.Print("Found two checkpoints!");

                NavCheckpoint origin = checkpoints[0];
                NavCheckpoint destination = checkpoints[1];

                Route createdRoute = Route.CreateRouteBFS(origin.GlobalSnappedPos, destination.GlobalSnappedPos, graph);

                GD.Print("Created a route!");
                // If no route available, try going the other way (probably because of directions lol)
                if (createdRoute == null)
                {
                    GD.Print("The route was empty. Creating a new one!");
                    createdRoute = Route.CreateRouteBFS(destination.GlobalSnappedPos, origin.GlobalSnappedPos, graph);
                    GD.Print("Created the new route!");
                }

                // Only draw the line if we found a route either direction
                if (createdRoute != null)
                {
                    GD.Print("The first or second route are not null! Displaying the route.");
                    Debugger3D.main.RouteEffectDefault(createdRoute, 5);
                    GD.Print("Displayed the route!");
                }

                // If no route, we show in red.
                else
                {
                    GD.Print("Both routes are null! Now showing the failure.");
                    Debugger3D.main.SphereEffect(origin.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                    Debugger3D.main.SphereEffect(destination.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                }
                
            }

        }

        base._UnhandledInput(receivedEvent);
    }
}
