using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.Scripts.Pathfinding;

namespace Transportme.Main.Scripts.RouteGeneration
{
    /// <summary>
    /// First iteration of the new route system. This simply provides all of the code needed to move a vehicle along a route
    /// </summary>
    public partial class RouteV2 : RefCounted, IRouteMovementIterator
    {
        private RouteResult _routeResult;
        private List<NavSegment> route;
        private int _index;
        private double _currentDistanceAlongSegment;
        private double _distanceAlongRoute;
        private double? _length = null;
        public double Length { get { return _length ?? ComputeLength(); } }

        public void InitializeRoute(NavConnection start, NavConnection end)
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
            int trackedIndex = _index;
            if (currentDistance < 0) {
                while(trackedIndex > 0 && currentDistance > 0)
                {
                    trackedIndex--;
                    currentDistance += route[trackedIndex].Length;
                }
            } else
            {
                while(trackedIndex < route.Count && currentDistance > route[trackedIndex].Length)
                {
                    currentDistance -= route[trackedIndex].Length;
                    trackedIndex++;
                }
            }
            if (currentDistance < 0)
            {
                if (!exterpolate)
                {
                    return route[trackedIndex].GlobalStart;
                }
                Vector3 direction = (route[trackedIndex].GlobalEnd - route[trackedIndex].GlobalStart).Normalized();
                return route[trackedIndex].GlobalStart + ((float)currentDistance * direction);
            }
            else if (trackedIndex == route.Count)
            {
                if (!exterpolate)
                {
                    return route[trackedIndex].GlobalEnd;
                }
                Vector3 direction = (route.Last().GlobalEnd - route.Last().GlobalStart).Normalized();
                return route.Last().GlobalEnd + ((float)currentDistance * direction);
            }
            return route[trackedIndex].GetPositionOnSegment((float)currentDistance);
        }
        
        public RoutePoint Move(double distance)
        {
            _distanceAlongRoute += distance;
            _currentDistanceAlongSegment += distance;
            if(_currentDistanceAlongSegment > route[_index].Length)
            {
                _index += 1;
                _currentDistanceAlongSegment -= route[_index].Length;
            }
            return GetVehicleRoutePositionAtPoint(0);
        }

        public RoutePoint GetPositionOnRoute(float delta)
        {
            return GetVehicleRoutePositionAtPoint(delta);
        }

        public NavSegment GetCurrentSegment()
        {
            return route[_index];
        }

        public bool IsFinishedRoute()
        {
            return _currentDistanceAlongSegment >= Length;
        }

        public RoutePoint GetVehicleRoutePositionAtPoint(float relativeDistanceOnRoute)
        {
            RoutePoint routePoint = new();
            float backDistance = 0.6f;
            float frontDistance = 0.4f;
            double actualPosition = Math.Max(
                Math.Min(_distanceAlongRoute + relativeDistanceOnRoute, Length),
                0f);
            double finalRelativeDistanceOnRoute = actualPosition - relativeDistanceOnRoute;

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
    }
}
