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


    // Managing Display
    public void CreatePhysicalRepresentation(float radius)
    {
        // Gives itself a mesh and collider
        MeshInstance3D meshInstance = new MeshInstance3D();
        SphereMesh sphereMesh = new SphereMesh();
        sphereMesh.Radius = radius;
        sphereMesh.Height = radius * 2.0f;
        meshInstance.Mesh = sphereMesh;
        AddChild(meshInstance);

        StaticBody3D colliderInstance = new StaticBody3D();
        CollisionShape3D collisionShape = new CollisionShape3D();
        SphereShape3D sphereShape = new SphereShape3D();
        sphereShape.Radius = radius;
        collisionShape.Shape = sphereShape;
        AddChild(colliderInstance);
        colliderInstance.AddChild(collisionShape);
    }

    public void RemovePhysicalRepresentation()
    {
        Simplifications.GetFirstChildOfType<MeshInstance3D>(this).Free();
        Simplifications.GetFirstChildOfType<StaticBody3D>(this).Free();
    }
}
