using Godot;
using System;
using System.ComponentModel.Design;

[GlobalClass]
[Tool]
public partial class NavSegment : Node3D
{
    // DATA
    // Serializable Properties
    protected Vector3 _start = Vector3.Zero;
    [Export] public virtual Vector3 Start { get { return _start; } set { _start = value; } }
    protected Vector3 _end = Vector3.Zero;
    [Export] public virtual Vector3 End { get { return _end; } set { _end = value; } }
    protected Vector3 _control = Vector3.Zero;
    [Export] public virtual Vector3 Control { get { return _control; } set { _control = value; } }
    [Export] public float MaxSpeed = 30f;

    // Readonly Properties
    public Vector3 GlobalStart { get { return ToGlobal(Start); } }
    public Vector3 GlobalEnd { get { return ToGlobal(End); } }
    public Vector3 GlobalControl { get { return ToGlobal(Control); } }
    public Vector3[] Endpoints { get { return new Vector3[2] { Start, End }; } }
    public Vector3[] GlobalEndpoints { get { return new Vector3[2] { GlobalStart, GlobalEnd }; } }
    public Vector3 DirectionalLine { get { return End - Start; } }
    public float SimpleLength { get { return DirectionalLine.Length(); } }
    public float Length { get { return SimpleLength; } }
    
    // Constants
    public static readonly int StartPointIndex = 0;
    public static readonly int ControlPointIndex = 1;
    public static readonly int EndPointIndex = 2;

    // Runtime only properties
    public NavConnection EndConnection { get; set; }
    public NavConnection StartConnection { get; set; }

    // FUNCTIONS //

    // Data Retrieval
    public Vector3 GetOtherEndLocal(Vector3 oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return Vector3.Zero;
    }

    public Vector3 GetOtherEndGlobal(Vector3 oneEnd)
    {
        if (GlobalStart == oneEnd) return GlobalEnd;
        else if (GlobalEnd == oneEnd) return GlobalStart;
        else return Vector3.Zero;
    }

    /// <summary>
    /// Gets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <returns>The local value of the requested point</returns>
    public Vector3 GetPointByIndex(int index)
    {
        if (index == 0) return Start;
        else if (index == 1) return Control;
        else if (index == 2) return End;
        else return Vector3.Zero;
    }

    /// <summary>
    /// Sets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <param name="value">The Vector3 value of the point</param>
    public void SetPointByIndex(int index, Vector3 value)
    {
        if (index == 0) Start = value;
        else if (index == 1) Control = value;
        else if (index == 2) End = value;
    }

    /// <summary>
    /// Gets the 3D position a given percentage from the start of the segment.
    /// </summary>
    /// <param name="percentOfSegment">How far along the segment</param>
    /// <param name="globalCoordinates">True if you want the result using global coordinates</param>
    /// <returns></returns>
    public virtual Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.BezierQuadratic3D(
            Start,
            Control,
            End,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }

    /// <summary>
    /// Gets a Mesh visualization of the curve of this segment in the requested colour and detail level.
    /// </summary>
    /// <param name="colourToUse"></param>
    /// <param name="numSegments"></param>
    /// <returns></returns>
    public virtual ImmediateMesh GetCurveVisualization(Color colourToUse, int numSegments)
    {
        return EasyShapes.CurveMesh(Start, End, Control, colourToUse, numSegments);
    }
}
