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
    private Dictionary<Vector3I, List<NavSegment>> segmentConnections; // Optimization: Maybe make it a list of 4, will there ever be more than 4?
    private Dictionary<Vector3I, NavCheckpoint> checkpoints;
    private List<NavSegment> segments;
    public bool isGraphReady = false;


    // FUNCTIONS //
    public override void _Ready()
    {
        isGraphReady = false;
        SetupNavGraph();
        CollapseSegments();
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
        segmentConnections = new Dictionary<Vector3I, List<NavSegment>>();
        checkpoints = new Dictionary<Vector3I, NavCheckpoint>();
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

    private void CollapseSegments()
    {
        NavSegment[] foundSegments = Simplifications.GetChildrenOfType<NavSegment>(this, true).ToArray();
        foreach(NavSegment segment in foundSegments)
        {
            segments.Add(segment);
            AddConnectedSegment(segment.GlobalStart, segment);
            AddConnectedSegment(segment.GlobalEnd, segment);
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

    private void AddConnectedSegment(Vector3I position, NavSegment segment)
    {
        if (segmentConnections.TryGetValue(position, out List<NavSegment> existing))
        {
            existing.Add(segment);
        }
        else
        {
            List<NavSegment> newList = new List<NavSegment>();
            newList.Add(segment);
            segmentConnections.Add(position, newList);
        }
    }

    private void AddCheckpoint(Vector3I position, NavCheckpoint checkpoint)
    {
        if (checkpoints.ContainsKey(position))
        {
            GD.PrintErr($"Checkpoint already exists at position: {position}");
        }
        else
        {
            checkpoints.Add(position, checkpoint);
        }
    }


    // Debug
    public void DebugDrawGraph()
    {
        foreach(NavSegment segment in segments)
        {
            Debugger3D.main.CurveEffect(segment.GlobalStart, segment.GlobalEnd, segment.GlobalControl, Colors.Red, 5);
        }

        foreach(Vector3I connector in segmentConnections.Keys)
        {
            Debugger3D.main.SphereEffect(connector, 0.5f, Colors.Black, 0.3f, 5);
        }

        foreach(NavCheckpoint checkpoint in checkpoints.Values)
        {
            Debugger3D.main.SphereEffect(checkpoint.GlobalSnappedPos, 0.7f, Colors.Orange, 0.3f, 5);
        }
    }


    // Data Retrieval
    public NavCheckpoint GetRandomCheckpoint(RandomNumberGenerator rng)
    {
        return checkpoints.Values.ToList()[rng.RandiRange(0, checkpoints.Count - 1)];
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
            return new NavCheckpoint[2] { checkpoints.Values.ToList()[first], checkpoints.Values.ToList()[second] };
        }
    }

    public NavCheckpoint GetCheckpoint(Vector3I position)
    {
        if(checkpoints.TryGetValue(position, out NavCheckpoint found))
        {
            return found;
        }

        return null;
    }

    public List<NavSegment> GetConnections(Vector3I position)
    {
        if(segmentConnections.TryGetValue(position, out List<NavSegment> found))
        {
            return found;
        }

        return new List<NavSegment>();
    }

    public List<NavSegment> GetStartingConnections(Vector3I position)
    {
        List<NavSegment> foundSegments = GetConnections(position);
        return foundSegments.Where((NavSegment segment) => { return segment.GlobalStart == position; }).ToList();
    }

    public List<NavSegment> GetEndingConnections(Vector3I position)
    {
        List<NavSegment> foundSegments = GetConnections(position);
        return foundSegments.Where((NavSegment segment) => { return segment.GlobalEnd == position; }).ToList();
    }


}
