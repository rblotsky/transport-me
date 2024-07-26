using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Route : RefCounted
{
    // DATA //
    // Instance Data
    public NavSegment[] OrderedSegments { get; set; }
    public Vector3I StartPoint { get; set; }
    public Vector3I EndPoint { get; set; }


    // FUNCTIONS 
    // Constructing
    public static Route CreateRoute(IEnumerable<NavSegment> segmentsToTake, Vector3I start, Vector3I end)
    {
        Route newRoute = new Route();
        newRoute.OrderedSegments = segmentsToTake.ToArray();
        newRoute.StartPoint = start;
        newRoute.EndPoint = end;

        return newRoute;
    }


    // Pathfinding
    public static Route CreateRouteBFS(Vector3I origin, Vector3I destination, NavGraphContainer graph)
    {
        GD.Print("\tEntered CreateRouteBFS");
        // Ignores if origin == destination
        if (origin == destination) return CreateRoute(new NavSegment[0], origin, destination);

        // Runs a BFS algorithm to create the Route
        List<NavSegment> segmentsToUse = new List<NavSegment>();
        Queue<Vector3I> pointsToScan = new Queue<Vector3I>();
        Dictionary<Vector3I, int> pointLevels = new Dictionary<Vector3I, int>();

        // Starting from the origin, check each "level" (number of segments from origin)
        // before moving on to the next.
        pointsToScan.Enqueue(origin);
        pointLevels[origin] = 0;
        while(pointsToScan.Count != 0)
        {
            Vector3I currentConnector = pointsToScan.Dequeue();

            foreach(NavSegment attachedSegment in graph.GetConnections(currentConnector))
            {
                Vector3I otherEnd = attachedSegment.GetOtherEndGlobal(currentConnector);

                if (attachedSegment.GlobalEnd == destination || attachedSegment.GlobalStart == destination)
                {
                    GD.Print("Reached destination lol");
                }
                // If the segment leads to the destination, goes back to find the shortest path
                if(attachedSegment.GlobalEnd == destination || (attachedSegment.Bidirectional && otherEnd == destination))
                {
                    segmentsToUse.Add(attachedSegment);

                    // Go backwards from the end. We start from the node before last because we already added
                    // that segment.
                    // This checks all segments around each node for ones that are ONE level below. Any node
                    // exactly 1 level below MUST be the fastest path back.
                    Vector3I backwardsCheck = currentConnector;
                    while(backwardsCheck != origin)
                    {
                        // Setting up the kill switch.
                        int maxChecks = graph.GetEndingConnections(backwardsCheck).Count;
                        int checksDone = 0;
                        int segmentsBeforeChecks = segmentsToUse.Count;
                        foreach(NavSegment backwardSegment in graph.GetConnections(backwardsCheck))
                        {
                            // Kill switch counter
                            checksDone++;

                            // Only check this segment if it is bidirectional or ENDS at the current node.
                            if (backwardSegment.GlobalEnd == backwardsCheck || backwardSegment.Bidirectional)
                            {
                                Vector3I nextBackwardConnector = backwardSegment.GetOtherEndGlobal(backwardsCheck);

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

                // Adds the attached node to scan if it hasn't been added yet and it's bidirectional or starts here
                else if (!pointLevels.ContainsKey(otherEnd) && (attachedSegment.GlobalStart == currentConnector || attachedSegment.Bidirectional))
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
