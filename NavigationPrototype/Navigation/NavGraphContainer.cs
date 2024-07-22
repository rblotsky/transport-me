using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NavGraphContainer : Node3D
{
    // DATA //
    // Cached Data
    private Dictionary<Vector3I, NavNode> nodesByLocation;
    private List<NavSegment> segments;


    // FUNCTIONS //
    private void SetupNavGraph()
    {
        nodesByLocation = new Dictionary<Vector3I, NavNode>();
        segments = new List<NavSegment>();
    }

    private void ClearNavGraph()
    {
        if(nodesByLocation != null)
        {
            nodesByLocation.Clear();
            nodesByLocation = null;
        }

        if(segments != null)
        {
            segments.Clear();
            segments = null;
        }
    }

    private void CollapsePloppables(PloppableGraph[] ploppables)
    {
        ClearNavGraph();
        SetupNavGraph();
        foreach(PloppableGraph ploppable in ploppables)
        {
            foreach(NavSegment segment in ploppable.segments)
            {
                segments.Add(segment);
                AddConnectorNode(segment.Start, segment, true);
                AddConnectorNode(segment.End, segment, false);
            }
        }
    }

    private void AddConnectorNode(Vector3I position, NavSegment segment, bool isStart)
    {
        if (nodesByLocation.TryGetValue(position, out NavNode existing))
        {
            existing.AddSegment(segment, isStart);
        }
        else
        {
            NavNode newNode = new NavNode();
            newNode.GridPosition = position;
            newNode.AddSegment(segment, isStart);
            nodesByLocation.Add(position, newNode);
        }
    }

    
    // Data Retrieval
    public NavNode GetRandomNode(RandomNumberGenerator rng)
    {
        return nodesByLocation.Values.ToList()[rng.RandiRange(0, nodesByLocation.Count - 1)];
    }
}
