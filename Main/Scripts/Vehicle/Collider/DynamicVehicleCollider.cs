using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Route;

namespace Transportme.Main.Scripts.Vehicle.Collider
{
    public partial class DynamicVehicleCollider : VehicleCollider, IDebugVisualizationProvider
    {
        public float RelativeDistance; 
        public double GetExpectedAcceleration(VehicleCollider collider)
        {
            Vector2 vehicleBasis = Simplifications.GetVectorXZ(collider.Basis.Z) * ((float)collider.AssociatedVehicle.Speed);
            float relativeVelocity = vehicleBasis.Project(Simplifications.GetVectorXZ(Transform.Basis.Z)).Length();

            double acceleration = Math.Pow(associatedVehicle.Speed - collider.AssociatedVehicle.Speed, 2) / Scale.X;
            return acceleration;
        }

        public void Initialize(float relativeDistance)
        {
            RelativeDistance = relativeDistance;
        }

        public IEnumerable<DebugVisualization> GetVisualization()
        {
            if(Visible == false)
            {
                yield break;
            }
            yield return DebugVisualizationFactory.Box(
                [DebugVisualizationFilters.VehicleCollisions],
                GlobalPosition,
                Quaternion,
                new Vector3(1, 1, 2),
                Colors.Black);
            yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], GlobalPosition, 0.1f, Colors.Black);
            if (associatedVehicle.Route == null)
            {
                yield break;
            }
            RoutePoint point = associatedVehicle.Route.GetPositionOnRoute(associatedVehicle.VehicleProperties, 0);
            yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.Black);
        }
        private float GetBrakingDistance()
        {
            float speed = (float)associatedVehicle.Speed;
            float timeToStop = speed / associatedVehicle.VehicleProperties.brakingPower;
            return (float)(speed * timeToStop) - 0.25f * (float)associatedVehicle.VehicleProperties.brakingPower * timeToStop * timeToStop;
        }
        public override void HandleUpdatePosition(IRouteMovementIterator route)
        {
            float brakingDistance = GetBrakingDistance() + associatedVehicle.VehicleProperties.chassisLength / 2 + RelativeDistance;
            RoutePoint point = route.GetPositionOnRoute(associatedVehicle.VehicleProperties, brakingDistance);
            FaceDirectionOfMotion(point.Rotation);
            GlobalPosition = point.Position;
        }

        protected override bool ShouldStop(List<VehicleCollider> colliders)
        {
            return false;
        }
    }
}
