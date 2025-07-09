using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.DevTools;

namespace Transportme.Main.Scripts.Pathfinding
{
    public interface IRouteMovementIterator : IDebugVisualizationProvider
    {
        bool Move(double distance);
        RoutePoint GetPositionOnRoute(VehicleProperties properties, float delta);
        NavSegment GetCurrentSegment();
        bool IsFinishedRoute();
    }
}
