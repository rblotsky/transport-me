using Godot;
using System;
using System.Collections.Generic;

public partial class HardStopCollider : VehicleCollider
{
	public override void UpdateVisualization()
	{
		base.UpdateVisualization();
		visualization.Scale = ((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size;
	}
	private float GetComputedPositionOnRoute()
	{
		float distanceAlongRoute = associatedVehicle.CurrentDistanceAlongRoute;
		float speed = (float)associatedVehicle.CurrentSpeed;
		float timeToStop = speed / (float)associatedVehicle.brakeSpeed;
		float brakingDistanceOnRoute = (float)distanceAlongRoute + (float)(speed * timeToStop) - 0.25f * (float)associatedVehicle.brakeSpeed * timeToStop * timeToStop;
		return brakingDistanceOnRoute;
	}
	public override void HandleUpdatePosition()
	{
		Route route = associatedVehicle.CurrentRoute;
		
		float brakingDistanceOnRoute = Mathf.Min(GetComputedPositionOnRoute(), route.GetLength());
		RoutePoint point = route.GetVehicleRoutePositionAtPoint(brakingDistanceOnRoute);
		FaceDirectionOfMotion(point.Rotation);
		GlobalPosition = point.Position;
	}

	protected override bool ShouldStop(List<VehicleCollider> colliders)
	{
		return colliders.Count > 0;
	}
}
