#if TOOLS
using Godot;
using System;

[Tool]
public partial class NavGraphGizmos : EditorPlugin
{
    private NavSegmentGizmo _gizmoPlugin;
	public override void _EnterTree()
	{
        _gizmoPlugin = new NavSegmentGizmo();
        AddNode3DGizmoPlugin(_gizmoPlugin);
	}

	public override void _ExitTree()
	{
        RemoveNode3DGizmoPlugin( new NavSegmentGizmo());
	}
}
#endif
