using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using Transportme.Main.DevTools;
using Transportme.Main.Scripts.Route;
using Transportme.Main.Scripts.Simulation;

namespace Transportme.Main.Scripts.Vehicle
{
	/// <summary>
	/// The base entity that traverses the navgraph. Within this node, it mainly controls the speed, and uses the route instance as well as the colliders
	/// to determine whether it should move, while placing most of the control along the <seealso cref="IRouteMovementIterator"/>. External control
	/// of the vehicle at certain events should be passed to the <seealso cref="VehicleController"/>.
	/// </summary>
	[GlobalClass]
	public partial class Vehicle : Node3D, IDebugVisualizationProvider, ISimulatable
	{
		// DATA //
		// Instance Configs
		[Export] public VehicleProperties VehicleProperties { get; set; }
		[Export] public VehicleController VehicleController { get; set; }

		[Export] public NavGraphContainer graph;
		[Export] protected Label3D speedLabel;

		private List<VehicleCollider> attachedColliders;
	
		// Properties
		private IRouteMovementIterator _route;
		public IRouteMovementIterator Route { get { return _route; } set { OnNewRoute(); _route = value; } }
		protected NavSegment CurrentSegment
		{
			get
			{
				return _route.GetCurrentSegment();
			}
		}
        private void OnNewRoute()
        {
            timeStopped = 0f;
        }

        // Cached Data
        protected double timeStopped = 0;
		private float turningRadius = 0;
		private double _speed;
		public double Speed { get { return _speed; } set { _speed = value; } }


        // METHODS //
        #region Godot Overrides
        public override void _Ready()
        {
            VehicleController.NotifyRouteComplete(this);
			var a = GetNode("/root/root/SimulationController");
			GD.Print(a.GetType().ToString());
            SimulationController sim = GetNode<SimulationController>("/root/root/SimulationController");
			if (sim == null) {
				GD.PrintErr("Vehicle > Couldn't find a Simulation Controller Node!");
			}
			sim.Register(this);
            base._Ready();
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

        public override void _EnterTree()
        {
            graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetNode("/root/"), true);
            attachedColliders = Simplifications.GetChildrenOfType<VehicleCollider>(this, true);
            //GD.Print(attachedColliders.Count);
            foreach (VehicleCollider c in attachedColliders)
            {
                c.AssociatedVehicle = this;
            }
            base._EnterTree();
        }

        #endregion


        private double GetTurningSpeedLimit()
		{
			RoutePoint original = Route.GetPositionOnRoute(VehicleProperties, 0);
			RoutePoint advance = Route.GetPositionOnRoute(VehicleProperties, 2);

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

        // Movement Functions

        public void BeforeSimulationStep()
        {
            if (_route == null || _route.IsFinishedRoute() || timeStopped > 10)
            {
                VehicleController.NotifyRouteComplete(this);
                return;
            }
        }

        public void Simulate(double delta)
        {
            RunMovementIteration(delta);
        }

        public void AfterSimulationStep()
        {
            //update collider positions
            foreach (VehicleCollider col in attachedColliders)
            {
                col.HandleUpdatePosition(_route);
            }
        }

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
			if (shouldStop || _speed - speedLimit > 0f)
			{
				_speed -= VehicleProperties.brakingPower * iterationDelta;
			}
			else if (_speed - speedLimit < 0f)
			{
				_speed += VehicleProperties.accelerationPower * iterationDelta;
			}

			//stop at 0
			if(_speed < 0) { _speed = 0; timeStopped += iterationDelta; }
			else { timeStopped = 0; }

			// update distance along route
			double newDistance = _speed * iterationDelta;
			_route.Move(newDistance);
		}

		public IEnumerable<DebugVisualization> GetVisualization()
		{
			var point = Route.GetPositionOnRoute(VehicleProperties, 2);
			yield return DebugVisualizationFactory.Line([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, point.backPoint, Colors.LimeGreen);
			yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.forwardPoint, 0.1f, Colors.LimeGreen);
			yield return DebugVisualizationFactory.Sphere([DebugVisualizationFilters.VehicleCollisions], point.backPoint, 0.1f, Colors.LimeGreen);
			foreach(var visualization in _route.GetVisualization())
			{
				yield return visualization;
			}

		}
    }
}