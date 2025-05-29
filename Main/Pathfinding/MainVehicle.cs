using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;

public partial class MainVehicle : VehicleCollider, IDebugVisualizationProvider
{
	public override void HandleUpdatePosition()
	{
        Route route = associatedVehicle.CurrentRoute;
		float distanceAlongRoute = associatedVehicle.CurrentDistanceAlongRoute;
		RoutePoint point = route.GetVehicleRoutePositionAtPoint(distanceAlongRoute);

		FaceDirectionOfMotion(point.Rotation);
		GlobalPosition = point.Position;
	}
	protected override bool ShouldStop(List<VehicleCollider> colliders)
	{
		return false;
	}

    public IEnumerable<DebugVisualization> GetVisualization()
    {
        yield return DebugVisualizationFactory.Box(
            GlobalPosition,
            Quaternion,
            new Vector3(1, 1, 2),
            Colors.Black);
		RoutePoint point = associatedVehicle.CurrentRoute.GetVehicleRoutePositionAtPoint(associatedVehicle.CurrentDistanceAlongRoute);
        yield return DebugVisualizationFactory.Line(point.forwardPoint, point.backPoint, Colors.Black);
        yield return DebugVisualizationFactory.Sphere(GlobalPosition, 0.1f, Colors.Black);
    }
}
