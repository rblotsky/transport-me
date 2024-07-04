using Godot;
using System;

[GlobalClass]

public partial class NavSegment : Node3D
{
    // DATA //
    [Export] private NavNode startNode;
    [Export] private NavNode endNode;
    [Export] public float Thickness { get; set; }

    /// <summary>
    /// Gives the endpoints in an array: [startNode, endNode]
    /// </summary>
    public NavNode[] Endpoints { get { return new NavNode[] { startNode, endNode }; } }
    public NavNode Start { get { return startNode; } }
    public NavNode End { get { return endNode; } }
    public Vector3 DirectionalLine { get { return endNode.GlobalPosition - startNode.GlobalPosition; } set { } }

    // INITIALIZATION //
    public void SetStartEnd(NavNode start, NavNode end)
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
        SetPositionAndRotation();
        base._Ready();
    }


    // Data Retrieval
    public NavNode GetOtherEnd(NavNode oneEnd)
    {
        if (startNode == oneEnd) return endNode;
        else if (endNode == oneEnd) return startNode;
        else return null;
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


    // Managing Physical Representation
    private void SetPositionAndRotation()
    {
        GlobalPosition = startNode.GlobalPosition + (DirectionalLine / 2);
        LookAt(Vector3.Down+GlobalPosition, DirectionalLine);
    }
    
    public void CreatePhysicalRepresentation()
    {
        // Gives itself a mesh and collider and a cone at the end
        MeshInstance3D meshInstance = new MeshInstance3D();
        CapsuleMesh capsule = new CapsuleMesh();
        capsule.Radius = Thickness;
        capsule.Height = DirectionalLine.Length();
        meshInstance.Mesh = capsule;
        Simplifications.AddOwnedChild(this, meshInstance);

        MeshInstance3D endpointInstance = new MeshInstance3D();
        SphereMesh endpointMesh = new SphereMesh();
        endpointMesh.Radius = Thickness * 2;
        endpointMesh.Height = Thickness * 4;
        StandardMaterial3D endpointMaterial = new StandardMaterial3D();
        endpointMaterial.AlbedoColor = Colors.Red;
        endpointMesh.Material = endpointMaterial;
        endpointInstance.Mesh = endpointMesh;
        Simplifications.AddOwnedChild(this, endpointInstance);
        endpointInstance.GlobalPosition = End.GlobalPosition - End.NodeRadius * DirectionalLine.Normalized();

        StaticBody3D colliderInstance = new StaticBody3D();
        CollisionShape3D collisionShape = new CollisionShape3D();
        CapsuleShape3D capsuleShape = new CapsuleShape3D();
        capsuleShape.Radius = Thickness;
        capsuleShape.Height = DirectionalLine.Length();
        collisionShape.Shape = capsuleShape;
        Simplifications.AddOwnedChild(this, colliderInstance);
        Simplifications.AddOwnedChild(colliderInstance, collisionShape);
    }

    public void RemovePhysicalRepresentation()
    {
        Simplifications.GetFirstChildOfType<MeshInstance3D>(this).Free();
        Simplifications.GetFirstChildOfType<StaticBody3D>(this).Free();
    }
}
