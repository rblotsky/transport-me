using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Pathfinding
{
    internal interface IRouteMovementIterator
    {
        RoutePoint Move(float distance);
        RoutePoint GetPositionOnRoute(float delta);
        NavSegment GetCurrentSegment();
    }
}
