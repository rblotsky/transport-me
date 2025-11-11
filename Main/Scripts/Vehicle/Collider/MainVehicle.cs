using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Route;
namespace Transportme.Main.Scripts.Vehicle
{
    public partial class MainVehicle : VehicleCollider, IDebugVisualizationProvider
    {
	    public override void HandleUpdatePosition(IRouteMovementIterator route)
	    {
		    RoutePoint point = route.GetPositionOnRoute(associatedVehicle.VehicleProperties, 0);

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
                [DebugVisualizationFilters.VehicleCollisions],
                GlobalPosition,
                Quaternion,
                new Vector3(1, 1, 2),
                Colors.Black);
            yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], GlobalPosition, 0.1f, Colors.Black);
            if(associatedVehicle.Route == null)
            {
                yield break;
            }
            RoutePoint point = associatedVehicle.Route.GetPositionOnRoute(associatedVehicle.VehicleProperties, 0);
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.Black);
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.Position, point.Position + Transform.Basis.X, Colors.Aqua);
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.Position, point.Position + Transform.Basis.Y, Colors.Green);
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.Position, point.Position + Transform.Basis.Z, Colors.Yellow);


        }
    }
}