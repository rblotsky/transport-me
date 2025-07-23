using Godot;
using System;
using System.ComponentModel.Design;

[Tool]
public abstract partial class NavSegment : Node3D
{
    // DATA //
    // Navigation and Usage
    [Export] public float MaxSpeed = 30f;

    // Readonly Properties
    public abstract Vector3 Start { get; }
    public abstract Vector3 End { get; }
    public Vector3 GlobalStart { get { return ToGlobal(Start); } }
    public Vector3 GlobalEnd { get { return ToGlobal(End); } }
    public Vector3 DirectionalLine { get { return End - Start; } }
    public float SimpleLength { get { return DirectionalLine.Length(); } }
    public float Length { get { return _length.HasValue ? _length.Value : ComputeLength(); } }
    private float? _length;

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

    private float ComputeLength()
    {
        float trueLength = 0f;
        Vector3[] points = SubdivideIntoPoints(10);
        for (int i = 1; i < points.Length; i++)
        {
            trueLength += (points[i-1] - points[i]).Length();
        }
        _length = trueLength;
        return trueLength;
    }

    // Abstract Methods
    /// <summary>
    /// Gets the 3D position a given percentage from the start of the segment.
    /// </summary>
    /// <param name="percentOfSegment">How far along the segment</param>
    /// <param name="globalCoordinates">True if you want the result using global coordinates</param>
    /// <returns></returns>
    public abstract Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true);
    /// <summary>
    /// Gets the 3D position a given distance from the start of the segment
    /// </summary>
    /// <param name="distanceAlongSegment"></param>
    /// <param name="globalCoordinates"></param>
    /// <returns></returns>
    public abstract Vector3 GetPositionOnSegmentAbsolute(float distanceAlongSegment,  bool globalCoordinates = true);

    /// <summary>
    /// Returns a list of discrete points from the start to end of the segment.
    /// </summary>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public abstract Vector3[] SubdivideIntoPoints(int numPoints);

    public Vector3[] ConvertPointsToGlobal(Vector3[] points)
    {
        Vector3[] convertedPoints = new Vector3[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            convertedPoints[i] = ToGlobal(points[i]);
        }
        return convertedPoints;
    }
}
