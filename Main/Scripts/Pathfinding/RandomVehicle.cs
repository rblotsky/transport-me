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
		//if (Route != null && !Route.IsFinishedRoute() && timeStopped < 10)
		//{
		//	RunMovementIteration(delta);
		//}
		//else
		//{
		//	StartRandomRoute();
		//	Speed = 0f;
		//	timeStopped = 0;
		//}
		RunMovementIteration(delta);

		base._PhysicsProcess(delta);
	}

    
}
