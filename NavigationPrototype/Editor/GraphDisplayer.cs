using Godot;
using System;

[GlobalClass]
public partial class GraphDisplayer : Node
{
    // DATA //
    // Toggle
    private bool _displayToggle = false;
    [Export] public bool DisplayToggle { get; set; }
}
