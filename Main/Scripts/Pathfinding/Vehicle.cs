using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Pathfinding;

[GlobalClass]
public partial class Vehicle : Node3D, IDebugVisualizationProvider
{
	// DATA //
	// Instance Configs
	[Export] public VehicleProperties VehicleProperties { get; set; }
	private double _speed;
	public double Speed { get { return _speed; } set { _speed = value; } }
	[Export] protected NavGraphContainer graph;
	//[Export] protected Vector3 graphOffset;
	[Export] protected float stoppingDistance;
	[Export] protected Label3D speedLabel;
	private IRouteMovementIterator _route;

	private List<VehicleCollider> attachedColliders;
	
	// Properties
	protected NavSegment CurrentSegment
	{
		get
		{
			return _route.GetCurrentSegment();
		}
	}
	public IRouteMovementIterator Route { get { return _route; } set { _route = value; } }
    // Cached Data
 //   private Route route = null;
	//private float distanceAlongRoute = 0f;

	//public double speed = 0;
	protected double timeStopped = 0;
	private float turningRadius = 0; 

	private double GetTurningSpeedLimit()
	{
		RoutePoint original = Route.GetPositionOnRoute(0);
		RoutePoint advance = Route.GetPositionOnRoute(2);

		if (original.Rotation.IsEqualApprox(advance.Rotation))
		{
			return VehicleProperties.maxSpeed;
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
			speedLabel.Text = $"""Speed: {Speed.ToString("0.##")}   Turning Max Speed: {speedLimit.ToString("0.##")} Turning Radius: {turningRadius.ToString("0.##")}""";
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
		//figure out how to do this
		//if (distanceAlongRoute > route.GetLength())
		//{
		//	FinishCurrentRoute(true);
		//	return;
		//}
		// collider checks
		// Decides whether to move at all this frame (is another vehicle blocking it?)
		bool shouldStop = false;
		for(int i = 0; i < attachedColliders.Count; i++)
		{
			shouldStop = attachedColliders[i].GetColliderStatus();
			if (shouldStop) { break; }
		}

		// max speed calculations
		NavSegment curSegment = _route.GetCurrentSegment();
		float speedLimit = Mathf.Min(VehicleProperties.maxSpeed, curSegment.MaxSpeed);
		double turningSpeedLimit = GetTurningSpeedLimit();

		if (turningSpeedLimit < speedLimit) {
			speedLimit = (float)turningSpeedLimit;
		}

		//speedLimit = Mathf.Min((float)maxVehicleSpeed, route.GetLength());

		//accelerating or decelerating
		if (shouldStop || _speed - speedLimit > 0.1f)
		{
            _speed -= VehicleProperties.brakingPower * iterationDelta;
		}
		else if (_speed - speedLimit < -0.1f)
		{
            _speed += VehicleProperties.accelerationPower * iterationDelta;
		}

		//stop at 0
		if(_speed < 0) { _speed = 0; timeStopped += iterationDelta; }
		else { timeStopped = 0; }

		// update distance along route
		double newDistance = _speed * iterationDelta;
		_route.Move(newDistance);

		//update collider positions
		foreach(VehicleCollider col in attachedColliders)
		{
			col.HandleUpdatePosition(_route);
		}
	}

    public IEnumerable<DebugVisualization> GetVisualization()
    {
		var point = Route.GetPositionOnRoute(2);
		yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.LimeGreen);
		yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, 0.1f, Colors.LimeGreen);
        yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.backPoint, 0.1f, Colors.LimeGreen);

    }
}

