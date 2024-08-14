using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Route : RefCounted
{
    // DATA //
    // Instance Data
    public NavSegment[] OrderedSegments { get; set; }
    public Vector3 StartPoint { get; set; }
    public Vector3 EndPoint { get; set; }


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

        NavConnection src = graph.IntersectionAtPosition(origin);
        NavConnection dst = graph.IntersectionAtPosition(destination);
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
