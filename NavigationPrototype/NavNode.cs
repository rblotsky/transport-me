using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class NavNode : Node3D
{
    // DATA //
    private Vector3 setPosition;
    public List<NavSegment> attachedSegments = new List<NavSegment>();


    // CONSTRUCTORS //
    public NavNode(Vector3 position)
    {
        setPosition = position;
    }


    // FUNCTIONS //
    // Godot Functions
    public override void _Ready()
    {
        // Puts itself in the intended position
        GlobalPosition = setPosition;

        // Gives itself a mesh
        MeshInstance3D meshInstance = new MeshInstance3D();
        SphereMesh sphere = new SphereMesh();
        meshInstance.Mesh = sphere;
        AddChild(meshInstance);

        base._Ready();
    }


    // Managing Structure
    public void ConnectSegment(NavSegment segment)
    {
        attachedSegments.Add(segment);
    }

    public void DisconnectSegment(NavSegment segment)
    {
        attachedSegments.Remove(segment);
    }
}
