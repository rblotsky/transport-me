using Godot;
using System;
using System.Collections.Generic;

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
		//DeleteVisualization();
		//visualization = EasyShapes.AddShapeMesh(this, EasyShapes.Box(0.1f));
		//visualization.GlobalPosition = GlobalPosition;
		base.UpdateVisualization();
		visualization.Scale = new Vector3(1, 1, 2);
	}

	public override void HandleUpdatePosition()
	{
		Route route = associatedVehicle.GetRoute();
		float distanceAlongRoute = associatedVehicle.GetDistanceAlongRoute();
		RoutePoint point = route.GetVehicleRoutePositionAtPoint(distanceAlongRoute);

		FaceDirectionOfMotion(point.Rotation);
		GlobalPosition = point.Position;
	}
	protected override bool ShouldStop(List<VehicleCollider> colliders)
	{
		return false;
	}
}
