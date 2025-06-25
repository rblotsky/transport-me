using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Tracing;
using Transportme.Main.DevTools;

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
	[Export] private DebugVisualizer debugVisualizer;

	// Editor Configs
	[ExportCategory("Configs")]
	[Export] private int maxPlacementDistance = 100;


	// FUNCTIONS //
	// Godot Defaults
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventKey keyInput)
		{
			if (keyInput.Keycode == Key.V && keyInput.IsPressed())
			{
				GD.Print("TODO spawn a vehicle!");
			}
			else if (keyInput.Keycode == Key.E && keyInput.IsPressed())
			{
				Vehicle[] allVehicles = Simplifications.GetChildrenOfType<Vehicle>(GetParent(), true).ToArray();
				foreach (Vehicle vehicle in allVehicles)
				{
					vehicle.speed += 1;
				}
			}
			else if (keyInput.Keycode == Key.Q && keyInput.IsPressed())
			{
				Vehicle[] allVehicles = Simplifications.GetChildrenOfType<Vehicle>(GetParent(), true).ToArray();
				foreach (Vehicle vehicle in allVehicles)
				{
					vehicle.speed -= 1;
				}
			}
			else if (keyInput.Keycode == Key.R && keyInput.IsPressed())
			{
				GD.Print("Pressed R");
				if ((debugVisualizer.ActiveFilters & DebugVisualizationFilters.VehicleCollisions) != 0)
				{
					debugVisualizer.ActiveFilters &= ~DebugVisualizationFilters.VehicleCollisions;
				}
				else
				{
					debugVisualizer.ActiveFilters |= DebugVisualizationFilters.VehicleCollisions;
				}
			}
			else if (keyInput.Keycode == Key.T && keyInput.IsPressed())
			{
				debugVisualizer.ActiveTypes.Add(DebugVisualizationType.Line);
			}
			else if (keyInput.Keycode == Key.F && keyInput.IsPressed()) {
				debugVisualizer.ActiveTypes.Add(DebugVisualizationType.Zone);
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
