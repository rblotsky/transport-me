using Godot;
using System;

[GlobalClass]

public partial class NavSegment : Node3D
{
    // DATA //
    [Export] private NavNode startNode;
    [Export] private NavNode endNode;
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
    
    public void CreatePhysicalRepresentation(float thickness)
    {
        // Gives itself a mesh and collider
        MeshInstance3D meshInstance = new MeshInstance3D();
        CapsuleMesh capsule = new CapsuleMesh();
        capsule.Radius = thickness;
        capsule.Height = DirectionalLine.Length();
        meshInstance.Mesh = capsule;
        Simplifications.AddOwnedChild(this, meshInstance);

        StaticBody3D colliderInstance = new StaticBody3D();
        CollisionShape3D collisionShape = new CollisionShape3D();
        CapsuleShape3D capsuleShape = new CapsuleShape3D();
        capsuleShape.Radius = thickness;
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
