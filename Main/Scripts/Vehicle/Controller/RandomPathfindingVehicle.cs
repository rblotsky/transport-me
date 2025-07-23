using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.Scripts.Route;

namespace Transportme.Main.Scripts.Vehicle
{
    public partial class RandomPathfindingVehicle : VehicleController
    {
        public override void _EnterTree()
        {
            OnRouteEnd += StartRandomRoute;
            base._EnterTree();
        }

        private void StartRandomRoute(Vehicle vehicle)
        {
            // Does nothing if graph isn't ready
            if ((bool)(vehicle?.graph?.isGraphReady))
            {
                // Randomly chooses two checkpoints from the graph
                RandomNumberGenerator rng = new RandomNumberGenerator();
                rng.Randomize();

                NavCheckpoint[] endpoints = vehicle.graph.GetTwoRandomCheckpoints(rng);
                if (endpoints.Length != 2)
                {
                    throw new Exception("booo");
                }
                NavCheckpoint start = endpoints[0];
                NavCheckpoint end = endpoints[1];
                if(start.AssociatedConnection == null || end.AssociatedConnection == null)
                {
                    GD.PrintErr("RandomVehicleController > Generated checkpoints have no associated connection!");
                }
                SingleRoute route = SingleRoute.Pathfind(start.AssociatedConnection, end.AssociatedConnection);
                vehicle.Speed = 0f;
                vehicle.Route = route;
            }
        }
    }
}
