using Godot;
using System;

[GlobalClass]
[Tool]
public partial class CurvedRoadNavSegment : NavSegment
{
    // DATA //
    [Export] public CurvedRoad roadToFollow;
    [Export] public float startOffset = 0f;
    [Export] public float endOffset = 0f;
    [Export] public bool forward;

    public override Vector3 Start { get { return roadToFollow.StartTransform * (Vector3.Right * startOffset); } set { /* empty */ } }
    public override Vector3 End { get { return roadToFollow.EndTransform * (Vector3.Right * endOffset); } set { /* empty */ } }
    public override Vector3 Control { get { return roadToFollow.Control; } set { /* empty */ } }


    // FUNCTIONS //
    // Overrides
    public override Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
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
    public override ImmediateMesh GetCurveVisualization(Color colourToUse, int numSegments)
    {
        return EasyShapes.OffsetCurveMesh(Start, End, Control, startOffset, endOffset, colourToUse, numSegments);
    }
}
