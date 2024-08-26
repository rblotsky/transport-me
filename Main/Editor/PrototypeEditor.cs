using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Tracing;

[GlobalClass]
public partial class PrototypeEditor : Node
{
	// DATA //
	// Scene References
	[ExportCategory("References")]
	[Export] private Camera3D camera;
	[Export] private Cursor cursor;
	[Export] private WorldGrid grid;
	[Export] private NavGraphContainer navGraph;

	// Editor Configs
	[ExportCategory("Configs")]
	[Export] private int maxPlacementDistance = 100;


	// FUNCTIONS //
	// Godot Defaults
	public override void _Ready()
	{
		InstructionsUI.instance.AddInstruction(this, "Press E to make them all go faster.");
		InstructionsUI.instance.AddInstruction(this, "Press Q to make them all go slower.");

		base._Ready();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventKey keyInput)
		{
			if(keyInput.Keycode == Key.V && keyInput.IsPressed())
			{
				GD.Print("TODO spawn a vehicle!");
			}
			else if (keyInput.Keycode == Key.E && keyInput.IsPressed())
			{
				Vehicle[] allVehicles = Simplifications.GetChildrenOfType<Vehicle>(GetParent(), true).ToArray();
				foreach(Vehicle vehicle in allVehicles)
				{
					vehicle.speed+=1;
				}
			}
			else if (keyInput.Keycode == Key.Q && keyInput.IsPressed())
			{
				Vehicle[] allVehicles = Simplifications.GetChildrenOfType<Vehicle>(GetParent(), true).ToArray();
				foreach (Vehicle vehicle in allVehicles)
				{
					vehicle.speed-=1;
				}
			}
		}
		base._UnhandledInput(@event);
	}

	public override void _Process(double delta)
	{
		cursor.SetNextPos(GetRaycastMousePosition());
		grid.SetNextPos(GetRaycastMousePosition());
		base._Process(delta);
	}

	// Mouse Position 
	private Vector3 GetRaycastMousePosition()
	{
		PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

		Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);

		if (raycastResults.Values.Count > 0)
		{
			return Simplifications.SnapV3ToGrid((Vector3)raycastResults["position"]);
		}
		else
		{
			return Simplifications.SnapV3ToGrid(Simplifications.GetWorldMousePosition(camera));
		}
	}

}
