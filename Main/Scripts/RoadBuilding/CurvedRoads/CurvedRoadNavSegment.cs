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

    public override Vector3 Start { 
        get 
        { 
            if (forward) { return roadToFollow.StartTransform * (Vector3.Right * startOffset); } 
            else { return roadToFollow.EndTransform * (Vector3.Left * startOffset); } 
        } 
    }
    public override Vector3 End {
        get
        {
            if (forward) { return roadToFollow.EndTransform * (Vector3.Right * endOffset); }
            else { return roadToFollow.StartTransform * (Vector3.Left * endOffset); }
        }
    }

    // FUNCTIONS //
    // Overrides
    public override Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Vector3.Zero;
        float percentToUse = percentOfSegment;

        if(!forward)
        {
            percentToUse = 1.0f - percentOfSegment;
        }

        localPos = Curves.BezierQuadraticWithOffset3D(
            roadToFollow.Start,
            roadToFollow.Control,
            roadToFollow.End,
            startOffset,
            endOffset,
            percentToUse
            );

        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }

    public override Vector3 GetPositionOnSegmentAbsolute(float distanceAlongSegment, bool globalCoordinates = true)
    {
        float percentage = distanceAlongSegment / Length;
        return GetPositionOnSegment(percentage, globalCoordinates);
    }


    public override Vector3[] SubdivideIntoPoints(int numPoints)
    {
        return Curves.BezierQuadraticWithOffset3DToPoints(roadToFollow.Start, roadToFollow.Control, roadToFollow.End, startOffset, endOffset, numPoints);
    }
}
