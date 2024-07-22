using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
[Tool]
public partial class NavNode : RefCounted
{
    // DATA //
    // Values
    private List<NavSegment> connectedSegments = new List<NavSegment>();

    // Properties
    public Vector3I GridPosition { get; set; }
    public NavSegment[] Segments { get { return connectedSegments.ToArray(); } }
    public NavSegment[] StartingSegments { get { return connectedSegments.Where((segment) => segment.Start == GridPosition).ToArray(); } }
    public NavSegment[] EndingSegments { get { return connectedSegments.Where((segment) => segment.End == GridPosition).ToArray(); } }


    // FUNCTIONS //
    // Connections
    public void AddSegment(NavSegment newSegment, bool isStart)
    {
        connectedSegments.Add(newSegment);
    }
}
