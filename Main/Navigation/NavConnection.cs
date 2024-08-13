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
		private List<NavSegment> inboundConnections;
		private List<NavSegment> outboundConnections;
		private Vector3 intersectionPosition;
		public List<NavSegment> InboundSegments { get { return inboundConnections; } }
		public List<NavSegment> OutboundSegments { get { return outboundConnections; } }
		public int InboundSegmentCount { get { return inboundConnections.Count;} }
		public int OutboundSegmentCount { get { return outboundConnections.Count;} }
		public Vector3 IntersectionPosition { get { return intersectionPosition; } }
		

		public void initialize(Vector3 position)
		{
			inboundConnections = new List<NavSegment>();
			outboundConnections = new List<NavSegment>();
			intersectionPosition = position;
		}	
		/// <summary>
		/// Add an inbound (entering) to this connection. NOTE: ensure the segment's global start position matches this connection's position
		/// </summary>
		/// <param name="segment">The segment with the matching end position</param>
		public void AddInbound(NavSegment segment)
		{
			if (Simplifications.Vector3ApproximationEquality(segment.GlobalEnd, intersectionPosition))
		{
				inboundConnections.Add(segment);
			} else
			{
				GD.PrintErr("Tried to add inbound segment endpoint", segment.GlobalEnd.ToString() , " to intersection", intersectionPosition.ToString());
			}
		}
		/// <summary>
		/// Add an outbound (leaving) to this connection. NOTE: ensure the segment's global start position matches this connection's position
		/// </summary>
		/// <param name="segment">The segment with the matching start position</param>
		public void AddOutbound(NavSegment segment)
		{
			if (Simplifications.Vector3ApproximationEquality(segment.GlobalStart, intersectionPosition))
			{
				outboundConnections.Add(segment);
			}
			else
			{
				GD.PrintErr("Tried to add outbound segment endpoint", segment.GlobalStart.ToString(), " to intersection", intersectionPosition.ToString());
			}
		}

	}

