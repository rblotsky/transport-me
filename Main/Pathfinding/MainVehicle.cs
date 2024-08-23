using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.Pathfinding;

public partial class MainVehicle : VehicleCollider
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
		visualization.Scale = new Vector3(1, 1, 2);
	}

	public override void HandleUpdatePosition()
	{
		Route route = associatedVehicle.GetRoute();
		float distanceAlongRoute = associatedVehicle.GetDistanceAlongRoute();
		RoutePoint point = route.GetVehiclePropsOnRoute(distanceAlongRoute);
		if (false) //previous implementation
		{
			Vector3 rotationDirection = route.GetDirectionOnRoute(distanceAlongRoute);
			Vector3 newPosition = route.GetPositionAlongRoute(distanceAlongRoute);
			FaceDirectionOfMotion(rotationDirection);
			GlobalPosition = newPosition;
			return;
		}

		FaceDirectionOfMotion(point.Rotation);
		GlobalPosition = point.Position;
	}
	protected override bool ShouldStop(List<VehicleCollider> colliders)
	{
		return false;
	}
}
