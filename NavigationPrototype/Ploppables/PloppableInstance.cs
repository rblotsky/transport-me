using Godot;
using System;

[GlobalClass]
public partial class PloppableInstance : Node3D
{
    // DATA //
    private bool isHologram = false;


    // FUNCTIONS //
    // Godot Defaults


    // Placement
    public void PlopIntoWorld(Transform3D ploppedTransform, NavGraph navGraph)
    {
        Transform = ploppedTransform;
    }


    // Holograms
    public void ConvertToHologram(Material hologramMaterial)
    {
        // Sets to hologram
        isHologram = true;

        // Sets all the submeshes to use the hologram material
        MeshInstance3D[] subMeshes = Simplifications.GetChildrenOfType<MeshInstance3D>(this, true).ToArray();
        foreach(MeshInstance3D meshInstance in subMeshes)
        {
            meshInstance.MaterialOverride = hologramMaterial;
        }
    }

    public bool IsInPloppablePosition()
    {
        return true;
    }
}
