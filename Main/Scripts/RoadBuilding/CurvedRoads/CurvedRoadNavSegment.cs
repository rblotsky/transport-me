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
