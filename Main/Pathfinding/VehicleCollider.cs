using Godot;
using System;
using System.Collections.Generic;

public enum CollisionState
{
	NO_COLLISIONS,
	COLLISION_HIGHER_PRIORITY,
	COLLISION_LOWER_PRIORITY,	
}

public abstract partial class VehicleCollider : Area3D
{
	protected Vehicle associatedVehicle;
	protected CollisionState collsionState;
	protected MeshInstance3D visualization;
	[Export] private int NumIntersecting;

	public Vehicle GetAssociatedVehicle()
	{
		return associatedVehicle;
	}
	public abstract void HandleUpdatePosition();
	protected abstract bool ShouldStop(List<VehicleCollider> colliders);

	public bool GetColliderStatus()
	{
		List<VehicleCollider> validColliders = new List<VehicleCollider>();
		NumIntersecting = GetOverlappingAreas().Count;
		foreach (Area3D area in GetOverlappingAreas())
		{
			if(area is VehicleCollider && ((VehicleCollider)area).GetAssociatedVehicle() != associatedVehicle)
			{
				validColliders.Add((VehicleCollider)area);
			}
		}
		return ShouldStop(validColliders);
	}

	public virtual void UpdateVisualization()
	{
		DeleteVisualization();
		visualization = EasyShapes.AddShapeMesh(this, new BoxMesh());
		visualization.GlobalPosition = GlobalPosition;
		visualization.GlobalRotation = GlobalRotation;
		//visualization.Scale = ((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size;
	}

	protected void FaceDirectionOfMotion(Vector3 positionDelta)
	{
		if (!positionDelta.IsEqualApprox(Vector3.Zero))
		{
			LookAt(GlobalPosition + positionDelta, Vector3.Up);
		}
		else
		{
			GD.Print(positionDelta, associatedVehicle.speed);
		}
	}

	public void DeleteVisualization()
	{
		if(visualization != null)
		{
			visualization.Free();
		}
	}

	public void SetAssociatedVehicle(Vehicle vehicle) 
	{
		associatedVehicle = vehicle;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}
}
