using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

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
        //TODO: Update so it doesn't run on CurvedRoadNavSegment
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
        ImmediateMesh curveMesh = node.GetCurveVisualization(lineColour, 9);
        Mesh arrowMesh = EasyShapes.TrianglePointerMesh(arrowColour, 0.15f);
        gizmo.AddMesh(curveMesh);
        gizmo.AddMesh(arrowMesh, null, new Transform3D(Basis.LookingAt(node.DirectionalLine, Vector3.Up), node.GetPositionOnSegment(0.5f, false)));


        gizmo.AddCollisionSegments(((Vector3[])curveMesh.SurfaceGetArrays(0)[0]));
        gizmo.AddCollisionTriangles(arrowMesh.GenerateTriangleMesh());

        if (node is not CurvedRoadNavSegment)
        {
            // Adds handles to modify the visualization
            Vector3[] handles = new Vector3[3];
            handles[NavSegment.StartPointIndex] = node.Start;
            handles[NavSegment.ControlPointIndex] = node.Control;
            handles[NavSegment.EndPointIndex] = node.End;

            gizmo.AddHandles(handles, EasyShapes.GizmoHandleMaterial(Colors.Red), [0, 1, 2], false);
        }
    }


    public override string _GetHandleName(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        string[] names = { "Start", "Control", "End" };
        return names[handleId];
    }

    public override Variant _GetHandleValue(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        NavSegment node = (NavSegment)gizmo.GetNode3D();
        return node.GetPointByIndex(handleId);
    }

    public override void _SetHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary, Camera3D camera, Vector2 screenPos)
    {
        NavSegment node = (NavSegment)gizmo.GetNode3D();
        float pointHeight = node.GetPointByIndex(handleId).Y;
        Plane placementPlane = new Plane(Vector3.Up, pointHeight);
        Vector3? mousePosWorld =
            placementPlane.IntersectsRay(
            camera.ProjectRayOrigin(screenPos),
            camera.ProjectRayNormal(screenPos));

        // Returns the placement point (at the same height as it currently is) or the current position if mouse position wasn't found
        Vector3 newPosition = node.GetPointByIndex(handleId);
        if (mousePosWorld != null)
        {
            newPosition =  mousePosWorld.Value;
        }

        if (Input.IsKeyPressed(Key.Ctrl))
        {
            newPosition = newPosition.Snapped(new Vector3(1, 1, 1));
        }
        node.SetPointByIndex(handleId, newPosition);
        node.UpdateGizmos();
    }

    public override void _CommitHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary, Variant restore, bool cancel)
    {
        NavSegment node = (NavSegment)gizmo.GetNode3D();

        Vector3[] values = { node.Start, node.Control, node.End };
        string[] propertyNames = { "Start", "Control", "End" };

        if (cancel)
        {
            node.Set(propertyNames[handleId], restore);
        }
        else
        {
            EditorUndoRedoManager undoRedoManager = EditorInterface.Singleton.GetEditorUndoRedo();
            undoRedoManager.CreateAction($"Move NavSegment {propertyNames[handleId]}");
            undoRedoManager.AddDoProperty(node, propertyNames[handleId], values[handleId]);
            undoRedoManager.AddDoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.AddUndoProperty(node, propertyNames[handleId], restore);
            undoRedoManager.AddUndoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.CommitAction(true);
        }
    }

}