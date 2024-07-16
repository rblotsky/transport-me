using Godot;
using System;

[GlobalClass]
[Tool]
public partial class BezierGenerator : Node
{
    // DATA //
    [ExportCategory("Curve Data")]
    private Vector2 _startPoint;
    [Export] public Vector2 Start { get { return _startPoint; } set { _startPoint = value; UpdateVisualization(); } }
    private Vector2 _endPoint;
    [Export] public Vector2 End { get { return _endPoint; } set { _endPoint = value; UpdateVisualization(); } }
    private Vector2 _controlPoint;
    [Export] public Vector2 Control { get { return _controlPoint; } set { _controlPoint = value; UpdateVisualization(); } }
    private float _height = 0;
    [Export] public float Height { get { return _height; } set { _height = value; UpdateVisualization(); } }
    private int _segments = 100;
    [Export(PropertyHint.Range, "0,200,")] public int Segments { get { return _segments; } set { _segments = value; UpdateVisualization(); } }

    [ExportCategory("Scene References")]
    [Export] private MeshInstance3D pathTraverser;
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

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint())
        {
            if (pathTime > 0.999 && pathTime < 1.1)
            {
                pathTime = 0;
            }

            pathTime += (float)delta;
            Vector2 bezierResult = QuadraticBezier(Start, Control, End, pathTime);
            pathTraverser.GlobalPosition = new Vector3(bezierResult.X, Height, bezierResult.Y);
        }
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

            startMesh.GlobalPosition = WithHeight(Start);
            endMesh.GlobalPosition = WithHeight(End);
            controlMesh.GlobalPosition = WithHeight(Control);

            if (pathMesh != null)
            {
                pathMesh.Mesh = EasyShapes.CurveMesh(WithHeight(Start), WithHeight(End), WithHeight(Control), Colors.Red, Segments);
            }
        }
    }

    private Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);

        Vector2 r = q0.Lerp(q1, t);
        return r;
    }

    private Vector3 WithHeight(Vector2 vec)
    {
        return new Vector3(vec.X, Height, vec.Y);
    }
}
