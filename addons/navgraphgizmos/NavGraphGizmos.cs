#if TOOLS
using Godot;
using System;

[Tool]
public partial class NavGraphGizmos : EditorPlugin
{
    private NavSegmentGizmo _navSegmentGizmos;
    private CurvedRoadGizmo _curvedRoadGizmos;
	public override void _EnterTree()
	{
        _navSegmentGizmos = new NavSegmentGizmo();
        _curvedRoadGizmos = new CurvedRoadGizmo();
        AddNode3DGizmoPlugin(_navSegmentGizmos);
        AddNode3DGizmoPlugin(_curvedRoadGizmos);
	}

	public override void _ExitTree()
	{
        RemoveNode3DGizmoPlugin( _navSegmentGizmos);
        RemoveNode3DGizmoPlugin(_curvedRoadGizmos);
	}
}
#endif
