using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Route;
namespace Transportme.Main.Scripts.Vehicle
{
    [GlobalClass]

    public partial class HardStopCollider : VehicleCollider, IDebugVisualizationProvider
	{
		private float GetBrakingDistance()
		{
			float speed = (float)associatedVehicle.Speed;
			float timeToStop = speed / associatedVehicle.VehicleProperties.brakingPower;
			return (float)(speed * timeToStop) - 0.25f * (float)associatedVehicle.VehicleProperties.brakingPower * timeToStop * timeToStop;
		}

		public override void HandleUpdatePosition(IRouteMovementIterator route)
		{
			float brakingDistance = GetBrakingDistance() + associatedVehicle.VehicleProperties.chassisLength / 2;
			RoutePoint point = route.GetPositionOnRoute(associatedVehicle.VehicleProperties, brakingDistance);
			FaceDirectionOfMotion(point.Rotation);
			GlobalPosition = point.Position;
		}

		protected override bool ShouldStop(List<VehicleCollider> colliders)
		{
			//GD.Print("Hard stop collider stopping: ", colliders.Count);
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
			yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], GlobalPosition, 0.1f, Colors.Black);
			if(associatedVehicle.Route != null)
			{
				RoutePoint point = associatedVehicle.Route.GetPositionOnRoute(AssociatedVehicle.VehicleProperties, GetBrakingDistance());
				yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.Black);
			}
		}
	}
}