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
    private List<NavConnection> connections;
    private List<NavCheckpoint> checkpoints;
    private List<NavSegment> segments;
    public bool isGraphReady = false;


    // FUNCTIONS //
    public override void _Ready()
    {
        InitializeLists();
        AssembleSegments(Simplifications.GetChildrenOfType<NavSegment>(this, true));
        AddAllCheckpoints(Simplifications.GetChildrenOfType<NavCheckpoint>(this, true));
        isGraphReady = true;
        base._Ready();
    }

    public override void _ExitTree()
    {
        isGraphReady = false;
        ClearNavGraph();
        base._ExitTree();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventKey keyInput)
        {
            if(keyInput.Keycode == Key.G && keyInput.IsPressed())
            {
                DebugDrawGraph();
            }
        }
        base._UnhandledInput(@event);
    }


    // Setup and Cleanup
    private void InitializeLists()
    {
        connections = new List<NavConnection>();
        checkpoints = new List<NavCheckpoint>();
        segments = new List<NavSegment>();
    }

    private void ClearNavGraph()
    {
        if(connections != null)
        {
            connections.Clear();
            connections = null;
        }

        if(segments != null)
        {
            segments.Clear();
            segments = null;
        }
    }

    private void AssembleSegments(IEnumerable<NavSegment> segmentsToAssemble)
    {
        NavSegment[] foundSegments = segmentsToAssemble.ToArray();
        foreach(NavSegment segment in foundSegments)
        {
            segments.Add(segment);

            NavConnection outbound = GetConnectionAtPosition(segment.GlobalStart, true);
            segment.StartConnection = outbound;
            outbound.AddOutbound(segment);

            NavConnection inbound = GetConnectionAtPosition(segment.GlobalEnd, true);
            segment.EndConnection = inbound;
            inbound.AddInbound(segment);
        }
    }

    private void AddAllCheckpoints(IEnumerable<NavCheckpoint> checkpoints)
    {
        NavCheckpoint[] newCheckpoints = checkpoints.ToArray();

        foreach (NavCheckpoint point in newCheckpoints)
        {
            AddCheckpoint(point.GlobalPosition, point);
        }
    }
    
    public NavConnection GetConnectionAtPosition(Vector3 position, bool shouldInitialize = false)
    {
        NavConnection found = connections.Find((NavConnection conn) => Simplifications.V3ApproximatelyEqual(conn.Position, position));
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
            NavConnection newConnection = new NavConnection();
            connections.Add(newConnection);
            newConnection.Position = position;
            return newConnection;
        }
    }

    private void AddCheckpoint(Vector3 position, NavCheckpoint checkpoint)
    {
        NavCheckpoint found = GetCheckpointAtPosition(position);
        NavConnection connection = GetConnectionAtPosition(position);
        if (connection == null)
        {
            GD.PrintErr($"NavGraphContainer > Cannot find NavConnection to bind to Checkpoint at position: {position}");
        }
        if (found != null)
        {
            GD.PrintErr($"NavGraphContainer > Checkpoint already exists at position: {position}");
        }
        else
        {
            checkpoint.AssociatedConnection = connection;
            checkpoints.Add(checkpoint);
        }
    }


    // Debug
    public void DebugDrawGraph()
    {
        foreach(NavSegment segment in segments)
        {
            Debugger3D.main.OrderedLinesEffect(segment.ConvertPointsToGlobal(segment.SubdivideIntoPoints(9)), Colors.Red, 5);
        }

        foreach(NavConnection connector in connections)
        {
            Debugger3D.main.SphereEffect(connector.Position, 0.5f, Colors.Black, 0.3f, 5);
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
            List<NavCheckpoint> tmp = checkpoints.Where(c => c.AssociatedConnection != null).ToList();
            int first = rng.RandiRange(0, tmp.Count - 1);
            int second = rng.RandiRange(0, tmp.Count - 1);

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
            return new NavCheckpoint[2] { tmp[first], tmp[second] };
        }
    }

    public NavCheckpoint GetCheckpointAtPosition(Vector3 position)
    {
        return checkpoints.Find((NavCheckpoint conn) => Simplifications.V3ApproximatelyEqual(conn.GlobalPosition, position));
    }
}
