using Godot;
using System;

[GlobalClass]
public partial class RandomVehicle : Vehicle
{
    // FUNCTIONS //
    public override void _PhysicsProcess(double delta)
    {
        // Only moves if we haven't reached the end yet.
        if (CurrentSegment != null)
        {
            RunMovementIteration(delta);
        }
        else
        {
            StartRandomRoute();
        }

        base._PhysicsProcess(delta);
    }


    private void StartRandomRoute()
    {
        // Does nothing if graph isn't ready
        if (graph.isGraphReady)
        {
            // Randomly chooses two checkpoints from the graph
            RandomNumberGenerator rng = new RandomNumberGenerator();
            rng.Randomize();

            NavCheckpoint[] endpoints = graph.GetTwoRandomCheckpoints(rng);

            if (endpoints.Length == 0)
            {
                GD.PrintErr("Failed to find two valid endpoints for a random path!");
            }
            else
            {
                StartRoute(Route.CreateRouteBFS(endpoints[0].GlobalSnappedPos, endpoints[1].GlobalSnappedPos, graph));
            }
        }
    }
}
