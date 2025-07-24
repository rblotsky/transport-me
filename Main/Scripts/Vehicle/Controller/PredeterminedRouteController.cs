using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.Scripts.Route;

namespace Transportme.Main.Scripts.Vehicle
{
    [GlobalClass]
    public partial class PredeterminedRouteController : VehicleController
    {
        [Export] protected NavCheckpoint Start { get; set; }
        [Export] protected NavCheckpoint End { get; set; }

        public override void _EnterTree()
        {
            OnRouteEnd += GenerateRoute;
            base._EnterTree();
        }

        private void GenerateRoute(Vehicle vehicle)
        {
            if (!(bool)(vehicle?.graph?.isGraphReady)) {
                return;
            }
            vehicle.Speed = 0;
            vehicle.Route = SingleRoute.Pathfind(Start.AssociatedConnection, End.AssociatedConnection);
        }
    }
}
