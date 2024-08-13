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
        // Runs a BFS algorithm to create the Route
        Dictionary<NavConnection, float> dist = new Dictionary<NavConnection, float>(); //length from this nav connection to src
        Dictionary<NavConnection, NavSegment> prev = new Dictionary<NavConnection, NavSegment>(); //segment used (inbound)
        HashSet<NavConnection> visited = new HashSet<NavConnection>();

        NavConnection src = graph.GetIntersectionAt(origin);
        NavConnection dst = graph.GetIntersectionAt(destination);
        dist.Add(src, 0f);
        prev.Add(src, null);
        PriorityQueue<NavConnection, float> queue = new PriorityQueue<NavConnection, float>();
        queue.Enqueue(src, 0f);

        while(queue.Count > 0)
        {
            NavConnection next = queue.Dequeue();
            if (visited.Contains(next))
            {
                continue;
            }
            visited.Add(next);
            
            if(next == dst)
            {
                break;
            }
            
            foreach (NavSegment segment in next.OutboundSegments) //look at all outbound (leaving) segments
            {
                if (!prev.ContainsKey(segment.InboundConnection)) {
                    //if we haven't ever seen this connection before, this segment is the only way to it
                    // add to prev
                    prev.Add(segment.InboundConnection, segment);

                    //compute the path length from the origin to here
                    float pathLength = dist[next] + segment.Length;
                    dist.Add(segment.InboundConnection, pathLength);

                    //throw this into the queue
                    queue.Enqueue(segment.InboundConnection, pathLength);
                } else
                {
                    //if we have seen this connection before, the only reason why we would do anything
                    // is if this new segment has a shorter length than what we have seen already
                    if(dist[next] + segment.Length < dist[segment.InboundConnection])
                    {
                        //we have found that this segment results in shorter path.
                        prev[segment.InboundConnection] = segment;

                        float pathLength = dist[next] + segment.Length;
                        dist[segment.InboundConnection] = pathLength;

                        //throw this new length into the queue
                        queue.Enqueue(segment.InboundConnection, pathLength);
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
        
        List<NavSegment>routeSegments = new List<NavSegment> ();
        NavConnection curPrevious = dst;
        while(prev[curPrevious] != null)
        {
            routeSegments.Add(prev[curPrevious]);
            curPrevious = prev[curPrevious].OutboundConnection;
        }
        routeSegments.Reverse();
        return CreateRoute(routeSegments, origin, destination);
    }
    // Pathfinding
    public static Route CreateRouteBFS(Vector3 origin, Vector3 destination, NavGraphContainer graph)
    {
        // Ignores if origin == destination
        if (origin == destination) return CreateRoute(new NavSegment[0], origin, destination);

        // Runs a BFS algorithm to create the Route
        List<NavSegment> segmentsToUse = new List<NavSegment>();
        Queue<Vector3> pointsToScan = new Queue<Vector3>();
        Dictionary<Vector3, int> pointLevels = new Dictionary<Vector3, int>();

        // Starting from the origin, check each "level" (number of segments from origin)
        // before moving on to the next.
        pointsToScan.Enqueue(origin);
        pointLevels[origin] = 0;
        while(pointsToScan.Count != 0)
        {
            Vector3 currentConnector = pointsToScan.Dequeue();

            foreach(NavSegment attachedSegment in graph.GetStartingSegments(currentConnector))
            {
                Vector3 otherEnd = attachedSegment.GlobalEnd;
                // If the segment leads to the destination, goes back to find the shortest path
                if(Simplifications.Vector3ApproximationEquality(attachedSegment.GlobalEnd, destination))
                {
                    segmentsToUse.Add(attachedSegment);

                    // Go backwards from the end. We start from the node before last because we already added
                    // that segment.
                    // This checks all segments around each node for ones that are ONE level below. Any node
                    // exactly 1 level below MUST be the fastest path back.
                    Vector3 backwardsCheck = currentConnector;
                    while(!Simplifications.Vector3ApproximationEquality(backwardsCheck, origin))
                    {
                        // Setting up the kill switch.
                        int maxChecks = graph.GetEndingSegments(backwardsCheck).Count;
                        int checksDone = 0;
                        int segmentsBeforeChecks = segmentsToUse.Count;
                        foreach(NavSegment backwardSegment in graph.GetEndingSegments(backwardsCheck))
                        {
                            // Kill switch counter
                            checksDone++;

                            // Only check this segment if it is bidirectional or ENDS at the current node.
                            if (Simplifications.Vector3ApproximationEquality(backwardSegment.GlobalEnd, backwardsCheck))
                            {
                                Vector3 nextBackwardConnector = backwardSegment.GetOtherEndGlobal(backwardsCheck);

                                if (pointLevels.ContainsKey(nextBackwardConnector) && pointLevels[nextBackwardConnector] == pointLevels[backwardsCheck] - 1)
                                {
                                    segmentsToUse.Add(backwardSegment);
                                    backwardsCheck = nextBackwardConnector;
                                    break;
                                }
                            }
                        }

                        // Kill switch - if we check every segment and find NOTHING that works, return an empty route.
                        // This ALWAYS indicates a problem with the program. The only reason this is here is to avoid
                        // infinite loops crashing the system.
                        if(checksDone == maxChecks && segmentsBeforeChecks == segmentsToUse.Count)
                        {
                            GD.Print($"ERR: Failed to find a way back to origin from {backwardsCheck}. This is from the innermost loop of CreateRouteBFS.");
                            Debugger3D.main.SphereEffect(backwardsCheck, 0.5f, Colors.Red, 1, 10);
                            return CreateRoute(new NavSegment[0], origin, destination);
                        }
                    }

                    // Since we added this backwards, we need to reverse it to get origin->destination.
                    segmentsToUse.Reverse();
                    pointsToScan.Clear();
                    break;
                }

                // Adds the attached node to scan if it hasn't been added yet
                else if (!pointLevels.ContainsKey(otherEnd))
                {
                    pointsToScan.Enqueue(otherEnd);
                    pointLevels[otherEnd] = pointLevels[currentConnector] + 1;
                }
            }
        }

        // Using the segments we got, assemble a route.
        // Return null if no route was found.
        if (segmentsToUse.Count > 0)
        {
            return CreateRoute(segmentsToUse, origin, destination);
        }
        else
        {
            return null;
        }
        
    }
    
}
