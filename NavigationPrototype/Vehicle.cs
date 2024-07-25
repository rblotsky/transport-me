using Godot;
using System;

[GlobalClass]
public partial class Vehicle : Node3D
{
    // DATA //
    // Instance Configs
    [Export] private double distancePerSecond = 1;
    [Export] private NavCheckpoint start;
    [Export] private NavCheckpoint destination;
    [Export] private NavGraphContainer graph;
    [Export] private Vector3 graphOffset;
    
    // Properties
    private NavSegment CurrentSegment 
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


    // FUNCTIONS //
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventKey keyInput)
        {
            if(keyInput.Keycode == Key.V && keyInput.IsPressed())
            {
                GD.Print("Pressed V, registered by the Vehicle!");
                route = Route.CreateRouteBFS(start.GlobalSnappedPos, destination.GlobalSnappedPos, graph);
                
            }
        }
        base._UnhandledInput(@event);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        // Only moves if we haven't reached the end yet.
        if (CurrentSegment != null)
        {
            // Gets how far to move this process frame
            double newDistance = distancePerSecond * delta;

            distanceAlongSegment += newDistance;

            // Moves to next segment if needed
            if (distanceAlongSegment > CurrentSegment.SimpleLength)
            {
                distanceAlongSegment -= CurrentSegment.SimpleLength;
                currentSegmentIndex += 1;
            }

            // If we reached the destination, stop. 
            if (CurrentSegment == null)
            {
                GlobalPosition = route.EndPoint + graphOffset;
            }
            // Sets its position along the segment
            else
            {
                float percentOfSegment = (float)distanceAlongSegment / CurrentSegment.SimpleLength;
                Vector3 newPosition = GetPositionOnSegment(percentOfSegment);
                FaceDirectionOfMotion(newPosition - GlobalPosition);
                GlobalPosition = newPosition;
            }
        }

        base._PhysicsProcess(delta);
    }

    private Vector3 GetPositionOnSegment(float percentOfSegment)
    {
        return graphOffset + EasyShapes.CalculateBezierQuadraticWithHeight(
            CurrentSegment.GlobalStart, 
            CurrentSegment.GlobalControl, 
            CurrentSegment.GlobalEnd, 
            percentOfSegment
            );
    }

    private void FaceDirectionOfMotion(Vector3 positionDelta)
    {
        LookAt(GlobalPosition + positionDelta, Vector3.Up);
    }

    private void StartRoute(Route newRoute)
    {
        route = newRoute;
        Debugger3D.main.RouteEffectDefault(route, 15);
        distanceAlongSegment = 0;
        currentSegmentIndex = 0;
    }

}
