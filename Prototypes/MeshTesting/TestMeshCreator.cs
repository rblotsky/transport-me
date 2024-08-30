using Godot;
using System;

[GlobalClass]
[Tool]
public partial class TestMeshCreator : Node3D
{
    // DATA //
    [Export] private MeshInstance3D renderer;
    private int _segments = 3;
    [Export] private int Segments { get { return _segments; } set { _segments = value; HandleChange(); } }
    private float _innerRadius = 1;
    [Export] private float InnerRadius { get { return _innerRadius; } set { _innerRadius = value; HandleChange(); } }
    private float _outerRadius;
    [Export] private float OuterRadius { get { return _outerRadius; } set { _outerRadius = value; HandleChange(); } }

    // Cached Data
    private ArrayMesh generatedMesh;


    // FUNCTIONS //
    // Mesh Creation
    public void HandleChange()
    {
        if(renderer != null)
        {
            CreateNewMesh();
        }
    }

    public void CreateNewMesh()
    {
        // Decides amount to rotate for each point
        float rotationPerPoint = Mathf.Tau / (float)Segments;

        // Creates a surface
        SurfaceTool surface = new SurfaceTool();
        surface.Begin(Mesh.PrimitiveType.TriangleStrip);

        for(int i = 0; i < Segments; i++)
        {
            Vector3 placementDirection = (Transform.Basis.Z).Rotated(Vector3.Up, -rotationPerPoint*i);
            surface.AddVertex(placementDirection * InnerRadius);
            surface.AddVertex(placementDirection * (OuterRadius + InnerRadius));
        }

        // Adds the last points to close the donut
        Vector3 finalPlacementDirection = (Transform.Basis.Z);
        surface.AddVertex(finalPlacementDirection * InnerRadius);
        surface.AddVertex(finalPlacementDirection * (OuterRadius + InnerRadius));

        // Sets the renderer to this mesh
        generatedMesh = surface.Commit();
        renderer.Mesh = generatedMesh;
    }
}
