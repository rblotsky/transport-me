using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[GlobalClass]
public partial class NavGraphContainer : Node3D
{
    // DATA //
    // Cached Data
    private List<NavConnection> segmentConnections; // Optimization: Maybe make it a list of 4, will there ever be more than 4 at one connector?
    private List<NavCheckpoint> checkpoints;
    private List<NavSegment> segments;
    public bool isGraphReady = false;


    // FUNCTIONS //
    public override void _Ready()
    {
        isGraphReady = false;
        SetupNavGraph();
        AssembleSegments();
        AddAllCheckpoints();
        InstructionsUI.instance.AddInstruction(this, "Press G to view a debug version of the nav graph.");
        isGraphReady = true;
        base._Ready();
    }

    public override void _ExitTree()
    {
        ClearNavGraph();
        base._ExitTree();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventKey keyInput)
        {
            if(keyInput.Keycode == Key.G && keyInput.IsPressed())
            {
                GD.Print("Lol they pressed G");
                DebugDrawGraph();
            }
        }
        base._UnhandledInput(@event);
    }


    // Setup and Cleanup
    private void SetupNavGraph()
    {
        segmentConnections = new List<NavConnection>();
        checkpoints = new List<NavCheckpoint>();
        segments = new List<NavSegment>();
    }

    private void ClearNavGraph()
    {
        if(segmentConnections != null)
        {
            segmentConnections.Clear();
            segmentConnections = null;
        }

        if(segments != null)
        {
            segments.Clear();
            segments = null;
        }
    }

    private void AssembleSegments()
    {
        NavSegment[] foundSegments = Simplifications.GetChildrenOfType<NavSegment>(this, true).ToArray();
        foreach(NavSegment segment in foundSegments)
        {
            segments.Add(segment);
            segment.DebugPrint();

            NavConnection outbound = GetIntersectionAt(segment.GlobalStart, true);
            segment.OutboundConnection = outbound;
            outbound.AddOutbound(segment);

            NavConnection inbound = GetIntersectionAt(segment.GlobalEnd, true);
            segment.InboundConnection = inbound;
            inbound.AddInbound(segment);
        }
    }

    private void AddAllCheckpoints()
    {
        NavCheckpoint[] newCheckpoints = Simplifications.GetChildrenOfType<NavCheckpoint>(this, true).ToArray();

        foreach (NavCheckpoint point in newCheckpoints)
        {
            AddCheckpoint(point.GlobalSnappedPos, point);
        }
    }
    
    public NavConnection GetIntersectionAt(Vector3 position, bool shouldInitialize = false)
    {
        NavConnection found = segmentConnections.Find((NavConnection conn) => Simplifications.Vector3ApproximationEquality(conn.IntersectionPosition, position)); //this probably can be optimized in the future
        if (found != null)
        {
            return found;
        }
        else
        {
            if (!shouldInitialize)
            {
                return null;
            }
            NavConnection newIntersection = new NavConnection();
            segmentConnections.Add(newIntersection);
            newIntersection.initialize(position);
            return newIntersection;
        }
    }

    private void AddCheckpoint(Vector3 position, NavCheckpoint checkpoint)
    {
        NavCheckpoint found = GetCheckpoint(position);
        if (found != null)
        {
            GD.PrintErr($"Checkpoint already exists at position: {position}");
        }
        else
        {
            checkpoints.Add(checkpoint);
        }
    }


    // Debug
    public void DebugDrawGraph()
    {
        foreach(NavSegment segment in segments)
        {
            Debugger3D.main.CurveEffect(segment.GlobalStart, segment.GlobalEnd, segment.GlobalControl, Colors.Red, 5);
        }

        foreach(NavConnection connector in segmentConnections)
        {
            Debugger3D.main.SphereEffect(connector.IntersectionPosition, 0.5f, Colors.Black, 0.3f, 5);
        }

        foreach(NavCheckpoint checkpoint in checkpoints)
        {
            Debugger3D.main.SphereEffect(checkpoint.GlobalPosition, 0.7f, Colors.Orange, 0.3f, 5);
        }
    }


    // Data Retrieval
    public NavCheckpoint GetRandomCheckpoint(RandomNumberGenerator rng)
    {
        return checkpoints.ToList()[rng.RandiRange(0, checkpoints.Count - 1)];
    }

    public NavCheckpoint[] GetTwoRandomCheckpoints(RandomNumberGenerator rng)
    {
        // If less than 2 items, we return nothing
        if (checkpoints.Count < 2)
        {
            return new NavCheckpoint[0];
        }
        else
        {
            int first = rng.RandiRange(0, checkpoints.Count - 1);
            int second = rng.RandiRange(0, checkpoints.Count - 1);

            // If we got the same, tries moving the second one below or above (depending on where there's more space)
            if (second == first)
            {
                if (second - 1 < 0)
                {
                    second += 1;
                }
                else
                {
                    second -= 1;
                }
            }

            // Returns
            return new NavCheckpoint[2] { checkpoints[first], checkpoints[second] };
        }
    }

    public NavCheckpoint GetCheckpoint(Vector3 position)
    {
        return checkpoints.Find((NavCheckpoint conn) => Simplifications.Vector3ApproximationEquality(conn.GlobalPosition, position));
    }

    public List<NavSegment> GetStartingSegments(Vector3 position)
    {

        return GetIntersectionAt(position)?.OutboundSegments ?? new List<NavSegment>();
    }

    public List<NavSegment> GetEndingSegments(Vector3 position)
    {
        return GetIntersectionAt(position)?.InboundSegments ?? new List<NavSegment>();
    }


}
