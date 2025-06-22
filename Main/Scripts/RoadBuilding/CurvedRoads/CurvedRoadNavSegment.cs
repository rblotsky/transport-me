using Godot;
using System;

[GlobalClass]
[Tool]
public partial class CurvedRoadNavSegment : NavSegment
{
    // DATA //
    [Export] private CurvedRoad roadToFollow;
    [Export] private float startOffset = 0f;
    [Export] private float endOffset = 0f;
    [Export] private bool forward;

    public override Vector3 Start { get { return roadToFollow.StartTransform * (Vector3.Right * startOffset); } }
    public override Vector3 End { get { return roadToFollow.EndTransform * (Vector3.Right * endOffset); } }

    public override float Length { get { return SimpleLength; } }

    // FUNCTIONS //
    // Overrides
    public override Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.BezierQuadratic3DWithOffset(
            roadToFollow.Start,
            roadToFollow.Control,
            roadToFollow.End,
            startOffset, 
            endOffset,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }

    
    public override ImmediateMesh GetCurveVisualization(Color colourToUse, int numSegments)
    {
        return EasyShapes.OffsetCurveMesh(roadToFollow.Start, roadToFollow.End, roadToFollow.Control, startOffset, endOffset, colourToUse, numSegments);
    }
}
