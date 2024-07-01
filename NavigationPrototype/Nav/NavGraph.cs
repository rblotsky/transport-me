using Godot;
using Godot.Collections;
using System;
using System.Linq;

[GlobalClass]

public partial class NavGraph : Node
{
    // DATA //
    // Graph data
    [Export] public Array<NavNode> nodes = new Array<NavNode>();
    [Export] public Array<NavSegment> segments = new Array<NavSegment>();

    // Scene references
    [Export] private MeshInstance3D graphMeshInstance;
    [Export] private Material drawMaterial;

    // Instance Data
    [Export] private float nodeRadius = 1;
    [Export] private float segmentThickness = 0.5f;


    // CONSTRUCTORS //


    // FUNCTIONS //
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


    // Checks
    /// <summary>
    /// Returns whether there exists a segment between A and B. Ignores direction.
    /// (A->B and B->A both count)
    /// </summary>
    public bool ExistsSegment(NavNode a, NavNode b)
    {
        foreach(NavSegment segment in a.attachedSegments)
        {
            if(segment.Endpoints.Contains(b))
            {
                return true;
            }
        }

        return false;
    }

    // Managing Structure
    public void AddNode(NavNode newNode)
    {
        Simplifications.AddOwnedChild(this, newNode);
        nodes.Add(newNode);
        newNode.CreatePhysicalRepresentation(nodeRadius);
    }

    public void AddSegment(NavSegment segment)
    {
        segment.ConnectToEndpoints();
        Simplifications.AddOwnedChild(this, segment);
        segments.Add(segment);
        segment.CreatePhysicalRepresentation(segmentThickness);
    }

    public void RemoveSegment(NavSegment segment)
    {
        if(!segments.Contains(segment))
        {
            GD.PrintErr("Trying to a remove a segment that isn't in this graph:", segment);
            return;
        }
        segment.DisconnectFromEndpoints();
        segment.RemovePhysicalRepresentation();
        segments.Remove(segment);
        Simplifications.FreeOwnedNode(segment);
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
        node.RemovePhysicalRepresentation();
        nodes.Remove(node);
        Simplifications.FreeOwnedNode(node);
    }
}
