using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class NavSegmentGizmo : EditorNode3DGizmoPlugin
{
    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is NavSegment;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        gizmo.Clear();

        NavSegment node = (NavSegment)gizmo.GetNode3D();

        Vector3[] handles = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 2, 0) };

        gizmo.AddMesh(EasyShapes.CurveMesh(node.Start, node.End, Curves.Vec3RemoveHeight(node.Control), Colors.Red, 9));
        gizmo.AddMesh(EasyShapes.TrianglePointerMesh(Colors.Orange, 0.15f), null, new Transform3D(Basis.LookingAt(node.End), node.GetPositionOnSegment(0.5f, false)));
        gizmo.AddHandles(handles, EasyShapes.ColouredMaterial(Colors.Blue, 1), []);
    }

}
