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
	[Export] public bool showPositionVisualizations;

	private List<VehicleCollider> areas;
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
	protected double timeStopped = 0;
	public double speed = 0;


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
		areas = Simplifications.GetChildrenOfType<VehicleCollider>(this, true);
		GD.Print(areas.Count);
		foreach(VehicleCollider c in areas)
		{
			c.SetAssociatedVehicle(this);
		}
		base._EnterTree();
	}

	private void OnAreaEntered(int number, VehicleCollider area)
	{
		if (areas.Contains(area))
		{
			// GD.Print("entered myself");
			return;
		}
		collisions[number] = true;
		GD.Print("entered");

	}

	private void OnAreaExited(int number, VehicleCollider area)
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
		// collider checks
		bool shouldStop = false;
		for(int i = 0; i<areas.Count; i++)
		{
			shouldStop = areas[i].GetColliderStatus();
			if (shouldStop) { break; }
		}

		// max speed calculations
		NavSegment curSegment = route.GetSegmentAlongRoute(distanceAlongRoute);
		float maxMaxSpeed = maxSpeed < curSegment.MaxSpeed ? (float)maxSpeed : curSegment.MaxSpeed;

		//accelerating or decelerating
		if (shouldStop || speed - maxMaxSpeed > 0.1f)
		{
			speed -= brakeSpeed * iterationDelta;
		}
		else if (speed - maxMaxSpeed < -0.1f)
		{
			speed += acceleration * iterationDelta;
		}

		//prevent reversing
		if(speed < 0) { speed = 0; timeStopped += iterationDelta; }
		else { timeStopped = 0; }

		// update distance along route
		double newDistance = speed * iterationDelta;
		distanceAlongRoute += (float)newDistance;
		foreach(VehicleCollider col in areas)
		{
			col.HandleUpdatePosition();
			if (showVisualizations)
			{

				col.UpdateVisualization();
			}
			if (showPositionVisualizations)
			{
				col.UpdatePositionVisualizations();
			}
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

