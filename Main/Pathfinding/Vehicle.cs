using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Vehicle : Node3D
{
	// DATA //
	// Instance Configs
	[Export] public double maxVehicleSpeed = 7.5;
	[Export] public double acceleration = 1;
	[Export] public double brakeSpeed = 3;
	[Export] protected NavGraphContainer graph;
	[Export] protected Vector3 graphOffset;
	[Export] protected float stoppingDistance;
	[Export] public bool showVisualizations;
	[Export] public bool showPositionVisualizations;

	private List<VehicleCollider> attachedColliders;
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
		attachedColliders = Simplifications.GetChildrenOfType<VehicleCollider>(this, true);
		GD.Print(attachedColliders.Count);
		foreach(VehicleCollider c in attachedColliders)
		{
			c.SetAssociatedVehicle(this);
		}
		base._EnterTree();
	}

	// Movement Functions
	protected void RunMovementIteration(double iterationDelta)
	{
		if (distanceAlongRoute > route.GetLength())
		{
			FinishCurrentRoute(true);
			return;
		}
		// collider checks
		// Decides whether to move at all this frame (is another vehicle blocking it?)
		bool shouldStop = false;
		for(int i = 0; i<attachedColliders.Count; i++)
		{
			shouldStop = attachedColliders[i].GetColliderStatus();
			if (shouldStop) { break; }
		}

		// max speed calculations
		NavSegment curSegment = route.GetSegmentAlongRoute(distanceAlongRoute);
		float speedLimit = Mathf.Min((float)maxVehicleSpeed, curSegment.MaxSpeed);

		//accelerating or decelerating
		if (shouldStop || speed - speedLimit > 0.1f)
		{
			speed -= brakeSpeed * iterationDelta;
		}
		else if (speed - speedLimit < -0.1f)
		{
			speed += acceleration * iterationDelta;
		}

		//stop at 0
		if(speed < 0) { speed = 0; timeStopped += iterationDelta; }
		else { timeStopped = 0; }

		// update distance along route
		double newDistance = speed * iterationDelta;
		distanceAlongRoute += (float)newDistance;

		//update collider positions
		foreach(VehicleCollider col in attachedColliders)
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

