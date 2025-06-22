using Godot;
using System;

/// <summary>
/// Adds editable handles to all BezierCurveNavSegments.
/// </summary>
public partial class BezierCurveNavSegmentGizmo : EditorNode3DGizmoPlugin
{

    public override string _GetGizmoName()
    {
        return "BezierCurveNavSegment Editor";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        //TODO: Update so it doesn't run on CurvedRoadNavSegment
        return forNode3D is BezierCurveNavSegment;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        gizmo.Clear();
        BezierCurveNavSegment node = (BezierCurveNavSegment)gizmo.GetNode3D();

        // Adds handles to modify the visualization
        Vector3[] handles = new Vector3[3];
        handles[BezierCurveNavSegment.StartPointIndex] = node.CurveStart;
        handles[BezierCurveNavSegment.ControlPointIndex] = node.CurveControl;
        handles[BezierCurveNavSegment.EndPointIndex] = node.CurveEnd;

        gizmo.AddHandles(handles, EasyShapes.GizmoHandleMaterial(Colors.Red), [0, 1, 2], false);
    }


    public override string _GetHandleName(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        string[] names = { "CurveStart", "CurveControl", "CurveEnd" };
        return names[handleId];
    }

    public override Variant _GetHandleValue(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        BezierCurveNavSegment node = (BezierCurveNavSegment)gizmo.GetNode3D();
        return node.GetPointByIndex(handleId);
    }

    public override void _SetHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary, Camera3D camera, Vector2 screenPos)
    {
        BezierCurveNavSegment node = (BezierCurveNavSegment)gizmo.GetNode3D();
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
            newPosition = mousePosWorld.Value;
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
        BezierCurveNavSegment node = (BezierCurveNavSegment)gizmo.GetNode3D();

        Vector3[] values = { node.CurveStart, node.CurveControl, node.CurveEnd };
        string[] propertyNames = { "CurveStart", "CurveControl", "CurveEnd" };

        if (cancel)
        {
            node.Set(propertyNames[handleId], restore);
        }
        else
        {
            EditorUndoRedoManager undoRedoManager = EditorInterface.Singleton.GetEditorUndoRedo();
            undoRedoManager.CreateAction($"Move BezierCurveNavSegment {propertyNames[handleId]}");
            undoRedoManager.AddDoProperty(node, propertyNames[handleId], values[handleId]);
            undoRedoManager.AddDoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.AddUndoProperty(node, propertyNames[handleId], restore);
            undoRedoManager.AddUndoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.CommitAction(true);
        }
    }

}
