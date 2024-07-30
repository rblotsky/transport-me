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
		private Vector3I intersectionPosition;
		public List<NavSegment> InboundSegments { get { return inboundConnections; } }
		public List<NavSegment> OutboundSegments { get { return outboundConnections; } }
		public int InboundSegmentCount { get { return inboundConnections.Count;} }
		public int OutboundSegmentCount { get { return outboundConnections.Count;} }
		public Vector3I IntersectionPosition { get { return intersectionPosition; } }
		

		public void initialize(Vector3I position)
		{
			inboundConnections = new List<NavSegment>();
			outboundConnections = new List<NavSegment>();
			intersectionPosition = position;
		}

		public void AddInbound(NavSegment segment)
		{
			if (segment.GlobalEnd.Equals(intersectionPosition))
			{
				inboundConnections.Add(segment);
			} else
			{
				GD.PrintErr("Tried to add inbound segment endpoint", segment.GlobalEnd.ToString() , " to intersection", intersectionPosition.ToString());
			}
		}
		public void AddOutbound(NavSegment segment)
		{
			if (segment.GlobalStart.Equals(intersectionPosition))
			{
				outboundConnections.Add(segment);
			}
			else
			{
				GD.PrintErr("Tried to add outbound segment endpoint", segment.GlobalStart.ToString(), " to intersection", intersectionPosition.ToString());
			}
		}

	}

