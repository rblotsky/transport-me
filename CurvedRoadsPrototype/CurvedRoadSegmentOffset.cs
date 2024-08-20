using Godot;
using System;

[GlobalClass]
[Tool]
public partial class CurvedRoadSegmentOffset : RefCounted
{
    // DATA //
    /*
    [Export] public int segmentIndex;
    [Export] public int roadStartOffset;
    [Export] public int roadEndOffset;
    [Export] bool direction;
    */

    // Properties
    public NavSegment Segment { get; private set; }
    public Vector3 RoadStartOffset { get; private set; }
    public Vector3 RoadEndOffset { get; private set; }
    public bool Direction { get; private set; }

    // Readonly Properties
    public int EndpointAtRoadStart { get { if (Direction) return 0; else return 1; } }
    public int EndpointAtRoadEnd { get { if (Direction) return 1; else return 0; } }


    // FUNCTIONS //
    public static CurvedRoadSegmentOffset GetSegmentOffset(NavSegment segment, CurvedRoad road)
    {
        CurvedRoadSegmentOffset segmentOffsetData = new CurvedRoadSegmentOffset();

        segmentOffsetData.Segment = segment;


        // Decides which direction the segment is going. Using that, saves
        // offsets from road start/end.
        Vector3 roadStartToSegmentStart = (road.ToLocal(segment.GlobalStart)) * road.StartTransform;
        Vector3 roadStartToSegmentEnd = (road.ToLocal(segment.GlobalEnd)) * road.StartTransform;

        GD.Print($"Road Start: {road.Start}");
        GD.Print($"Road start transform: {road.StartTransform}");
        GD.Print($"Global start of segment: {segment.GlobalStart}, Local start: {segment.Start}, After transform: {road.Start + road.ToGlobal(road.StartTransform * roadStartToSegmentStart)}");
        GD.Print($"Offset from road start: {roadStartToSegmentStart}");

        Vector3 roadEndToSegmentStart = (road.ToLocal(segment.GlobalStart)) * road.EndTransform;
        Vector3 roadEndToSegmentEnd = (road.ToLocal(segment.GlobalEnd)) * road.EndTransform;

        if(roadStartToSegmentEnd.LengthSquared() > roadStartToSegmentStart.LengthSquared())
        {
            segmentOffsetData.RoadStartOffset = roadStartToSegmentStart;
            segmentOffsetData.RoadEndOffset = roadEndToSegmentEnd;
            segmentOffsetData.Direction = true;
        }
        else
        {
            segmentOffsetData.RoadStartOffset = roadStartToSegmentEnd;
            segmentOffsetData.RoadEndOffset = roadEndToSegmentStart;
            segmentOffsetData.Direction = false;
        }

        return segmentOffsetData;
    }
}
