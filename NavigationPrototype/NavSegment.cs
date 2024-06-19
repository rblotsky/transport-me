using Godot;
using System;

[GlobalClass]
public partial class NavSegment : Node3D
{
    // DATA //
    private NavNode startNode;
    private NavNode endNode;
    /// <summary>
    /// Gives the endpoints in an array: [startNode, endNode]
    /// </summary>
    public NavNode[] Endpoints { get { return new NavNode[] { startNode, endNode }; } }
    public Vector3 DirectionalLine { get { return endNode.Position - startNode.Position; } set { } }

    // CONSTRUCTORS //
    public NavSegment(NavNode start, NavNode end)
    {
        startNode = start;
        endNode = end;

        // Note: We do not call the node ConnectSegment functions. The constructor
        // should instantiate THIS object but not make any changes to OTHER objects.
    }


    // FUNCTIONS //
    // Godot Functions
    public override void _Ready()
    {
        GlobalPosition = DirectionalLine / 2;
        base._Ready();
    }


    // Managing Structure
    public void ConnectToEndpoints()
    {
        startNode.ConnectSegment(this);
        endNode.ConnectSegment(this);
    }

    public void DisconnectFromEndpoints()
    {
        startNode.DisconnectSegment(this);
        endNode.DisconnectSegment(this);
    }
}
