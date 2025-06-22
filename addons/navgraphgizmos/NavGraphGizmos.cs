#if TOOLS
using Godot;
using System;

[Tool]
public partial class NavGraphGizmos : EditorPlugin
{
    private NavSegmentGizmo _navSegmentGizmos;
    private CurvedRoadGizmo _curvedRoadGizmos;
    private BezierCurveNavSegmentGizmo _bezierCurveNavSegmentGizmos;

	public override void _EnterTree()
	{
        _navSegmentGizmos = new NavSegmentGizmo();
        _curvedRoadGizmos = new CurvedRoadGizmo();
        _bezierCurveNavSegmentGizmos = new BezierCurveNavSegmentGizmo();
        AddNode3DGizmoPlugin(_navSegmentGizmos);
        AddNode3DGizmoPlugin(_curvedRoadGizmos);
        AddNode3DGizmoPlugin(_bezierCurveNavSegmentGizmos);
	}

	public override void _ExitTree()
	{
        RemoveNode3DGizmoPlugin( _navSegmentGizmos);
        RemoveNode3DGizmoPlugin(_curvedRoadGizmos);
        RemoveNode3DGizmoPlugin(_bezierCurveNavSegmentGizmos);
	}
}
#endif
