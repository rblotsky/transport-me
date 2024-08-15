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
            if (route == null) return null;
            else if (currentSegmentIndex < 0) return null;
            else if (currentSegmentIndex >= route.OrderedSegments.Length) return null;
            return route.OrderedSegments[currentSegmentIndex]; 
        } 
    }

    // Cached Data
    private Route route = null;
    private double distanceAlongSegment = -1;
    private int currentSegmentIndex = -1;
    private double timeStopped = 0;
    public double speed = 0;
    private MeshInstance3D visualization;

    //using the current segment
    private Vector3 GetPositionOnSegment(float currentDistanceAlongSegment) 
    {
        if(CurrentSegment == null) return Position;
        NavSegment currentSegment = CurrentSegment;
        int segmentIndex = currentSegmentIndex;
        while(currentDistanceAlongSegment > currentSegment.Length)
        {
            currentDistanceAlongSegment -= currentSegment.Length;
            if(route.OrderedSegments.Length == segmentIndex - 1)
            {
                return currentSegment.GlobalEnd;
            }
            currentSegment = route.OrderedSegments[++segmentIndex];
        }
        float percentOfSegment = (float)currentDistanceAlongSegment / currentSegment.Length;
        return currentSegment.GetPositionOnSegment(percentOfSegment);
    }

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

        distanceAlongSegment += newDistance;

        // Moves to next segment if needed
        if (distanceAlongSegment > CurrentSegment.Length)
        {
            distanceAlongSegment -= CurrentSegment.Length;
            currentSegmentIndex += 1;
        }

        // If we reached the destination, stop. 
        if (CurrentSegment == null)
        {
            FinishCurrentRoute(true);
        }
        // Sets its position along the segment
        else
        {
            float percentOfSegment = (float)distanceAlongSegment / CurrentSegment.Length;
            Vector3 newPosition = CurrentSegment.GetPositionOnSegment(percentOfSegment);
            FaceDirectionOfMotion(newPosition - GlobalPosition);
            GlobalPosition = newPosition;
        }

        Vector3 colliderPosition = GetPositionOnSegment((float)distanceAlongSegment + (float)speed);
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
        distanceAlongSegment = 0;
        currentSegmentIndex = 0;
    }

    protected void FinishCurrentRoute(bool moveToEnd)
    {
        if (moveToEnd)
        {
            GlobalPosition = route.EndPoint + graphOffset;
        }

        Route finishedRoute = route;

        route = null;
        distanceAlongSegment = -1;
        currentSegmentIndex = -1;

        OnRouteFinish(finishedRoute);
    }

}
