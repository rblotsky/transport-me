using Godot;
using System;

[GlobalClass]
[Tool]
public partial class NavSegment : Node3D
{
    // DATA
    // Serializable Properties
    private Vector3I _start = Vector3I.Zero;
    [Export] public Vector3I Start { get { return _start; } set { _start = value; UpdateVisualization(); } }
    private Vector3I _end = Vector3I.Zero;
    [Export] public Vector3I End { get { return _end; } set { _end = value; UpdateVisualization(); } }
    private Vector3I _control = Vector3I.Zero;
    [Export] public Vector3I Control { get { return _control; } set { _control = value; UpdateVisualization(); } }
    
    // Cached data in game
    private bool stopTraffic = false;
    
    // Properties
    public Vector3I[] Endpoints { get { return new Vector3I[2] { Start, End}; } }
    public Vector3 DirectionalLine { get { return End - Start; }}

    // Editor Cached Data
    private MeshInstance3D curveVisualizer;
    private MeshInstance3D endpointVisualizer;


    // FUNCTIONS //
    // Godot Defaults
    public override void _EnterTree()
    {
        // In editor, run visualization
        UpdateVisualization();
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        if(Engine.IsEditorHint())
        {
            RemoveVisualizers();
        }

        base._ExitTree();
    }


    // Data Retrieval
    public Vector3I GetOtherEnd(Vector3I oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return Vector3I.Zero ;
    }

    // Visualization
    private void RemoveVisualizers()
    {
        if (curveVisualizer != null)
        {
            curveVisualizer.Free();
            curveVisualizer = null;
        }

        if (endpointVisualizer != null)
        {
            endpointVisualizer.Free();
            endpointVisualizer = null;
        }
    }

    private void UpdateVisualization()
    {
        // Only runs in editor
        if(Engine.IsEditorHint())
        {
            // Removes and creates new visualizers
            RemoveVisualizers();
            curveVisualizer = new MeshInstance3D();
            AddChild(curveVisualizer);
            endpointVisualizer = new MeshInstance3D();
            AddChild(endpointVisualizer);

            curveVisualizer.Mesh = EasyShapes.CurveMesh(Start, End, Control, Colors.LightBlue, 10);
            endpointVisualizer.Position = End;
            endpointVisualizer.Mesh = EasyShapes.SphereMesh(0.1f, EasyShapes.ColouredMaterial(Colors.LightBlue, 1));
        }
    }
}
