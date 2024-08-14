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
        // Updates the start of all children by difference
        Vector3 diff = newValue - _start;

        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            // Decides if the segments start or end is closer to the road's start, then moves that.
            // This is because segments might be forwards or backwards relative to the road.
            float startDist = (segment.Start - Start).Length();
            float endDist = (segment.End - Start).Length();

            if(startDist < endDist)
            {
                segment.Start += diff;
            }
            else
            {
                segment.End += diff;
            }
        }

        _start = newValue;
        UpdateVisualization();
    }

    private void SetNewControl(Vector3 newValue)
    {
        // Updates the start of all children by difference
        Vector3 diff = newValue - _control;

        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            segment.Control += diff;
        }

        _control = newValue;
        UpdateVisualization();
    }

    private void SetNewEnd(Vector3 newValue)
    {
        // Updates the start of all children by difference
        Vector3 diff = newValue - _end;

        foreach (NavSegment segment in Simplifications.GetChildrenOfType<NavSegment>(this))
        {
            // Decides if the segments start or end is closer to the road's end, then moves that.
            // This is because segments might be forwards or backwards relative to the road.
            float startDist = (segment.Start - End).Length();
            float endDist = (segment.End - End).Length();

            if (startDist < endDist)
            {
                segment.Start += diff;
            }
            else
            {
                segment.End += diff;
            }
        }

        _end = newValue;
        UpdateVisualization();
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
