using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Pathfinding
{
    public delegate void RouteCompleteHandler(Vehicle invokingVehicle);
    public partial class VehicleController : Node3D
    {
        public event RouteCompleteHandler OnRouteEnd;

        public void NotifyRouteComplete(Vehicle vehicle)
        {
            OnRouteEnd?.Invoke(vehicle);
        }
    }
}
