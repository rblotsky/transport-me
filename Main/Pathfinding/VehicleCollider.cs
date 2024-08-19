using Godot;
using System;
public enum CollisionState
{
	NO_COLLISIONS,
	COLLISION_HIGHER_PRIORITY,
	COLLISION_LOWER_PRIORITY,	
}

public abstract partial class VehicleCollider : Area3D
{
	protected VehicleRefactored associatedVehicle;
	protected CollisionState collsionState;
	private MeshInstance3D visualization;

	public VehicleRefactored GetAssociatedVehicle()
	{
		return associatedVehicle;
	}
	public abstract void HandleUpdatePosition(VehicleRefactored vehicle);
	public void UpdateVisualization()
	{
		DeleteVisualization();
		visualization = EasyShapes.AddShapeMesh(this, new BoxMesh());
		visualization.GlobalPosition = GlobalPosition;
		visualization.GlobalRotation = GlobalRotation;
	}

	protected void FaceDirectionOfMotion(Vector3 positionDelta)
	{
		if (positionDelta.LengthSquared() != 0)
		{
			LookAt(GlobalPosition + positionDelta, Vector3.Up);
		}
	}

	public void DeleteVisualization()
	{
		if(visualization != null)
		{
			visualization.Free();
		}
	}

	public void OnAreaEntered(Area3D area)
	{
		if (((VehicleCollider)area)?.GetAssociatedVehicle() == associatedVehicle)
		{
			GD.Print("entered myself");
			return;
		}
		GD.Print("entered");

	}

	public void OnAreaExited(Area3D area)
	{
		if (((VehicleCollider)area)?.GetAssociatedVehicle() == associatedVehicle)
		{
			GD.Print(";eft myself");
			return;
		}
		GD.Print("exited");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		associatedVehicle = GetParent<VehicleRefactored>();
		base._Ready();
	}
}
