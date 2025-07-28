#if TOOLS
using Godot;
using System;

[Tool]
public partial class NavGraphGizmos : EditorPlugin
{
    private NavSegmentGizmo _navSegmentGizmos;
    private CurvedRoadGizmo _curvedRoadGizmos;
    private BezierCurveNavSegmentGizmo _bezierCurveNavSegmentGizmos;
    private NavCheckpointGizmo _navCheckpointGizmos;

    public override void _EnterTree()
	{
        _navSegmentGizmos = new NavSegmentGizmo();
        _curvedRoadGizmos = new CurvedRoadGizmo();
        _bezierCurveNavSegmentGizmos = new BezierCurveNavSegmentGizmo();
        _navCheckpointGizmos = new NavCheckpointGizmo();
        AddNode3DGizmoPlugin(_navSegmentGizmos);
        AddNode3DGizmoPlugin(_curvedRoadGizmos);
        AddNode3DGizmoPlugin(_bezierCurveNavSegmentGizmos);
        AddNode3DGizmoPlugin(_navCheckpointGizmos);
	}

	public override void _ExitTree()
	{
        RemoveNode3DGizmoPlugin( _navSegmentGizmos);
        RemoveNode3DGizmoPlugin(_curvedRoadGizmos);
        RemoveNode3DGizmoPlugin(_bezierCurveNavSegmentGizmos);
        RemoveNode3DGizmoPlugin(_navCheckpointGizmos);

    }
}
#endif
