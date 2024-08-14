using Godot;
using System;

[GlobalClass]
[Tool]
public partial class WorldGrid : Node3D
{
	// DATA //
	// Editor Configs
	[Export] private MeshInstance3D gridRenderer;
	[Export] private float lerpWeight = 15;
	[Export] private bool smoothMovement = true;
	[Export] private float verticalOffsetFromPointer = -0.2f;

	// Properties - editor and elsewhere
	private float _gridSize = 1;
	[Export] private float GridSize { set { _gridSize = value; SetGridSize(value); } get { return _gridSize; } }
	private float _renderRadius = 15;
	[Export] private float RenderRadius { set { _renderRadius = value; SetRenderRadius(value); } get { return _renderRadius; } }
	private Color _gridColour = Colors.Blue;
	[Export] private Color GridColour { set { _gridColour = value; SetGridColour(value); } get { return _gridColour; } }
	// Cached Data
	private Vector3 nextPosition;


	// FUNCTIONS //
	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			SetGridSize(GridSize);
			SetRenderRadius(RenderRadius);
			SetGridColour(GridColour);
		}

		base._Ready();
	}

	public override void _Process(double delta)
	{
		// Moves to next position
		if (!Engine.IsEditorHint())
		{
			if (smoothMovement) GlobalPosition = GlobalPosition.Lerp(nextPosition, (float)delta * lerpWeight);
			else GlobalPosition = nextPosition;
		}

		base._Process(delta);
	}


	// External Usage
	public void SetNextPos(Vector3 newPos)
	{
		nextPosition = newPos + (Vector3.Down * verticalOffsetFromPointer);
	}

	
	// Changing Grid
	private void SetGridSize(float size)
	{
		// Sets the grid size
		gridRenderer?.SetInstanceShaderParameter("unitSize", size);
	}

	private void SetRenderRadius(float radius)
	{
		// Updates the radius of the mesh instance
		Vector3 newScale = new Vector3(radius, radius, radius);
		if (gridRenderer != null)
		{
			gridRenderer.Scale = newScale;
		}
	}

	private void SetGridColour(Color newColour)
	{
		gridRenderer?.SetInstanceShaderParameter("gridColor", newColour);
	}
}
