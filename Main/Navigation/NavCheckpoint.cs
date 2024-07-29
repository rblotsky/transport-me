using Godot;
using System;

[GlobalClass]
[Tool]
public partial class NavCheckpoint : Node3D
{
    // DATA //
    
    // Readonly Properties
    public Vector3I GlobalSnappedPos { get { return Simplifications.SnapV3ToGrid(GlobalPosition); } }
    public Vector3I LocalSnappedPos { get { return Simplifications.SnapV3ToGrid(Position); } }

    // Editor Cached Data
    private MeshInstance3D renderMesh;


    // FUNCTIONS //
    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
        {
            UpdateVisualization();
        }
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
        {
            DeleteVisualization();
        }
        base._ExitTree();
    }

    private void UpdateVisualization()
    {
        DeleteVisualization();
        renderMesh = new MeshInstance3D();
        AddChild(renderMesh);
        renderMesh.Mesh = EasyShapes.SphereMesh(0.3f, EasyShapes.ColouredMaterial(Colors.Orange, 0.3f));
    }

    private void DeleteVisualization()
    {
        if(renderMesh != null)
        {
            renderMesh.Free();
            renderMesh = null;
        }
    }
}
