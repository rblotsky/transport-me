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
        /// <summary>
        /// Move the entity a specified distance along the route. Contains a return value to determine if the route has been completed after this move.
        /// </summary>
        /// <param name="distance">Distance to advance the entity along the route</param>
        /// <returns>Boolean - true if the route has been finished</returns>
        bool Move(double distance);
        /// <summary>
        /// Returns a <see cref="RoutePoint"/> object for a given point along the route. Position on the route is done relative to the current entitys' position.
        /// If position before or past the end of the route is requested, the point will be bounded to the start, and the simple start/end will be used to extrapolate the direction.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        RoutePoint GetPositionOnRoute(VehicleProperties properties, float delta);
        /// <summary>
        /// Get the current segment the entity is on
        /// </summary>
        /// <returns></returns>
        NavSegment GetCurrentSegment();
        /// <summary>
        /// Quick method to retrieve whether the route has been finished.
        /// </summary>
        /// <returns></returns>
        bool IsFinishedRoute();
    }
}
