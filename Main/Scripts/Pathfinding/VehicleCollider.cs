using Godot;
using System;
using System.Collections.Generic;
using Transportme.Main.Scripts.Pathfinding;

public enum CollisionState
{
	NO_COLLISIONS,
	COLLISION_HIGHER_PRIORITY,
	COLLISION_LOWER_PRIORITY,	
}

public abstract partial class VehicleCollider : Area3D
{
    // DATA //
	protected Vehicle associatedVehicle;
	protected CollisionState collsionState;
	[Export] private int NumIntersecting;

    // FUNCTIONS //
	public abstract void HandleUpdatePosition(IRouteMovementIterator route);
	protected abstract bool ShouldStop(List<VehicleCollider> colliders);

    public Vehicle AssociatedVehicle 
    { 
        get { return associatedVehicle; } set { associatedVehicle = value; } 
    }

	public bool GetColliderStatus()
	{
		List<VehicleCollider> validColliders = new List<VehicleCollider>();
		NumIntersecting = GetOverlappingAreas().Count;
		foreach (Area3D area in GetOverlappingAreas())
		{
			if(area is VehicleCollider && ((VehicleCollider)area).AssociatedVehicle != associatedVehicle)
			{
				validColliders.Add((VehicleCollider)area);
			}
		}
		return ShouldStop(validColliders);
	}

	//public virtual void UpdateVisualization()
	//{
	//	DeleteVisualization();
	//	visualization = EasyShapes.AddShapeMesh(this, new BoxMesh());
	//	visualization.GlobalPosition = GlobalPosition;
	//	visualization.GlobalRotation = GlobalRotation;
	//	//visualization.Scale = ((BoxShape3D)Simplifications.GetFirstChildOfType<CollisionShape3D>(this).Shape).Size;
	//}
	//public virtual void UpdatePositionVisualizations()
	//{
	//	DeletePosVisualization();
	//	Route route = associatedVehicle.CurrentRoute;
	//	RoutePoint point = route.GetVehicleRoutePositionAtPoint(associatedVehicle.CurrentDistanceAlongRoute);
	//	tangentLine = EasyShapes.AddShapeMesh(this, EasyShapes.LineMesh(ToLocal(point.backPoint), ToLocal(point.forwardPoint), Colors.Black));
	//	visPos = EasyShapes.AddShapeMesh(this, EasyShapes.SphereMesh(0.1f));
	//	visPos.GlobalPosition = GlobalPosition;
	//}

    // TODO: Extract because it's used elsewhere too
	protected void FaceDirectionOfMotion(Vector3 positionDelta)
	{
		if (!positionDelta.IsEqualApprox(Vector3.Zero))
		{
			LookAt(GlobalPosition + positionDelta, Vector3.Up);
		}
		else
		{
			//GD.Print(positionDelta, associatedVehicle.speed);
		}
	}

	//public void DeleteVisualization()
	//{
	//	if(visualization != null)
	//	{
	//		visualization.Free();
	//	}
	//}

	//public void DeletePosVisualization()
	//{
	//	if (visPos != null)
	//	{
	//		visPos.Free();
	//	}
	//	if(tangentLine != null)
	//	{
	//		tangentLine.Free();
	//	}
	//}

}
