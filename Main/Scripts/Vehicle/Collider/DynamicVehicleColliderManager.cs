using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Route;

namespace Transportme.Main.Scripts.Vehicle.Collider
{
    [GlobalClass]
    public partial class DynamicVehicleColliderManager : VehicleCollider
    {
        protected List<DynamicVehicleCollider> _vehicleColliders;
        protected int activeColliders = 0;

        public override void _EnterTree()
        {
            _vehicleColliders = new List<DynamicVehicleCollider>();
            base._EnterTree();
        }

        private float GetHalfBrakingDistance()
        {
            float speed = (float)associatedVehicle.Speed;
            float timeToStop = (2 * speed) / associatedVehicle.VehicleProperties.brakingPower;
            return ((float)(speed * timeToStop) - (0.25f / 2) * (float)associatedVehicle.VehicleProperties.brakingPower * timeToStop * timeToStop);
        }
        private float GetBrakingDistance()
        {
            float speed = (float)associatedVehicle.Speed;
            float timeToStop = speed / associatedVehicle.VehicleProperties.brakingPower;
            return (float)(speed * timeToStop) - 0.25f * (float)associatedVehicle.VehicleProperties.brakingPower * timeToStop * timeToStop;
        }
        public override void HandleUpdatePosition(IRouteMovementIterator route)
        {
            int segments = (int)Math.Ceiling(GetHalfBrakingDistance() - GetBrakingDistance() / associatedVehicle.VehicleProperties.chassisLength);
            if (segments > activeColliders)
            {
                GD.Print("adding, ", activeColliders, " to ", segments);
                int startingIndex = activeColliders; // add 1
                for (int i = startingIndex; i < segments; i++)
                {
                    if(_vehicleColliders.Count > i)
                    {
                        _vehicleColliders[i].Visible = true;
                        _vehicleColliders[i].SetProcess(true);
                        continue;
                    }
                    DynamicVehicleCollider collider = GD.Load<PackedScene>("res://Main/Resources/Vehicles/dynamic_vehicle_collider.tscn").Instantiate<DynamicVehicleCollider>();
                    associatedVehicle.AddChild(collider);
                    _vehicleColliders.Add(collider);
                    collider.AssociatedVehicle = associatedVehicle;
                    collider.Initialize((i + 1) * associatedVehicle.VehicleProperties.chassisLength);
                    Simplifications.GetFirstChildOfType<DebugVisualizer>(GetNode("/root/"), true).RegisterProvider(collider);
                }
            } else if (segments < activeColliders)
            {
                GD.Print("removing, ", activeColliders, " to ", segments);
                for (int i = activeColliders - 1; i >= segments; i--)
                {
                    _vehicleColliders[i].Visible = false;
                    _vehicleColliders[i].SetProcess(false);
                }
            }
            activeColliders = segments;
            for (int i = 0; i < activeColliders; i++)
            {
                _vehicleColliders[i].HandleUpdatePosition(route);
            }
        }

        protected override bool ShouldStop(List<VehicleCollider> colliders)
        {
            return false;
        }
    }
}
