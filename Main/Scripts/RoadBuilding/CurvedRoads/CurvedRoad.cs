using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class CurvedRoad : Node3D
{
    // DATA //
    // Graph data
    [ExportCategory("Road Shape Data")]
    private Vector3 _start = Vector3.Zero;
    [Export] public Vector3 Start { get { return _start; } set { _start = value; } }
    private Vector3 _end = Vector3.Zero;
    [Export] public Vector3 End { get { return _end; } set { _end = value; } }
    private Vector3 _control = Vector3.Zero;
    [Export] public Vector3 Control { get { return _control; } set { _control = value ; } }

    // Properties
    public Vector3 ControlAtStartHeight { get { return new Vector3(Control.X, Start.Y, Control.Z); } }
    public Vector3 ControlAtEndHeight { get { return new Vector3(Control.X, End.Y, Control.Z); } }
    public Vector3 ControlAtAvgHeight { get { return new Vector3(Control.X, (Start.Y + End.Y) / 2, Control.Z);  } }
    public Vector3 DirectionalLine { get { return End - Start; } }
    /// <summary>
    /// A transform of the Start. Origin = Start, Basis = facing Control from Start
    /// </summary>
    public Transform3D StartTransform { get { return new Transform3D(Basis.LookingAt(Start - ControlAtStartHeight), Start); } }

    /// <summary>
    /// A transform of the End. Origin = End, Basis = facing End from Control
    /// </summary>
    public Transform3D EndTransform { get { return new Transform3D(Basis.LookingAt(ControlAtEndHeight - End), End); } }


    // Road Mesh Data
    [ExportCategory("Mesh")]
    [Export] private RoadMesh roadMesh;
    [Export] private MeshInstance3D meshRenderer;
    [Export] private bool debugNormals = false;
    [Export] private MeshInstance3D debugRenderer;


    // Segment Offset Data
    [ExportCategory("Saving Controlled Segments")]
    [Export] private bool SaveSegmentOffsetsToggle { set { SaveSegmentOffsets(); } get { return true; } }
    [Export] private Array<NavSegment> segments;
    [Export] private Array<CurvedRoadSegmentOffset> segmentOffsets;


    // FUNCTIONS //
    // Controlling
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
        if (index == 0) SetNewStart(value);
        else if (index == 1) SetNewControl(value);
        else if (index == 2) SetNewEnd(value);
    }

    private void SaveSegmentOffsets()
    {
        // Creates a new list for segment offsets and segments
        segments = new Array<NavSegment>(Simplifications.GetChildrenOfType<NavSegment>(this, true));
        segmentOffsets = new Array<CurvedRoadSegmentOffset>();

        for (int i = 0; i < segments.Count; i++)
        {
            segmentOffsets.Add(CurvedRoadSegmentOffset.GetSegmentOffset(i, this));
        }
    }

    private void SetNewStart(Vector3 newValue)
    {
        _start = newValue;
        RecalculateStartPoints();
        RecalculateControlPoints();

        UpdateMesh();
    }

    private void SetNewControl(Vector3 newValue)
    {
        _control = newValue;
        RecalculateControlPoints();
        RecalculateEndPoints();
        RecalculateStartPoints();

        UpdateMesh();
    }

    private void SetNewEnd(Vector3 newValue)
    {
        _end = newValue;
        RecalculateEndPoints();
        RecalculateControlPoints();

        UpdateMesh();
    }

    private void RecalculateEndPoints()
    {
        if (segmentOffsets != null)
        {
            // Recalculates all segment offsets using new endpoint
            foreach (CurvedRoadSegmentOffset offset in segmentOffsets)
            {
                GetSegment(offset.SegmentIndex).SetEndpoint(
                    offset.EndpointAtRoadEnd, 
                    LocalizeOffsetToSegment(GetSegment(offset.SegmentIndex), EndTransform, offset.RoadEndOffset)
                    );
            }
        }
    }

    private void RecalculateStartPoints()
    {
        // Recalculates all segment offsets using new endpoint
        if (segmentOffsets != null)
        {
            foreach (CurvedRoadSegmentOffset offset in segmentOffsets)
            {
                GetSegment(offset.SegmentIndex).SetEndpoint(
                    offset.EndpointAtRoadStart, 
                    LocalizeOffsetToSegment(GetSegment(offset.SegmentIndex), StartTransform, offset.RoadStartOffset)
                    );
            }
        }
    }

    private void RecalculateControlPoints()
    {
        // Sets all control points at intersection of lines from start to end
        if (segmentOffsets != null)
        {
            foreach (CurvedRoadSegmentOffset offset in segmentOffsets)
            {
                // Gets the control position: intersection of a line drawn through the start and end
                // of this segment
                Vector2 startV2 = Curves.Vec3RemoveHeight(
                    LocalizeSegmentPointToRoad(
                        GetSegment(offset.SegmentIndex), 
                        GetSegment(offset.SegmentIndex).Endpoints[offset.EndpointAtRoadStart]
                        )
                    );
                Vector2 endV2 = Curves.Vec3RemoveHeight(
                    LocalizeSegmentPointToRoad(
                        GetSegment(offset.SegmentIndex), 
                        GetSegment(offset.SegmentIndex).Endpoints[offset.EndpointAtRoadEnd]
                        )
                    );

                Vector2 startDirection = Curves.Vec3RemoveHeight(Control) - Curves.Vec3RemoveHeight(Start);
                Vector2 endDirection = Curves.Vec3RemoveHeight(Control) - Curves.Vec3RemoveHeight(End);

                Variant intersection = Geometry2D.LineIntersectsLine(startV2, startDirection, endV2, endDirection);

                if (intersection.VariantType != Variant.Type.Nil)
                {
                    GetSegment(offset.SegmentIndex).Control = LocalizeRoadPointToSegment(
                        GetSegment(offset.SegmentIndex), 
                        Curves.Vec2WithHeight(intersection.AsVector2(), 
                        Start.Y)
                        );
                }
            }
        }
    }

    private Vector3 LocalizeOffsetToSegment(NavSegment segment, Transform3D transform, Vector3 offset)
    {
        return segment.ToLocal(ToGlobal(transform * offset));
    }

    private Vector3 LocalizeSegmentPointToRoad(NavSegment segment, Vector3 point)
    {
        return ToLocal(segment.ToGlobal(point));
    }

    private Vector3 LocalizeRoadPointToSegment(NavSegment segment, Vector3 point)
    {
        return segment.ToLocal(ToGlobal(point));
    }


    // Data Retrieval
    public NavSegment GetSegment(int index)
    {
        return segments[index];
    }

    // Visualization
    public void UpdateMesh()
    {
        // Runs regardless of editor
        if (roadMesh != null && meshRenderer != null)
        {
            meshRenderer.Mesh = roadMesh.GenerateRoadMesh(this);
        }

        if (debugRenderer != null && roadMesh != null && debugNormals)
        {
            debugRenderer.Mesh = roadMesh.GenerateRoadNormalsMesh(this);
        }
    }
}
