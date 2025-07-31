using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Tracing;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Vehicle;

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
			if (keyInput.IsPressed())
			{
				switch (keyInput.Keycode)
				{
					case Key.V:
						GD.Print("TODO spawn a vehicle!");
						break;
					case Key.E:
						GetNode<SimulationController>("/root/root/SimulationController").simulationTimeMultiplier += 1;
						break;
					case Key.Q:
                        GetNode<SimulationController>("/root/root/SimulationController").simulationTimeMultiplier -= 1;
                        break;
					case Key.Space:
						GetNode<SimulationController>("/root/root/SimulationController").ToggleSimulation();
						break;
					case Key.Y:
						GetNode<SimulationController>("/root/root/SimulationController").SimulateSteps(100);
						break;
                    case Key.R:
						GD.Print("Pressed R");
						if ((debugVisualizer.ActiveFilters & DebugVisualizationFilters.VehicleCollisions) != 0)
						{
							debugVisualizer.ActiveFilters &= ~DebugVisualizationFilters.VehicleCollisions;
						}
						else
						{
							debugVisualizer.ActiveFilters |= DebugVisualizationFilters.VehicleCollisions;
						}
						break;
					case Key.T:
						debugVisualizer.ActiveTypes.Add(DebugVisualizationType.Line);
						break;
					case Key.F:
						debugVisualizer.ActiveTypes.Add(DebugVisualizationType.Zone);
						break;
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
