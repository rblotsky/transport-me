using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public partial class RoutePoint : RefCounted
	{
		private Vector3 _globalPoint;
		private Vector3 _rotationVector;
		private NavSegment _segment;
		public Vector3 backPoint { set; get; }
		public Vector3 forwardPoint { set; get; }
		public float lerpValue { set; get; }
		public Vector3 Position { get { return _globalPoint;  } set { _globalPoint = value; } }
		public Vector3 Rotation { get { return _rotationVector; } set { _rotationVector = value; } }
		public NavSegment Segment { get { return _segment; } set { _segment = value; } }
	}
