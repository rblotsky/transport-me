using Godot;
using System;

[GlobalClass]
public partial class RandomVehicle : Node3D
{
    // DATA //
    // Instance Configs
    [Export] private double distancePerSecond = 1;
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

        // If we reached the end, starts a new route.
        else
        {
            StartRandomRoute();
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
        if (positionDelta.Length() != 0)
        {
            LookAt(GlobalPosition + positionDelta, Vector3.Up);
        }
    }

    private void StartRoute(Route newRoute)
    {
        if (newRoute == null)
        {
            GD.Print("ERR CANNOT FIND ROUTE!");
        }
        else
        {
            route = newRoute;
            //Debugger3D.main.RouteEffectDefault(route, 15);
            distanceAlongSegment = 0;
            currentSegmentIndex = 0;
        }
    }

    private void StartRandomRoute()
    {
        // Does nothing if graph isn't ready
        if (graph.isGraphReady)
        {
            // Randomly chooses two checkpoints from the graph
            RandomNumberGenerator rng = new RandomNumberGenerator();
            rng.Randomize();

            NavCheckpoint[] endpoints = graph.GetTwoRandomCheckpoints(rng);

            if (endpoints.Length == 0)
            {
                GD.PrintErr("Failed to find two valid endpoints for a random path!");
            }
            else
            {
                StartRoute(Route.CreateRouteBFS(endpoints[0].GlobalSnappedPos, endpoints[1].GlobalSnappedPos, graph));
            }
        }
    }
}
