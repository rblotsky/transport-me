using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Vehicle : Node3D
{
	// DATA //
	// Instance Configs
	[Export] public double maxSpeed = 7.5;
	[Export] public double acceleration = 1;
	[Export] public double brakeSpeed = 3;
	[Export] protected NavGraphContainer graph;
	[Export] protected Vector3 graphOffset;
	[Export] protected float stoppingDistance;
	[Export] public bool showVisualizations;

	private List<Area3D> areas;
	private List<bool> collisions;
	// Properties
	protected NavSegment CurrentSegment 
	{ 
		get 
		{
			return route?.GetSegmentAlongRoute(distanceAlongRoute);
		} 
	}

	// Cached Data
	private Route route = null;
	private float distanceAlongRoute = 0f;
	private double timeStopped = 0;
	public double speed = 0;
	private MeshInstance3D visualization;
	private MeshInstance3D visualization2;


	public Route GetRoute()
	{
		return route;
	}

	public float GetDistanceAlongRoute()
	{
		return distanceAlongRoute;
	}
	public double GetCurrentSpeed()
	{
		return speed;
	}

	// FUNCTIONS //
	// Godot Defaults
	public override void _EnterTree()
	{
		graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetNode("/root/"), true);
		areas = Simplifications.GetChildrenOfType<Area3D>(this, true);
		GD.Print(areas.Count);
		collisions = new List<bool>(areas.Count);
		for(int i = 0; i< areas.Count; i++)
		{
			areas[i].AreaEntered += (Area3D area) => OnAreaEntered(i, area);
			areas[i].AreaExited += (Area3D area) => OnAreaExited(i, area);

		}
		base._EnterTree();
	}

	private void OnAreaEntered (int number, Area3D area)
	{
		if (areas.Contains(area))
		{
			// GD.Print("entered myself");
			return;
		}
		collisions[number] = true;
		GD.Print("entered");

	}

	private void OnAreaExited (int number, Area3D area)
	{
		if (areas.Contains(area))
		{
			// GD.Print(";eft myself");
			return;
		}
		collisions[number] = false;
		GD.Print("exited");
	}


	// Collision Detection
	protected bool ShouldStop(Vector3 startPos, Vector3 direction)
	{
		return false;
	}
	   

	// Movement Functions
	protected void RunMovementIteration(double iterationDelta)
	{
		if (ShouldStop(GlobalPosition, -GlobalTransform.Basis.Z) && timeStopped > 10)
		{
			GD.Print("i hate mvoing");
			timeStopped += iterationDelta;
			return;
		}
		// Decides whether to move at all this frame (is another vehicle blocking it?)
		if (distanceAlongRoute > route.GetLength())
		{
			FinishCurrentRoute(true);
			return;
		}
		// Clears time stopped
		timeStopped = 0;
		NavSegment curSegment = route.GetSegmentAlongRoute(distanceAlongRoute);
		float maxMaxSpeed = maxSpeed < curSegment.MaxSpeed ? (float)maxSpeed : curSegment.MaxSpeed;

		if(speed - maxMaxSpeed > 0.1f)
		{
			speed -= brakeSpeed * iterationDelta;
		} else if (speed - maxMaxSpeed < -0.1f)
		{
			speed += acceleration * iterationDelta;
		}
		
		// Gets how far to move this process frame
		double newDistance = speed * iterationDelta;
		distanceAlongRoute += (float)newDistance;

		// We have 


		// Sets its position along the segment

			Vector3 newPosition = route.GetPositionAlongRoute(distanceAlongRoute);
			FaceDirectionOfMotion(newPosition - GlobalPosition);
			GlobalPosition = newPosition;
		

		Vector3 colliderPosition = route.GetPositionAlongRoute((float)distanceAlongRoute + (float)speed);
		areas[0].Position = ToLocal(colliderPosition);
		float timeToStop = (float)(speed / brakeSpeed);
		Vector3 brakeColliderPosition = route.GetPositionAlongRoute((float)distanceAlongRoute + (float)(speed * timeToStop) + 0.5f * (float)brakeSpeed * timeToStop * timeToStop);
		areas[1].Position = ToLocal(brakeColliderPosition);
		UpdateVisualization();
		
	}
	private void UpdateVisualization()
	{
		DeleteVisualization();
		visualization = EasyShapes.AddShapeMesh(this, new BoxMesh());
		visualization.Position = areas[0].Position;
		visualization.Scale = areas[0].Scale;
		visualization2 = EasyShapes.AddShapeMesh(this, new BoxMesh());
		visualization2.Position = areas[1].Position;
		visualization2.Scale = areas[1].Scale;
	}

	private void DeleteVisualization()
	{
		if (visualization != null)
		{
			visualization.Free();
			visualization = null;
		}
		if (visualization2 != null)
		{
			visualization2.Free();
			visualization2 = null;
		}
	}

	protected void FaceDirectionOfMotion(Vector3 positionDelta)
	{
		if (positionDelta.LengthSquared() != 0)
		{
			LookAt(GlobalPosition + positionDelta, Vector3.Up);
		}
	}


	// Virtual Functions
	protected virtual void OnRouteFinish(Route finished)
	{
	}

	// Managing Route
	protected void StartRoute(Route newRoute)
	{
		route = newRoute;
		distanceAlongRoute = 0;
	}

	protected void FinishCurrentRoute(bool moveToEnd)
	{
		if (moveToEnd)
		{
			GlobalPosition = route.EndPoint + graphOffset;
		}

		Route finishedRoute = route;

		route = null;
		distanceAlongRoute = 0;

		OnRouteFinish(finishedRoute);
	}

}
