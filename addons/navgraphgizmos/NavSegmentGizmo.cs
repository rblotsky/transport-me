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
    private Color defaultColour = Colors.Blue;
    private Color teleportColour = Colors.DarkBlue;
    private int defaultLineSubdivisions = 9;
    private int teleportLineSubdivisions = 20;

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
        Color lineColour = defaultColour;
        Color arrowColour = defaultColour;
        int lineSubdivisions = defaultLineSubdivisions;
        bool isLineDashed = false;

        // Changes the display a bit if the segment is a teleport segment
        if (node.Teleport)
        {
            lineSubdivisions = teleportLineSubdivisions;
            isLineDashed = true;
            lineColour = teleportColour;
            arrowColour = teleportColour;
        }

        // Adds the actual visualization
        ImmediateMesh curveMesh = EasyShapes.OrderedLinesMesh(node.SubdivideIntoPoints(lineSubdivisions), lineColour, isLineDashed);
        Mesh arrowMesh = EasyShapes.TrianglePointerMesh(arrowColour, 0.15f);
        gizmo.AddMesh(curveMesh);
        gizmo.AddMesh(arrowMesh, null, new Transform3D(Basis.LookingAt(node.DirectionalLine, Vector3.Up), node.GetPositionOnSegment(0.5f, false)));

        gizmo.AddCollisionSegments((Vector3[])curveMesh.SurfaceGetArrays(0)[0]);
        gizmo.AddCollisionTriangles(arrowMesh.GenerateTriangleMesh());
    }

}