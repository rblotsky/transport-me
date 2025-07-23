using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Vehicle;

namespace Transportme.Main.Scripts.Route
{
    /// <summary>
    /// First iteration of the new route system. This simply provides all of the code needed to move a vehicle along a route
    /// </summary>
    public partial class SingleRoute : RefCounted, IRouteMovementIterator
    {
        private RouteResult _routeResult;
        private List<NavSegment> route;
        private int _currentSegmentIndex;
        private double _currentDistanceAlongSegment;
        private double _distanceAlongRoute;
        private double? _length = null;
        public double Length { get { return _length ?? ComputeLength(); } }

        public static SingleRoute Pathfind(NavConnection start, NavConnection end)
        {
            SingleRoute singleRoute = new SingleRoute(start, end);
            return singleRoute;
        }
        public SingleRoute(NavConnection start, NavConnection end)
        {
            _routeResult = AStar.Compute(start, end);
            route = _routeResult.GetPath().ToList();
        }

        private double ComputeLength()
        {
            double length = 0f;
            foreach (NavSegment segment in route) {
                length += segment.Length;
            }
            _length = length;
            return length;
        }
        
        //how to communicate it was cut off? - maybe include the ability to overflow via extending points
        private Vector3 GetPointAlongRoute(float relativeToPosition, bool exterpolate)
        {
            double currentDistance = _currentDistanceAlongSegment + relativeToPosition;
            int trackedIndex = _currentSegmentIndex;
            // backwards
            if (currentDistance < 0) {
                while(trackedIndex > 0 && currentDistance < 0)
                {
                    trackedIndex--;
                    currentDistance += route[trackedIndex].Length;
                }
            } else //forwards
            {
                while(trackedIndex < route.Count && currentDistance > route[trackedIndex].Length)
                {
                    currentDistance -= route[trackedIndex].Length;
                    trackedIndex++;
                }
            }

            // before the start
            if (currentDistance < 0)
            {
                if (!exterpolate)
                {
                    return route[0].GlobalStart;
                }
                Vector3 direction = (route[trackedIndex].GetPositionOnSegment(0.01f) - route[trackedIndex].GlobalStart).Normalized();
                return route[trackedIndex].GlobalStart + ((float)currentDistance * direction);
            }
            else if (trackedIndex == route.Count) //after the end
            {
                if (!exterpolate)
                {
                    return route.Last().GlobalEnd;
                }
                Vector3 direction = (route.Last().GlobalEnd - route.Last().GetPositionOnSegment(0.99f)).Normalized();
                return route.Last().GlobalEnd + ((float)currentDistance * direction);
            }

            //actually found a segment
            return route[trackedIndex].GetPositionOnSegmentAbsolute((float)currentDistance);
        }
        
        public bool Move(double distance)
        {
            _distanceAlongRoute += distance;
            _currentDistanceAlongSegment += distance;
            while(_currentSegmentIndex < route.Count && _currentDistanceAlongSegment > route[_currentSegmentIndex].Length )
            {
                _currentDistanceAlongSegment -= route[_currentSegmentIndex].Length;
                _currentSegmentIndex += 1;
            }
            return _distanceAlongRoute > Length;
        }

        public RoutePoint GetPositionOnRoute(VehicleProperties properties, float delta)
        {
            return GetVehicleRoutePositionAtPoint(delta);
        }

        public NavSegment GetCurrentSegment()
        {
            return route[_currentSegmentIndex];
        }

        public bool IsFinishedRoute()
        {
            return _distanceAlongRoute >= Length || _currentSegmentIndex >= route.Count;
        }

        public RoutePoint GetVehicleRoutePositionAtPoint(float relativeDistanceOnRoute)
        {
            RoutePoint routePoint = new();
            float backDistance = 0.6f;
            float frontDistance = 0.4f;

            double boundedPosition = Math.Max(
                Math.Min(_distanceAlongRoute + relativeDistanceOnRoute, Length),
                0f);
            double finalRelativeDistanceOnRoute = boundedPosition - _distanceAlongRoute;

            Vector3 from = GetPointAlongRoute((float)finalRelativeDistanceOnRoute - 0.6f, true);
            Vector3 to = GetPointAlongRoute((float)finalRelativeDistanceOnRoute + 0.4f, true);
            float lerpValue;
            //if (distanceFrom == 0f)
            //{
            //    lerpValue = distanceAlongRoute / distanceTo;
            //}
            //else if (distanceTo == (float)length)
            //{
            //    lerpValue = 1f - Mathf.Min((distanceTo - distanceAlongRoute) / (distanceTo - distanceFrom), 1f);
            //    //GD.Print(lerpValue);
            //}
            //else
            //{
            //    lerpValue = backDistance / (backDistance + frontDistance);
            //}

            routePoint.Position = from.Lerp(to, 0.5f);
            routePoint.Rotation = (to - from).Normalized();
            routePoint.backPoint = from;
            routePoint.forwardPoint = to;
            return routePoint;
        }

        public IEnumerable<DebugVisualization> GetVisualization()
        {
            var thing = route.ElementAtOrDefault(_currentSegmentIndex)?.GetPositionOnSegmentAbsolute((float)_currentDistanceAlongSegment);
            if (thing.HasValue) { 
                yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.NavSegments], thing.Value, 1f, Colors.Red);
            }
            int alt = 0;
            foreach(NavSegment segment in route)
            {
                yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.NavSegments], segment.GlobalStart, segment.GlobalEnd, Colors.White);
                float i = 0.1f;
                while (i < segment.Length)
                {
                    float oijasdfj = 0.05f;
                    Color funny = Colors.White;
                    if(alt % 2 == 0)
                    {
                        funny = Colors.LimeGreen;
                    }
                    if(i == 0.1f)
                    {
                        funny = Colors.Red;
                        oijasdfj = 0.1f;
                    }
                    yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.NavSegments], segment.GetPositionOnSegmentAbsolute(i), oijasdfj, funny);
                    i += 0.2f;
                }
                alt += 1;
            }
        }
    }
}
