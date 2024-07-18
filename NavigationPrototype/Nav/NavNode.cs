using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class NavNode : Resource
{
    // DATA //
    // Serialized Properties
    [Export] public float NodeRadius { get; set; }
    [Export] public Vector3I GridPosition { get; set; }
}
