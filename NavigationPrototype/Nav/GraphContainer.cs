using Godot;
using Godot.Collections;
using System;

public partial class GraphContainer : Node
{
    // DATA //
    // Graph data
    [ExportCategory("Graph Data")]
    [Export] public Array<NavNode> nodes = new Array<NavNode>();
    [Export] public Array<NavSegment> segments = new Array<NavSegment>();


    // FUNCTIONS //
}
