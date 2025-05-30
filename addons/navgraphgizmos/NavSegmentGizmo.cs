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

        Vector3[] lines = EasyShapes.CurveMesh(node.Start, node.End, node.Control, Colors.Red, 9).Get;


        Vector3[] handles = new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 2, 0) };



        gizmo.AddLines(lines, EasyShapes.ColouredMaterial(Colors.Red, 1), false);

        gizmo.AddHandles(handles, EasyShapes.ColouredMaterial(Colors.Blue, 1), []);
    }

}
