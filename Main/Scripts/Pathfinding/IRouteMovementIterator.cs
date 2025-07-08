using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Pathfinding
{
    public interface IRouteMovementIterator
    {
        RoutePoint Move(double distance);
        RoutePoint GetPositionOnRoute(VehicleProperties properties, float delta);
        NavSegment GetCurrentSegment();
        bool IsFinishedRoute();
    }
}
