using Godot;
using System;

[GlobalClass]
public partial class PloppableInstance : Node3D
{
    // DATA //
    [Export] public bool Hologram { get; set; }


    // FUNCTIONS //
    // Godot Defaults
    public override void _Ready()
    {
        GD.Print("LOL I JUST GOT READIED LMAO");
        base._Ready();
    }


    // Instantiation
    public void ConvertToHologram(Material hologramMaterial)
    {
        Hologram = true;

        MeshInstance3D[] subMeshes = Simplifications.GetChildrenOfType<MeshInstance3D>(this, true).ToArray();

    }
}
