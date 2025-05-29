using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;

[GlobalClass]
[Tool]
public partial class NavSegment : Node3D, IDebugVisualizationProvider
{
    // DATA
    // Serializable Properties
    private Vector3 _start = Vector3.Zero;
    [Export] private Vector3 Start { get { return _start; } set { _start = value; } }
    private Vector3 _end = Vector3.Zero;
    [Export] private Vector3 End { get { return _end; } set { _end = value; } }
    private Vector3 _control = Vector3.Zero;
    [Export] private Vector3 Control { get { return _control; } set { _control = value; } }

    [Export] public float MaxSpeed = 30f;
    // Readonly Properties
    public Vector3 GlobalStart { get { return ToGlobal(Start); } }
    public Vector3 GlobalEnd { get { return ToGlobal(End); } }
    public Vector3 GlobalControl { get { return ToGlobal(Control); } }
    public Vector3 DirectionalLine { get { return End - Start; }}
    public float SimpleLength { get { return DirectionalLine.Length(); } }
    public float Length { get { return SimpleLength; } } // TODO: Use a proper length calculation

    // Runtime only properties
    public NavConnection EndConnection { get; set; }
    public NavConnection StartConnection { get; set; }



    // FUNCTIONS //


    // Data Retrieval
    public Vector3 GetOtherEndLocal(Vector3 oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return Vector3.Zero ;
    }

    public Vector3 GetOtherEndGlobal(Vector3 oneEnd)
    {
        if (GlobalStart == oneEnd) return GlobalEnd;
        else if (GlobalEnd == oneEnd) return GlobalStart;
        else return Vector3.Zero;
    }
    public Vector3 GetDirectionVectorOnSegment(float percentOfSegment)
    {
        float offset = Mathf.Max(0.1f / Length, 0.01f);

        float to = Mathf.Min(1f, percentOfSegment + offset);
        float from = Mathf.Max(0f, percentOfSegment - offset);
        Vector3 dir = GetPositionOnSegment(to) - GetPositionOnSegment(from);
        return dir.Normalized();
    }
    public Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.CalculateBezierQuadraticIn3D(
            Start,
            Control,
            End,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }

    public IEnumerable<DebugVisualization> GetVisualization()
    {
        yield return DebugVisualizationFactory.Curve(Start, End, Control, Colors.LightBlue);
        yield return DebugVisualizationFactory.Arrow(End, Control, Colors.Red, 0.5f);
        yield return DebugVisualizationFactory.Sphere(End, 0.5f, Colors.Red, 0.3f);
        yield return DebugVisualizationFactory.Sphere(Start, 0.3f, Colors.Blue, 0.7f);
    }
}
