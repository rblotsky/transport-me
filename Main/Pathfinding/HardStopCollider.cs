using Godot;
using System;
using System.Collections.Generic;

public partial class HardStopCollider : VehicleCollider
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void UpdateVisualization()
	{
		base.UpdateVisualization();
		visualization.Scale = ((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size;
	}

	public override void HandleUpdatePosition(VehicleRefactored vehicle)
    {
		Route route = vehicle.GetRoute();
		float distanceAlongRoute = vehicle.GetDistanceAlongRoute();
		float speed = (float)vehicle.GetCurrentSpeed();
		float timeToStop = speed / (float)vehicle.brakeSpeed;
		Vector3 brakeColliderPosition = route.GetPositionAlongRoute((float)distanceAlongRoute + (float)(speed * timeToStop) - 0.25f * (float)vehicle.brakeSpeed * timeToStop * timeToStop);
		FaceDirectionOfMotion(brakeColliderPosition - GlobalPosition);
		GlobalPosition = brakeColliderPosition;
	}

    protected override bool ShouldStop(List<VehicleCollider> colliders)
    {
        return (bool)(colliders.Count > 0);
    }
}
