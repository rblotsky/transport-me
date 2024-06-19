using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class NavGraph : Node
{
    // DATA //
    // Internal graph data
    public List<NavNode> nodes = new List<NavNode>();
    public List<NavSegment> segments = new List<NavSegment>();

    // Scene references
    [Export] private MeshInstance3D graphMeshInstance;
    [Export] private Material drawMaterial;


    // CONSTRUCTORS //


    // FUNCTIONS //
    // Godot Functions
    public override void _Process(double delta)
    {
        
        base._Process(delta);
    }


    // Managing display
    private void UpdateMesh()
    {
        ImmediateMesh mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Points, drawMaterial);
        foreach(NavNode node in nodes)
        {
            mesh.SurfaceAddVertex(node.GlobalPosition);
        }
        mesh.SurfaceEnd();

        graphMeshInstance.Mesh = mesh;
    }

    // Managing Structure
    public void AddNode(NavNode newNode)
    {
        nodes.Add(newNode);
    }

    public void AddSegment(NavSegment segment)
    {
        segment.ConnectToEndpoints();
        segments.Add(segment);
    }

    public void RemoveSegment(NavSegment segment)
    {
        if(!segments.Contains(segment))
        {
            GD.PrintErr("Trying to a remove a segment that isn't in this graph:", segment);
            return;
        }
        segment.DisconnectFromEndpoints();
        segments.Remove(segment);
    }

    public void RemoveNodeAndConnections(NavNode node)
    {
        if (!nodes.Contains(node))
        {
            GD.PrintErr("Trying to a remove a node that isn't in this graph:", node);
            return;
        }
        
        // Removes all attached segments first
        NavSegment[] attachedSegments = node.attachedSegments.ToArray();
        foreach(NavSegment attached in attachedSegments)
        {
            RemoveSegment(attached);
        }

        // Removes itself afterwards
        nodes.Remove(node);
    }
}
