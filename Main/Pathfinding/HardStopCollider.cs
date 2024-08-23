using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.Pathfinding;

public partial class HardStopCollider : VehicleCollider
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void UpdateVisualization()
	{
		base.UpdateVisualization();
		visualization.Scale = ((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size;
	}

	public override void HandleUpdatePosition()
    {
		Route route = associatedVehicle.GetRoute();
		float distanceAlongRoute = associatedVehicle.GetDistanceAlongRoute();
		float speed = (float)associatedVehicle.GetCurrentSpeed();
		float timeToStop = speed / (float)associatedVehicle.brakeSpeed;
		float brakingDistanceOnRoute = (float)distanceAlongRoute + (float)(speed * timeToStop) - 0.25f * (float)associatedVehicle.brakeSpeed * timeToStop * timeToStop;
		
		brakingDistanceOnRoute = Mathf.Min(brakingDistanceOnRoute, route.GetLength());
		RoutePoint point = route.GetVehiclePropsOnRoute(brakingDistanceOnRoute);
		FaceDirectionOfMotion(point.Rotation);
		GlobalPosition = point.Position;
	}

    protected override bool ShouldStop(List<VehicleCollider> colliders)
    {
        return (bool)(colliders.Count > 0);
    }
}
