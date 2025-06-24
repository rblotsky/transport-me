using Godot;
using System;

public partial class CurvedRoadGizmo : EditorNode3DGizmoPlugin
{
    private Color handleColour = Colors.Red;

    public override string _GetGizmoName()
    {
        return "Curved Road Gizmo";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is CurvedRoad;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        gizmo.Clear();
        CurvedRoad node = (CurvedRoad)gizmo.GetNode3D();

        // Adds the actual visualization
        ImmediateMesh curveMesh = EasyShapes.CurveMesh(node.Start, node.End, node.Control, Colors.Green, 9);
        Mesh arrowMesh = EasyShapes.TrianglePointerMesh(Colors.Green, 0.15f);
        gizmo.AddMesh(curveMesh);
        gizmo.AddMesh(arrowMesh, null, new Transform3D(Basis.LookingAt(node.DirectionalLine, Vector3.Up), node.Start));

        gizmo.AddCollisionSegments(((Vector3[])curveMesh.SurfaceGetArrays(0)[0]));
        gizmo.AddCollisionTriangles(arrowMesh.GenerateTriangleMesh());

        // Regenerates the road mesh
        Mesh renderMesh = node.GetDisplayMesh();
        gizmo.AddMesh(renderMesh);
        gizmo.AddCollisionTriangles(renderMesh.GenerateTriangleMesh());

        // Adds handles to modify the visualization
        Vector3[] handles = {node.Start, node.Control, node.End};

        gizmo.AddHandles(handles, EasyShapes.GizmoHandleMaterial(handleColour), [0, 1, 2], false);
        
        foreach(NavSegment navSegment in Simplifications.GetChildrenOfType<NavSegment>(node))
        {
            navSegment.UpdateGizmos();
        }
    }


    public override string _GetHandleName(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        string[] names = { "Start", "Control", "End" };
        return names[handleId];
    }

    public override Variant _GetHandleValue(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        CurvedRoad node = (CurvedRoad)gizmo.GetNode3D();
        return node.GetPointByIndex(handleId);
    }

    public override void _SetHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary, Camera3D camera, Vector2 screenPos)
    {
        CurvedRoad node = (CurvedRoad)gizmo.GetNode3D();
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
        node.SetPointByIndex(handleId, node.ToLocal(newPosition));
        node.UpdateGizmos();
    }

    public override void _CommitHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary, Variant restore, bool cancel)
    {
        CurvedRoad node = (CurvedRoad)gizmo.GetNode3D();

        Vector3[] values = { node.Start, node.Control, node.End };
        string[] propertyNames = { "Start", "Control", "End" };

        if (cancel)
        {
            node.Set(propertyNames[handleId], restore);
        }
        else
        {
            EditorUndoRedoManager undoRedoManager = EditorInterface.Singleton.GetEditorUndoRedo();
            undoRedoManager.CreateAction($"Move CurvedRoad {propertyNames[handleId]}");
            undoRedoManager.AddDoProperty(node, propertyNames[handleId], values[handleId]);
            undoRedoManager.AddDoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.AddUndoProperty(node, propertyNames[handleId], restore);
            undoRedoManager.AddUndoMethod(this, MethodName._Redraw, gizmo);
            undoRedoManager.CommitAction(true);
        }
    }
}
