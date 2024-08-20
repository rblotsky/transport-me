using Godot;
using System;

[GlobalClass]
public partial class RandomVehicleRefactored : VehicleRefactored
{
	// FUNCTIONS //
	public override void _PhysicsProcess(double delta)
	{
		// Only moves if we haven't reached the end yet.
		if (CurrentSegment != null && timeStopped < 5)
		{
			RunMovementIteration(delta);
		}
		else
		{
			StartRandomRoute();
			timeStopped = 0;
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
				StartRoute(Route.CreateRouteDjikstras(endpoints[0].GlobalPosition, endpoints[1].GlobalPosition, graph));
			}
		}
	}
}
