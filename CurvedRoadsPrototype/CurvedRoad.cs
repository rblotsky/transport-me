using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
[Tool]
public partial class CurvedRoad : Node3D
{
    // DATA //
    // Graph data
    [ExportCategory("Road Shape Data")]
    private Vector3 _start = Vector3.Zero;
    [Export] public Vector3 Start { get { return _start; } set { SetNewStart(value); } }
    private Vector3 _end = Vector3.Zero;
    [Export] public Vector3 End { get { return _end; } set { SetNewEnd(value); } }
    private Vector2 _control = Vector2.Zero;
    [Export] public Vector2 Control { get { return _control; } set { SetNewControl(value); } }

    // Properties
    public Vector3 ControlAtStartHeight { get { return Curves.Vec2WithHeight(Control, Start.Y); } }
    public Vector3 ControlAtEndHeight { get { return Curves.Vec2WithHeight(Control, End.Y); } }
    public Vector3 ControlAtAvgHeight { get { return Curves.Vec2WithHeight(Control, (Start.Y + End.Y) / 2); } }

    /// <summary>
    /// A transform of the Start. Origin = Start, Basis = facing Control from Start
    /// </summary>
    public Transform3D StartTransform { get { return new Transform3D(Basis.LookingAt(Start - ControlAtStartHeight), Start); } }

    /// <summary>
    /// A transform of the End. Origin = End, Basis = facing End from Control
    /// </summary>
    public Transform3D EndTransform { get { return new Transform3D(Basis.LookingAt(ControlAtEndHeight - End), End); } }


    // Runtime Cached Data
    private List<CurvedRoadSegmentOffset> segmentOffsets;

    // Editor Cached Data
    private MeshInstance3D startVisualizer;
    private MeshInstance3D controlVisualizer;
    private MeshInstance3D endVisualizer;
    private MeshInstance3D curveVisualizer;


    // FUNCTIONS //

    // Godot Defaults
    public override void _Ready()
    {
        // In editor, run visualization
        if (Engine.IsEditorHint())
        {
            UpdateVisualization();
        }

        // Save segment offsets
        segmentOffsets = new List<CurvedRoadSegmentOffset>();
        foreach (NavSegment childSegment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            segmentOffsets.Add(CurvedRoadSegmentOffset.GetSegmentOffset(childSegment, this));
        }

        base._Ready();
    }

    public override void _ExitTree()
    {
        // In editor, clear visualization and offset data
        if (Engine.IsEditorHint())
        {
            RemoveVisualizers();
        }

        // Clear segment offsets
        segmentOffsets = null;

        RequestReady();

        base._ExitTree();
    }


    // Controlling
    private void SetNewStart(Vector3 newValue)
    {
        _start = newValue;
        RecalculateStartPoints();
        RecalculateControlPoints();
        UpdateVisualization();
    }

    private void SetNewControl(Vector2 newValue)
    {
        _control = newValue;
        RecalculateControlPoints();
        RecalculateEndPoints();
        RecalculateStartPoints();
        UpdateVisualization();
    }

    private void SetNewEnd(Vector3 newValue)
    {
        _end = newValue;
        RecalculateEndPoints();
        RecalculateControlPoints();
        UpdateVisualization();
    }

    private void RecalculateEndPoints()
    {
        if (segmentOffsets != null)
        {
            // Recalculates all segment offsets using new endpoint
            foreach (CurvedRoadSegmentOffset offset in segmentOffsets)
            {
                offset.Segment.SetEndpoint(offset.EndpointAtRoadEnd, LocalizeOffsetToSegment(offset.Segment, EndTransform, offset.RoadEndOffset));
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
                offset.Segment.SetEndpoint(offset.EndpointAtRoadStart, LocalizeOffsetToSegment(offset.Segment, StartTransform, offset.RoadStartOffset));
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
                Vector2 startV2 = Curves.Vec3RemoveHeight(LocalizeSegmentPointToRoad(offset.Segment, offset.Segment.Endpoints[offset.EndpointAtRoadStart]));
                Vector2 endV2 = Curves.Vec3RemoveHeight(LocalizeSegmentPointToRoad(offset.Segment, offset.Segment.Endpoints[offset.EndpointAtRoadEnd]));

                Vector2 startDirection = Control - Curves.Vec3RemoveHeight(Start);
                Vector2 endDirection = Control - Curves.Vec3RemoveHeight(End);

                Variant intersection = Geometry2D.LineIntersectsLine(startV2, startDirection, endV2, endDirection);

                if (intersection.VariantType != Variant.Type.Nil)
                {
                    offset.Segment.Control = LocalizeRoadPointToSegment(offset.Segment, Curves.Vec2WithHeight(intersection.AsVector2(), Start.Y));
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


    // Visualization
    private void RemoveVisualizers()
    {
        if (startVisualizer != null)
        {
            startVisualizer.Free();
            startVisualizer = null;
        }

        if (controlVisualizer != null)
        {
            controlVisualizer.Free();
            controlVisualizer = null;
        }

        if (endVisualizer != null)
        {
            endVisualizer.Free();
            endVisualizer = null;
        }

        if (curveVisualizer != null)
        {
            curveVisualizer.Free();
            curveVisualizer = null;
        }
    }

    private void UpdateVisualization()
    {
        // Only runs in editor
        if (Engine.IsEditorHint() && IsNodeReady())
        {
            // Removes and creates new visualizers
            RemoveVisualizers();
            startVisualizer = new MeshInstance3D();
            AddChild(startVisualizer);
            controlVisualizer = new MeshInstance3D();
            AddChild(controlVisualizer);
            endVisualizer = new MeshInstance3D();
            AddChild(endVisualizer);
            curveVisualizer = new MeshInstance3D();
            AddChild(curveVisualizer);

            startVisualizer.Position = Start;
            startVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.Green, 0.5f));

            controlVisualizer.Position = ControlAtAvgHeight;
            controlVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.Gray, 0.5f));

            endVisualizer.Position = End;
            endVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.Red, 0.5f));

            curveVisualizer.Mesh = EasyShapes.CurveMesh(Start, End, ControlAtAvgHeight, Colors.Gray, 10);

        }
    }
}
