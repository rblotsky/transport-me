using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;

[GlobalClass]
public partial class Vehicle : Node3D, IDebugVisualizationProvider
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
	[Export] protected Label3D speedLabel;

	private List<VehicleCollider> attachedColliders;
	// Properties
	protected NavSegment CurrentSegment
	{
		get
		{
			return route?.GetSegmentAlongRoute(distanceAlongRoute);
		}
	}
    public Route CurrentRoute { get { return route; } }
    public float CurrentDistanceAlongRoute { get { return distanceAlongRoute; } }
    public double CurrentSpeed { get { return speed; } }

    // Cached Data
    private Route route = null;
	private float distanceAlongRoute = 0f;
	protected double timeStopped = 0;
	public double speed = 0;

	private float turningRadius = 0; 

	private double GetTurningSpeedLimit()
	{
		RoutePoint original = route.GetVehicleRoutePositionAtPoint(distanceAlongRoute);
		RoutePoint advance = route.GetVehicleRoutePositionAtPoint(distanceAlongRoute + 2);

		if (original.Rotation.IsEqualApprox(advance.Rotation))
		{
			return maxVehicleSpeed;
		}

		Vector2 radiusPoint = Simplifications.GetPointOfIntersection(
			Simplifications.GetVectorXZ(original.Position),
			Simplifications.GetVectorXZ(original.Rotation.Normalized()).Orthogonal(),
			Simplifications.GetVectorXZ(advance.Position),
			Simplifications.GetVectorXZ(advance.Rotation.Normalized()).Orthogonal()
		);

		float radius = (radiusPoint - Simplifications.GetVectorXZ(original.Position)).Length();
		this.turningRadius = radius;
		double thing = Math.Sqrt(10 * (radius + 1));
		return thing;
	}

    public override void _Process(double delta)
    {
		if (speedLabel != null)
		{
			double speedLimit = GetTurningSpeedLimit();
			speedLabel.Text = $"""Speed: {speed.ToString("0.##")}   Turning Max Speed: {speedLimit.ToString("0.##")} Turning Radius: {turningRadius.ToString("0.##")}""";
		}
        base._Process(delta);
    }
    // FUNCTIONS //
    // Godot Defaults
    public override void _EnterTree()
	{
		graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetNode("/root/"), true);
		attachedColliders = Simplifications.GetChildrenOfType<VehicleCollider>(this, true);
		//GD.Print(attachedColliders.Count);
		foreach(VehicleCollider c in attachedColliders)
		{
			c.AssociatedVehicle = this;
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
		double turningSpeedLimit = GetTurningSpeedLimit();

		if (turningSpeedLimit < speedLimit) {
			GD.Print("speed limits ", turningSpeedLimit, " ", speed);
			speedLimit = (float)turningSpeedLimit;
		}

		//speedLimit = Mathf.Min((float)maxVehicleSpeed, route.GetLength());

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
		}
	}

	// Virtual Functions
    //TODO: Rename so it doesnt sound like it should be an `event`
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

    public IEnumerable<DebugVisualization> GetVisualization()
    {
		var point = route.GetVehicleRoutePositionAtPoint(distanceAlongRoute + 2);
		yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.LimeGreen);
		yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, 0.1f, Colors.LimeGreen);
        yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.backPoint, 0.1f, Colors.LimeGreen);

    }
}

