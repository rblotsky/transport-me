using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Transportme.Main.Pathfinding;

public partial class Route : RefCounted
{
    // DATA //
    // Instance Data
    public NavSegment[] _orderedSegments;
    public NavSegment[] OrderedSegments { get { return _orderedSegments; } set { length = null; _orderedSegments = value; } }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }
    private float? length = null;

    //get the vector3 position along a route
    public Vector3 GetPositionAlongRoute(float distanceAlongRoute)
    {
        if (OrderedSegments == null) return Vector3.Zero;
        int segmentIndex = 0;
        if(distanceAlongRoute < 0)
        {
            return OrderedSegments[0].GlobalStart;
        }
        while (distanceAlongRoute > OrderedSegments[segmentIndex].Length)
        {
            distanceAlongRoute -= OrderedSegments[segmentIndex++].Length;
            if (OrderedSegments.Length == segmentIndex)
            {
                return OrderedSegments[segmentIndex - 1].GlobalEnd;
            }
        }
        float percentOfSegment = (float)distanceAlongRoute / OrderedSegments[segmentIndex].Length;
        return OrderedSegments[segmentIndex].GetPositionOnSegment(percentOfSegment);
    }

    public Vector3 GetDirectionOnRoute(float distanceAlongRoute)
    {
        if (OrderedSegments == null) return Vector3.Zero;

        float distanceFrom = Mathf.Max(distanceAlongRoute - 0.1f, 0f);
        float distanceTo = Mathf.Min(distanceAlongRoute + 0.1f, (float)length);
        Vector3 direction = GetPositionAlongRoute(distanceTo) - GetPositionAlongRoute(distanceFrom);
        return direction.Normalized();
        
        int segmentIndex = 0;
        while (distanceAlongRoute > OrderedSegments[segmentIndex].Length)
        {
            distanceAlongRoute -= OrderedSegments[segmentIndex++].Length;
            if (OrderedSegments.Length == segmentIndex)
            {
                return OrderedSegments[segmentIndex - 1].GetDirectionVectorOnSegment(1);
            }
        }
        float percentOfSegment = (float)distanceAlongRoute / OrderedSegments[segmentIndex].Length;
        return OrderedSegments[segmentIndex].GetDirectionVectorOnSegment(percentOfSegment);
    }

    public NavSegment GetSegmentAlongRoute(float distanceAlongRoute)
    {
        if (OrderedSegments == null) return null;
        int segmentIndex = 0;

        while (distanceAlongRoute > OrderedSegments[segmentIndex].Length)
        {
            distanceAlongRoute -= OrderedSegments[segmentIndex++].Length;
            if (OrderedSegments.Length == segmentIndex)
            {
                return null;
            }
        }
        return OrderedSegments[segmentIndex];
    }

    public RoutePoint GetVehiclePropsOnRoute(float distanceAlongRoute)
    {
        RoutePoint routePoint = new();
        float distanceFrom = Mathf.Max(distanceAlongRoute - 0.4f, 0f);
        float distanceTo = Mathf.Min(distanceAlongRoute + 0.2f, (float)length);
        Vector3 from = GetPositionAlongRoute(distanceFrom);
        Vector3 to = GetPositionAlongRoute(distanceTo);
        float lerpValue;
        if(distanceFrom == 0f)
        {
            lerpValue = distanceAlongRoute / distanceTo;
        } else if(distanceTo == (float)length)
        {
            lerpValue = 1 - ((distanceTo - distanceAlongRoute) / (distanceTo - distanceFrom));
            GD.Print(lerpValue);
        } else
        {
            lerpValue = 0.8f;
        }

        routePoint.Position = from.Lerp(to, lerpValue);
        routePoint.Rotation = (to - from).Normalized();
        return routePoint;
    }

    public float GetLength()
    {
        if(length == null)
        {
            length = 0f;
            foreach(NavSegment segment in OrderedSegments)
            {
                length += segment.Length;
            }
        }
        return (float)length;
    }

    // FUNCTIONS 
    // Constructing
    public static Route CreateRoute(IEnumerable<NavSegment> segmentsToTake, Vector3 start, Vector3 end)
    {
        Route newRoute = new Route();
        newRoute.OrderedSegments = segmentsToTake.ToArray();
        newRoute.StartPoint = start;
        newRoute.EndPoint = end;
        return newRoute;
    }

    public static Route CreateRouteDjikstras(Vector3 origin, Vector3 destination, NavGraphContainer graph)
    {
        if (origin == destination) return CreateRoute(new NavSegment[0], origin, destination);

        // Runs a Djikstras algorithm to create the Route

        Dictionary<NavConnection, float> dist = new Dictionary<NavConnection, float>(); //length from this nav connection to src
        Dictionary<NavConnection, NavSegment> prev = new Dictionary<NavConnection, NavSegment>(); //segment used (inbound)
        HashSet<NavConnection> visited = new HashSet<NavConnection>();

        NavConnection src = graph.GetIntersectionAtPosition(origin);
        NavConnection dst = graph.GetIntersectionAtPosition(destination);
        dist.Add(src, 0f);
        prev.Add(src, null);

        PriorityQueue<NavConnection, float> queue = new PriorityQueue<NavConnection, float>();
        queue.Enqueue(src, 0f);

        while(queue.Count > 0)
        {
            NavConnection next = queue.Dequeue();

            //since we use priority queue, the first time a connection is visited will be the quickest
            //so we ignore any duplicate connections in the queue
            if (visited.Contains(next))
            {
                continue;
            }
            visited.Add(next);
            
            //we are examining the destination connection, and this is verified to be the quickest route
            if(next == dst)
            {
                break;
            }

            //look at all outbound (leaving) segments
            foreach (NavSegment segment in next.Outbound)
            {
                //if we haven't ever seen this connection before, this segment is the only way to it
                if (!prev.ContainsKey(segment.EndConnection)) {

                    // add to prev
                    prev.Add(segment.EndConnection, segment);

                    //compute the path length from the origin to here
                    float pathLength = dist[next] + segment.Length;
                    dist.Add(segment.EndConnection, pathLength);

                    //throw this into the queue
                    queue.Enqueue(segment.EndConnection, pathLength);
                } 
                else
                {
                    //if we have seen this connection before, the only reason why we would do anything
                    // is if this new segment has a shorter length than what we have seen already
                    if(dist[next] + segment.Length < dist[segment.EndConnection])
                    {
                        //we have found that this segment results in shorter path.
                        prev[segment.EndConnection] = segment;

                        float pathLength = dist[next] + segment.Length;
                        dist[segment.EndConnection] = pathLength;

                        //throw this new length into the queue
                        queue.Enqueue(segment.EndConnection, pathLength);
                    }
                }
            }
        }
        
        //there is no route to the destination
        if (!prev.ContainsKey(dst))
        {
            GD.PrintErr($"Failed to find a path from to origin {origin} to {destination}. CreateRouteDjikstras.");

            return null;
        }
        //start assembling the path now
        List<NavSegment>routeSegments = new List<NavSegment> ();
        NavConnection curPrevious = dst;
        //simply iterate through the previous dict to get the fastest path
        while(prev[curPrevious] != null)
        {
            routeSegments.Add(prev[curPrevious]);
            curPrevious = prev[curPrevious].StartConnection;
        }

        //reverse since the segments are from destination to origin currently
        routeSegments.Reverse();

        return CreateRoute(routeSegments, origin, destination);
    }
}
