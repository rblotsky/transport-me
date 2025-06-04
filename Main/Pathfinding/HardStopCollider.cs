using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;

public partial class HardStopCollider : VehicleCollider, IDebugVisualizationProvider
{
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
    private RoutePoint getPositionOnRoute()
	{
		Route route = associatedVehicle.CurrentRoute;
		
		float brakingDistanceOnRoute = Mathf.Min(GetComputedPositionOnRoute(), route.GetLength());
		return route.GetVehicleRoutePositionAtPoint(brakingDistanceOnRoute);
	}

	protected override bool ShouldStop(List<VehicleCollider> colliders)
	{
		return colliders.Count > 0;
	}

    public IEnumerable<DebugVisualization> GetVisualization()
    {
		yield return DebugVisualizationFactory.Box(
			[DebugVisualizationFilters.VehicleCollisions],
			GlobalPosition,
			Quaternion,
			((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size,
			Colors.Black);
		RoutePoint point = getPositionOnRoute();
		yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.Black);
		yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], GlobalPosition, 0.1f, Colors.Black);
    }
}
