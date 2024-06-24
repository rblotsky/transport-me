using Godot;
using System;

public static class Debugging
{
	// FUNCTIONS //
    public static void ColourNode(Node3D node, Color colourToUse)
    {
        MeshInstance3D mesh = Simplifications.GetFirstChildOfType<MeshInstance3D>(node);
        StandardMaterial3D colourMaterial = new StandardMaterial3D();
        colourMaterial.AlbedoColor = colourToUse;
        mesh.MaterialOverride = colourMaterial;
    }

    public static Node3D InstantiateCollisionMarkerAsChild(Node parent, Color colourToUse, float radius, Vector3 globalPos)
    {
        StandardMaterial3D colourMaterial = new StandardMaterial3D();
        colourMaterial.AlbedoColor = colourToUse;

        SphereMesh sphere = new SphereMesh();
        sphere.Radius = radius;
        sphere.Height = radius * 2.0f;
        sphere.Material = colourMaterial;

        MeshInstance3D newNode = new MeshInstance3D();
        newNode.Mesh = sphere;

        parent.AddChild(newNode);
        newNode.GlobalPosition = globalPos;
        return newNode;
    }
}
