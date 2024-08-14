using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// An internal class to represent and store an intersection between one or more segments.
/// Stores the inbound and outbound segments
/// </summary>
public partial class NavConnection : RefCounted
{
    // DATA //
    // Local values
	private List<NavSegment> inboundConnections = new List<NavSegment>();
	private List<NavSegment> outboundConnections = new List<NavSegment>();
	private Vector3 intersectionPosition;

    // Public Properties
	public NavSegment[] Inbound { get { return inboundConnections.ToArray(); } }
	public NavSegment[] Outbound { get { return outboundConnections.ToArray(); } }
	public int InboundCount { get { return inboundConnections.Count;} }
	public int OutboundCount { get { return outboundConnections.Count;} }

    // Public Modifiable Properties
	public Vector3 IntersectionPosition { get { return intersectionPosition; } set { intersectionPosition = value; } }
		

    // FUNCTIONS //
	/// <summary>
	/// Add an Ending (Inbound) to this connection. NOTE: ensure the segment's global end position matches this connection's position
	/// </summary>
	/// <param name="segment">The segment with the matching end position</param>
	public void AddInbound(NavSegment segment)
	{
		if (Simplifications.V3ApproximatelyEqual(segment.GlobalEnd, intersectionPosition))
		{
			inboundConnections.Add(segment);
		} 
        else
		{
			GD.PrintErr("Tried to add ending segment endpoint", segment.GlobalEnd.ToString() , " to intersection", intersectionPosition.ToString());
		}
	}

	/// <summary>
	/// Add a Starting (Outbound) to this connection. NOTE: ensure the segment's global start position matches this connection's position
	/// </summary>
	/// <param name="segment">The segment with the matching start position</param>
	public void AddOutbound(NavSegment segment)
	{
		if (Simplifications.V3ApproximatelyEqual(segment.GlobalStart, intersectionPosition))
		{
			outboundConnections.Add(segment);
		}
		else
		{
			GD.PrintErr("Tried to add a starting segment endpoint", segment.GlobalStart.ToString(), " to intersection", intersectionPosition.ToString());
		}
	}

}

