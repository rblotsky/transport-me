using Godot;
using System;

[GlobalClass]
public partial class Building : Node3D
{
    [Flags]
    public enum BuildingType
    {
        None = 0,
        SuburbanResidential=1,
        RuralResidential=2,
        MediumResidential=4,
        HighRiseResidential=8,
    }

    // DATA //
    [Export] private string name;
    [Export(PropertyHint.Flags)] private BuildingType type;
    [Export] private NavCheckpoint accessPoint;
}
