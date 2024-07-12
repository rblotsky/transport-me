using Godot;
using System;

[GlobalClass]
public partial class PloppableResource : Resource
{
    // DATA //
    [Export] public string Name { get; set; }
    [Export] public PackedScene PloppableScene { get; set; }


    // FUNCTIONS //
    // lol
}
