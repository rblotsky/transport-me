using Godot;
using System;

[GlobalClass]
public partial class BezierGenerator : Node
{
    [Export] private Vector2 startPoint;
    [Export] private Vector2 endPoint;
    [Export] private Vector2 controlPoint;
    [Export] private float height;

    [Export] private MeshInstance3D pathTraverser;
    private float pathTime;
    private MeshInstance3D startMesh;
    private MeshInstance3D endMesh;
    private MeshInstance3D controlMesh;


    public override void _Ready()
    {
        startMesh = EasyShapes.AddGenericShapeMesh(this, 
            EasyShapes.CreateSphereMesh(0.2f, EasyShapes.CreateMaterial(Colors.Green, 1))
            );
        endMesh = EasyShapes.AddGenericShapeMesh(this, 
            EasyShapes.CreateSphereMesh(0.2f, EasyShapes.CreateMaterial(Colors.Red, 1))
            );
        controlMesh = EasyShapes.AddGenericShapeMesh(
            this, EasyShapes.CreateSphereMesh(0.2f, EasyShapes.CreateMaterial(Colors.Blue, 1))
            );

        startMesh.GlobalPosition = new Vector3(startPoint.X, height, startPoint.Y);
        endMesh.GlobalPosition = new Vector3(endPoint.X, height, endPoint.Y);
        controlMesh.GlobalPosition = new Vector3(controlPoint.X, height, controlPoint.Y);

    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if(pathTime > 0.999 && pathTime < 1.1)
        {
            pathTime = 0;
        }

        pathTime += (float)delta;
        Vector2 bezierResult = QuadraticBezier(startPoint, controlPoint, endPoint, pathTime);
        pathTraverser.GlobalPosition = new Vector3(bezierResult.X, height, bezierResult.Y);
    }

    private Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);

        Vector2 r = q0.Lerp(q1, t);
        return r;
    }
}
