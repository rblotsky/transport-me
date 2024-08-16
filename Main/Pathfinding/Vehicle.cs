using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Vehicle : Node3D
{
    // DATA //
    // Instance Configs
    [Export] public double maxSpeed = 1;
    [Export] public double acceleration = 0.2;
    [Export] protected NavGraphContainer graph;
    [Export] protected Vector3 graphOffset;
    [Export] protected float stoppingDistance;
    [Export] public bool showVisualizations;

    private List<Area3D> areas;

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

    

    // FUNCTIONS //
    // Godot Defaults
    public override void _EnterTree()
    {
        graph = Simplifications.GetFirstChildOfType<NavGraphContainer>(GetNode("/root/"), true);
        areas = Simplifications.GetChildrenOfType<Area3D>(this, true);
        GD.Print(areas.Count);
        base._EnterTree();
    }


    // Collision Detection
    protected bool ShouldStop(Vector3 startPos, Vector3 direction)
    {
        return false;
        PhysicsRayQueryParameters3D raycast = PhysicsRayQueryParameters3D.Create(
                        startPos,
                        (direction.Normalized()*stoppingDistance)+startPos);

        Dictionary raycastResult = GetWorld3D().DirectSpaceState.IntersectRay(raycast);

        // If it collides with anything, the vehicle should stop.
        if(raycastResult.Count > 0)
        {
            return true;
        }

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
        
        // Clears time stopped
        timeStopped = 0;

        // Gets how far to move this process frame
        double newDistance = speed * iterationDelta;

        distanceAlongRoute += (float)newDistance;

        // We have 
        if (distanceAlongRoute > route.GetLength())
        {
            FinishCurrentRoute(true);
        }

        // Sets its position along the segment
        else
        {
            Vector3 newPosition = route.GetPositionAlongRoute(distanceAlongRoute);
            FaceDirectionOfMotion(newPosition - GlobalPosition);
            GlobalPosition = newPosition;
        }

        Vector3 colliderPosition = route.GetPositionAlongRoute((float)distanceAlongRoute + (float)speed);
        areas[0].Position = ToLocal(colliderPosition);
        UpdateVisualization();
        
    }
    private void UpdateVisualization()
    {
        DeleteVisualization();
        visualization = EasyShapes.AddShapeMesh(this, new BoxMesh());
        visualization.Position = areas[0].Position;
    }

    private void DeleteVisualization()
    {
        if (visualization != null)
        {
            visualization.Free();
            visualization = null;
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
