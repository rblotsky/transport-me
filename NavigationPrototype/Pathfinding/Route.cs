using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Route : Node
{
    // DATA //
    // Instance Data
    private NavSegment[] orderedSegments;
    private NavNode startNode;
    private NavNode endNode;

    public NavSegment[] Segments { get { return orderedSegments; } }
    public NavNode Start { get { return startNode; } }
    public NavNode End { get { return endNode; } }


    // CONSTRUCTOR //
    public Route(IEnumerable<NavSegment> segmentsInOrder, NavNode start, NavNode end)
    {
        orderedSegments = segmentsInOrder.ToArray();
        startNode = start;
        endNode = end;
    }


    // FUNCTIONS 
    // Pathfinding
    public static Route CreateRouteBFS(NavNode origin, NavNode destination)
    {
        // Ignores if origin == destination
        if (origin == destination) return new Route(new NavSegment[0], origin, destination);

        // Runs a BFS algorithm to create the Route
        List<NavSegment> segmentsToUse = new List<NavSegment>();
        Queue<NavNode> nodesToScan = new Queue<NavNode>();
        Dictionary<NavNode, int> nodeLevels = new Dictionary<NavNode, int>();
        
        // Starting from the origin, check each "level" (number of segments from origin)
        // before moving on to the next.
        nodesToScan.Enqueue(origin);
        nodeLevels[origin] = 0;
        while(nodesToScan.Count != 0)
        {
            NavNode currentNode = nodesToScan.Dequeue();

            foreach(NavSegment attachedSegment in currentNode.attachedSegments)
            {
                NavNode otherEndNode = attachedSegment.GetOtherEnd(currentNode);

                // If the segment leads to the destination, goes back to find the shortest path
                if(attachedSegment.Endpoints.Contains(destination))
                {
                    segmentsToUse.Add(attachedSegment);

                    // Go backwards from the end. We start from the node before last because we already added
                    // that segment.
                    // This checks all segments around each node for ones that are ONE level below. Any node
                    // exactly 1 level below MUST be the fastest path back.
                    NavNode backwardsCheckNode = currentNode;
                    while(backwardsCheckNode != origin)
                    {
                        foreach(NavSegment backwardSegment in backwardsCheckNode.attachedSegments)
                        {
                            NavNode nextBackwardNode = backwardSegment.GetOtherEnd(backwardsCheckNode);
                            if (nodeLevels.ContainsKey(nextBackwardNode) && nodeLevels[nextBackwardNode] == nodeLevels[backwardsCheckNode]-1)
                            {
                                segmentsToUse.Add(backwardSegment);
                                backwardsCheckNode = nextBackwardNode;
                                break;
                            }
                        }
                    }

                    // Since we added this backwards, we need to reverse it to get origin->destination.
                    segmentsToUse.Reverse();
                    nodesToScan.Clear();
                    break;
                }

                // Adds the attached node to scan if it hasn't been added yet.
                else if (!nodeLevels.ContainsKey(otherEndNode))
                {
                    nodesToScan.Enqueue(otherEndNode);
                    nodeLevels[otherEndNode] = nodeLevels[currentNode] + 1;
                }
            }
        }

        // Using the segments we got, assemble a route.
        // Return null if no route was found.
        if (segmentsToUse.Count > 0)
        {
            return new Route(segmentsToUse, origin, destination);
        }
        else
        {
            return null;
        }

    }
}
