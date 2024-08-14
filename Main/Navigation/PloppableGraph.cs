using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class PloppableGraph : Node3D
{
	// DATA //
	// Graph data
	[ExportCategory("Graph Data")]
	[Export] public Array<NavSegment> segments = new Array<NavSegment>();


	// FUNCTIONS //
}
