using Godot;
using System;

[GlobalClass]
[Tool]
public partial class BezierGenerator : Node
{
    // DATA //
    [ExportCategory("Curve Data")]
    private Vector3 _startPoint;
    [Export] public Vector3 Start { get { return _startPoint; } set { _startPoint = value; UpdateVisualization(); } }
    private Vector3 _endPoint;
    [Export] public Vector3 End { get { return _endPoint; } set { _endPoint = value; UpdateVisualization(); } }
    private Vector3 _controlPoint;
    [Export] public Vector3 Control { get { return _controlPoint; } set { _controlPoint = value; UpdateVisualization(); } }
    private int _segments = 100;
    [Export(PropertyHint.Range, "0,200,")] public int Segments { get { return _segments; } set { _segments = value; UpdateVisualization(); } }

    [ExportCategory("Scene References")]
    [Export] private MeshInstance3D pathMesh;

    // Cached Data
    private float pathTime;
    private MeshInstance3D startMesh;
    private MeshInstance3D endMesh;
    private MeshInstance3D controlMesh;


    // FUNCTIONS //
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Ready()
    {
        if(!Engine.IsEditorHint())
        {
            UpdateVisualization();
        }
        base._Ready();
    }

    private void UpdateVisualization()
    {
        if (IsNodeReady())
        {
            if (startMesh == null)
            {
                startMesh = EasyShapes.AddShapeMesh(this,
                    EasyShapes.SphereMesh(0.2f, EasyShapes.ColouredMaterial(Colors.Green, 1))
                    );
            }

            if (endMesh == null)
            {
                endMesh = EasyShapes.AddShapeMesh(this,
                    EasyShapes.SphereMesh(0.2f, EasyShapes.ColouredMaterial(Colors.Red, 1))
                    );
            }

            if (controlMesh == null)
            {
                controlMesh = EasyShapes.AddShapeMesh(
                    this, EasyShapes.SphereMesh(0.2f, EasyShapes.ColouredMaterial(Colors.Blue, 1))
                    );
            }

            startMesh.GlobalPosition = Start;
            endMesh.GlobalPosition = End;
            controlMesh.GlobalPosition = Control;

            if (pathMesh != null)
            {
                pathMesh.Mesh = EasyShapes.CurveMesh(Start, End, Curves.Vec3RemoveHeight(Control), Colors.Red, Segments);
            }
        }
    }
}
