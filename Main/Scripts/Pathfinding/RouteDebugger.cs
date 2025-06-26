using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.RouteGeneration;

[GlobalClass]
public partial class RouteDebugger : Node, IDebugVisualizationProvider
{
    private AStar routeTest;
    public static Color ValueToRedGradient(float value, float maxValue)
    {
        // Clamp value between 0 and maxValue
        value = Mathf.Clamp(value, 0f, maxValue);

        float t = value / maxValue;

        // Green and blue go from 1.0 (white) to 0.0 (red)
        float gb = 1.0f - t;

        return new Color(1.0f, gb, gb); // RGB values in range [0.0, 1.0]
    }
    public IEnumerable<DebugVisualization> GetVisualization()
    {
        if (routeTest == null) {
            yield break;
        }
        var thing = routeTest.GetComputedPoints();
        thing.Sort((a, b) => a.cost.CompareTo(b.cost));
        float max = thing.LastOrDefault().cost;
        foreach (AStarCosts costs in routeTest.GetComputedPoints())
        {
            Color c = ValueToRedGradient(costs.cost, max);
            yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.NavSegments], costs.connection.Position, 0.5f, c);
        }

        foreach (NavSegment path in routeTest.GetPath())
        {
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.NavSegments], path.GlobalStart, path.GlobalEnd, Colors.Black);
        }
    }

    public override void _Ready()
    {
        routeTest = null;
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

                NavConnection origin = graph.GetConnectionAtPosition(checkpoints[0].GlobalPosition);
                NavConnection destination = graph.GetConnectionAtPosition(checkpoints[1].GlobalPosition);

                AStar aStar = new AStar();
                aStar.Compute(origin, destination);
                routeTest = aStar;
                return;

                //Route createdRoute = Route.CreateRouteDjikstras(origin.GlobalSnappedPos, destination.GlobalSnappedPos, graph);

                //// If no route available, try going the other way (probably because of directions lol)
                //if (createdRoute == null)
                //{
                //    createdRoute = Route.CreateRouteDjikstras(destination.GlobalSnappedPos, origin.GlobalSnappedPos, graph);
                //}

                //// Only draw the line if we found a route either direction
                //if (createdRoute != null)
                //{
                //    Debugger3D.main.RouteEffectDefault(createdRoute, 5);
                //}

                //// If no route, we show in red.
                //else
                //{
                //    Debugger3D.main.SphereEffect(origin.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                //    Debugger3D.main.SphereEffect(destination.GlobalSnappedPos, 0.7f, Colors.Red, 1, 5);
                //}
                
            }

        }

        base._UnhandledInput(receivedEvent);
    }
}
