using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

/// <summary>
/// Adds a selectable visualization to all NavSegments.
/// </summary>
[Tool]
public partial class NavSegmentGizmo : EditorNode3DGizmoPlugin
{
    private Color defaultLineColour = Colors.Blue;
    private Color selectedLineColour = Colors.Blue;
    private Color defaultArrowColour = Colors.Blue;
    private Color selectedArrowColour = Colors.Blue;

    public override string _GetGizmoName()
    {
        return "NavSegment Gizmo";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is NavSegment;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        gizmo.Clear();
        NavSegment node = (NavSegment)gizmo.GetNode3D();

        // Decides colour to use based on whether gizmo is selected
        Color lineColour = defaultLineColour;
        Color arrowColour = defaultArrowColour;

        if(EditorInterface.Singleton.GetSelection().GetSelectedNodes().Contains(node))
        {
            lineColour = selectedLineColour;
            arrowColour = selectedArrowColour;
        }

        // Adds the actual visualization
        ImmediateMesh curveMesh = EasyShapes.OrderedLinesMesh(node.SubdivideIntoPoints(9), lineColour);
        Mesh arrowMesh = EasyShapes.TrianglePointerMesh(arrowColour, 0.15f);
        gizmo.AddMesh(curveMesh);
        gizmo.AddMesh(arrowMesh, null, new Transform3D(Basis.LookingAt(node.DirectionalLine, Vector3.Up), node.GetPositionOnSegment(0.5f, false)));

        gizmo.AddCollisionSegments((Vector3[])curveMesh.SurfaceGetArrays(0)[0]);
        gizmo.AddCollisionTriangles(arrowMesh.GenerateTriangleMesh());
    }

}