using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;

//TODO: Remove? Have separate entrance/exit checkpoints for people to spawn/get deleted in?
[GlobalClass]
[Tool]
public partial class NavCheckpoint : Node3D, IDebugVisualizationProvider
{
	// DATA //
	
	// Readonly Properties
	public Vector3 GlobalSnappedPos { get { return Simplifications.SnapV3ToGrid(GlobalPosition); } }
	public Vector3 LocalSnappedPos { get { return Simplifications.SnapV3ToGrid(Position); } }

	private NavConnection _associatedConnection;
	public NavConnection AssociatedConnection { get { return _associatedConnection; } set { GD.Print(value); _associatedConnection = value; } }

	// FUNCTIONS //

    public IEnumerable<DebugVisualization> GetVisualization()
    {
		Color color = _associatedConnection == null ? Colors.Red : Colors.Green;
		yield return DebugVisualizationFactory.Sphere( [DebugVisualizationFilters.NavSegments], GlobalPosition, 0.3f, color);
    }
}
