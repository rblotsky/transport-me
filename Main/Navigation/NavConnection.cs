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
		private List<NavSegment> inboundSegments;
		private List<NavSegment> outboundSegments;
		private Vector3I intersectionPosition;
		public List<NavSegment> InboundSegments { get { return inboundSegments; } }
		public List<NavSegment> OutboundSegments { get { return outboundSegments; } }
		public int InboundSegmentCount { get { return inboundSegments.Count;} }
		public int OutboundSegmentCount { get { return outboundSegments.Count;} }
		public Vector3I IntersectionPosition { get { return intersectionPosition; } }
		

		public void initialize(Vector3I position)
		{
			inboundSegments = new List<NavSegment>();
			outboundSegments = new List<NavSegment>();
			intersectionPosition = position;
		}
	/// <summary>
	/// Add an inbound (entering) to this connection. NOTE: ensure the segment's global start position matches this connection's position
	/// </summary>
	/// <param name="segment">The segment with the matching end position</param>
	public void AddInbound(NavSegment segment)
		{
			if (segment.GlobalEnd.Equals(intersectionPosition))
			{
				inboundSegments.Add(segment);
			} else
			{
				GD.PrintErr("Failed to add inbound segment endpoint", segment.GlobalEnd.ToString() , " to intersection", intersectionPosition.ToString());
			}
		}
	/// <summary>
	/// Add an outbound (leaving) to this connection. NOTE: ensure the segment's global start position matches this connection's position
	/// </summary>
	/// <param name="segment">The segment with the matching start position</param>
		public void AddOutbound(NavSegment segment)
		{
			if (segment.GlobalStart.Equals(intersectionPosition))
			{
				outboundSegments.Add(segment);
			}
			else
			{
				GD.PrintErr("Failed to add outbound segment endpoint", segment.GlobalStart.ToString(), " to intersection", intersectionPosition.ToString());
			}
		}

	}

