using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.Scripts.Pathfinding;

namespace Transportme.Main.Scripts.RouteGeneration
{
    //this just tracks one thingy
    public partial class RouteV2 : RefCounted, IRouteMovementIterator
    {
        private List<NavSegment> route;
        private int _index;
        private float _currentDistanceAlongSegment;
        private float _distanceAlongRoute;
        private float? _length = null;
        public float Length { get { !_length.HasValue ? computeLength() : _length.Value } }

        private float computeLength()
        {
            float length = 0f;
            foreach (NavSegment segment in route) { 
                length += segment.Length;
            }
            _length = length;
            return length;
        
        //how to communicate it was cut off? - maybe include the ability to overflow via extending points
        private Vector3 GetPointAlongRoute(float relativeToPosition, bool exterpolate)
        {
            float currentDistance = _currentDistanceAlongSegment + relativeToPosition;
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
                return route[trackedIndex].GlobalStart + (currentDistance * direction);
            }
            else if (trackedIndex == route.Count)
            {
                if (!exterpolate)
                {
                    return route[trackedIndex].GlobalEnd;
                }
                Vector3 direction = (route.Last().GlobalEnd - route.Last().GlobalStart).Normalized();
                return route.Last().GlobalEnd + (currentDistance * direction);
            }
            return route[trackedIndex].GetPositionOnSegment(currentDistance);
        }
        
        public RoutePoint Move(float distance)
        {
            
            throw new NotImplementedException();
        }

        public RoutePoint GetPositionOnRoute(float delta)
        {
            throw new NotImplementedException();
        }

        public NavSegment GetCurrentSegment()
        {
            throw new NotImplementedException();
        }


        public RoutePoint GetVehicleRoutePositionAtPoint(float relativeDistanceOnRoute)
        {
            RoutePoint routePoint = new();
            float backDistance = 0.6f;
            float frontDistance = 0.4f;
            float actualPosition = Math.Max(
                Math.Min(_distanceAlongRoute + relativeDistanceOnRoute, Length),
                0fs);
            float finalRelativeDistanceOnRoute = actualPosition - relativeDistanceOnRoute;

            Vector3 from = GetPointAlongRoute(finalRelativeDistanceOnRoute - 0.6f, true);
            Vector3 to = GetPointAlongRoute(finalRelativeDistanceOnRoute + 0.4f, true);
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

            routePoint.Position = from.Lerp(to, lerpValue);
            routePoint.Rotation = (to - from).Normalized();
            routePoint.backPoint = from;
            routePoint.forwardPoint = to;
            return routePoint;
        }
    }
}
