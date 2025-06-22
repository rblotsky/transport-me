using Godot;
using System;
using System.ComponentModel.Design;

[GlobalClass]
[Tool]
public partial class BezierCurveNavSegment : NavSegment
{
    // DATA
    // Controlling Position
    private Vector3 _start = new Vector3(1,0,1);
    [Export] public Vector3 CurveStart { get { return _start; } set { _start = value; } }
    private Vector3 _end = Vector3.Zero;
    [Export] public Vector3 CurveEnd { get { return _end; } set { _end = value; } }
    private Vector3 _control = new Vector3(-1, 0, -1);
    [Export] public Vector3 CurveControl { get { return _control; } set { _control = value; } }

    // Readonly Properties
    public override Vector3 Start { get { return CurveStart; } }
    public override Vector3 End { get { return CurveEnd; } }
    public override float Length { get { return SimpleLength; } }

    // Constants
    public static readonly int StartPointIndex = 0;
    public static readonly int ControlPointIndex = 1;
    public static readonly int EndPointIndex = 2;


    // FUNCTIONS //
    /// <summary>
    /// Gets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <returns>The local value of the requested point</returns>
    public Vector3 GetCurvePointByIndex(int index)
    {
        if (index == 0) return CurveStart;
        else if (index == 1) return CurveControl;
        else if (index == 2) return CurveEnd;
        else return Vector3.Zero;
    }

    /// <summary>
    /// Sets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <param name="value">The Vector3 value of the point</param>
    public void SetCurvePointByIndex(int index, Vector3 value)
    {
        if (index == 0) CurveStart = value;
        else if (index == 1) CurveControl = value;
        else if (index == 2) CurveEnd = value;
    }

    public override Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.BezierQuadratic3D(
            CurveStart,
            CurveControl,
            CurveEnd,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }

    public override Vector3[] SubdivideIntoPoints(int numPoints)
    {
        return Curves.BezierQuadratic3DToPoints(CurveStart, CurveControl, CurveEnd, numPoints);
    }
}
