using Godot;
using System;
using Transportme.Main.Scripts.RouteGeneration;

[GlobalClass]
public partial class RandomVehicle : Vehicle
{
	// FUNCTIONS //
	public override void _PhysicsProcess(double delta)
	{
		// Only moves if we haven't reached the end yet.
		if (Route != null && !Route.IsFinishedRoute() && timeStopped < 10)
		{
			RunMovementIteration(delta);
		}
		else
		{
			StartRandomRoute();
			Speed = 0f;
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
			if (endpoints.Length != 2) {
				throw new Exception("booo");
			}
			RouteV2 route = new RouteV2();
			route.InitializeRoute(graph.GetConnectionAtPosition(endpoints[0].GlobalPosition), graph.GetConnectionAtPosition(endpoints[1].GlobalPosition));
			Route = route;
		}
	}
}
