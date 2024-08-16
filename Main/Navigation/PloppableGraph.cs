using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;

[GlobalClass]
[Tool]
public partial class PloppableGraph : Node3D
{
    // DATA //
    // Graph data
    [ExportCategory("Road Shape Data")]
    private Vector3 _start = Vector3.Zero;
    [Export] private Vector3 Start { get { return _start; } set { SetNewStart(value); } }
    private Vector3 _end = Vector3.Zero;
    [Export] private Vector3 End { get { return _end; } set { SetNewEnd(value); } }
    private Vector3 _control = Vector3.Zero;
    [Export] private Vector3 Control { get { return _control; } set { SetNewControl(value); } }

    // Editor Cached Data
    private MeshInstance3D startVisualizer;
    private MeshInstance3D controlVisualizer;
    private MeshInstance3D endVisualizer;
    private MeshInstance3D curveVisualizer;

    // TODO CHANGE CONSTANTS TO MODIFIABLE VALUES
    private float segmentOffset = 2;


    // FUNCTIONS //

    // Godot Defaults
    public override void _EnterTree()
    {
        // In editor, run visualization
        if (Engine.IsEditorHint())
        {
            UpdateVisualization();
        }
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            RemoveVisualizers();
        }

        base._ExitTree();
    }

    
    // Controlling
    private void SetNewStart(Vector3 newValue)
    {
        // Updates the start of all children by adding their vectors to the new position, but rotated
        // to face the correct direction
        Vector3 oldFacing = new Vector3(Control.X, Start.Y, Control.Z) - Start;
        Vector3 newFacing = new Vector3(Control.X, newValue.Y, Control.Z) - newValue;
        
        float angleToNew = oldFacing.SignedAngleTo(newFacing, Vector3.Up);
        
        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            // Decides if the segments start or end is closer to the road's start, then moves that.
            // This is because segments might be forwards or backwards relative to the road.
            Vector3 vectorToStart = segment.Start - Start;
            Vector3 vectorToEnd = segment.End - Start;

            float startDist = vectorToStart.LengthSquared();
            float endDist = vectorToEnd.LengthSquared();

            if (startDist < endDist)
            {
                segment.Start = newFacing.Normalized().Rotated(Vector3.Up, Mathf.Pi / 2) *segmentOffset + newValue;
            }
            else
            {
                segment.End = newFacing.Normalized().Rotated(Vector3.Up, -Mathf.Pi / 2) * segmentOffset + newValue;
            }
        }

        _start = newValue;
        RecalculateControlPoints();
        UpdateVisualization();
    }

    private void SetNewControl(Vector3 newValue)
    {
        _control = newValue;
        RecalculateControlPoints();
        UpdateVisualization();
    }

    private void SetNewEnd(Vector3 newValue)
    {
        // Updates the end of all children by adding their vectors to the new position, but rotated
        // to face the correct direction
        Vector3 oldFacing = new Vector3(Control.X, End.Y, Control.Z) - End;
        Vector3 newFacing = new Vector3(Control.X, newValue.Y, Control.Z) - newValue;

        float angleToNew = oldFacing.SignedAngleTo(newFacing, Vector3.Up);

        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            // Decides if the segments start or end is closer to the road's end, then moves that.
            // This is because segments might be forwards or backwards relative to the road.
            Vector3 vectorToStart = segment.Start - End;
            Vector3 vectorToEnd = segment.End - End;

            float startDist = vectorToStart.LengthSquared();
            float endDist = vectorToEnd.LengthSquared();

            if (startDist < endDist)
            {
                segment.Start = newFacing.Normalized().Rotated(Vector3.Up, Mathf.Pi / 2) *segmentOffset + newValue;
            }
            else
            {
                segment.End = newFacing.Normalized().Rotated(Vector3.Up, -Mathf.Pi/2) * segmentOffset + newValue;
            }
        }

        

        _end = newValue;
        RecalculateControlPoints();
        UpdateVisualization();
    }

    private void RecalculateEndPoints(Vector3 old, Vector3 newPoint)
    {

    }

    private void RecalculateStartPoints(Vector3 old, Vector3 newPoint)
    {

    }

    private void RecalculateControlPoints()
    {
        // Puts control points on a straight line perpendicular to the path from
        // start to end
        Vector3 overallDirection = (End - Start).Normalized();

        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            // Decides if the segments start or end is closer to the road's end, then moves that.
            // This is because segments might be forwards or backwards relative to the road.
            Vector3 vectorToStart = segment.Start - End;
            Vector3 vectorToEnd = segment.End - End;

            float startDist = vectorToStart.LengthSquared();
            float endDist = vectorToEnd.LengthSquared();

            if (startDist < endDist)
            {
                segment.Control = overallDirection.Rotated(Vector3.Up, -Mathf.Pi / 2) * segmentOffset + Control;
            }
            else
            {
                segment.Control = overallDirection.Rotated(Vector3.Up, Mathf.Pi / 2) * segmentOffset + Control;
            }
            
        }
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
        if (Engine.IsEditorHint())
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

            controlVisualizer.Position = Control;
            controlVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.Gray, 0.5f));

            endVisualizer.Position = End;
            endVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.Red, 0.5f));

            curveVisualizer.Mesh = EasyShapes.CurveMesh(Start, End, Control, Colors.Gray, 10);

        }
    }

}
