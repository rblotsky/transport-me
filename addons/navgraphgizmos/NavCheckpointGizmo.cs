using Godot;
using System;

public partial class NavCheckpointGizmo : EditorNode3DGizmoPlugin
{
    public override string _GetGizmoName()
    {
        return "NavCheckpoint Gizmo";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is NavCheckpoint;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        gizmo.Clear();
        NavCheckpoint node = (NavCheckpoint)gizmo.GetNode3D();

        // Adds the actual visualization
        SphereMesh sphereMesh = EasyShapes.SphereMesh(0.2f, EasyShapes.ColouredMaterial(Colors.Green, 0.5f));
        gizmo.AddMesh(sphereMesh);

        gizmo.AddCollisionTriangles(sphereMesh.GenerateTriangleMesh());
    }
}
