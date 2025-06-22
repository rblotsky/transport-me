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
    public abstract float Length { get ; }
    
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

    // Abstract Methods
    /// <summary>
    /// Gets the 3D position a given percentage from the start of the segment.
    /// </summary>
    /// <param name="percentOfSegment">How far along the segment</param>
    /// <param name="globalCoordinates">True if you want the result using global coordinates</param>
    /// <returns></returns>
    public abstract Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true);

    /// <summary>
    /// Gets a Mesh visualization of the curve of this segment in the requested colour and detail level.
    /// </summary>
    /// <param name="colourToUse"></param>
    /// <param name="numSegments"></param>
    /// <returns></returns>
    public abstract ImmediateMesh GetCurveVisualization(Color colourToUse, int numSegments);
}
