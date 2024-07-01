using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class NavNode : Node3D
{
    // DATA //
    [Export] private Vector3 setPosition;
    [Export] public Array<NavSegment> attachedSegments = new Array<NavSegment>();
    public NavSegment[] StartingSegments
    {
        get
        {
            return attachedSegments.Where((NavSegment segment) => { return segment.Start == this; }).ToArray();
        }
    }
    public NavSegment[] EndingSegments
    {
        get
        {
            return attachedSegments.Where((NavSegment segment) => { return segment.End == this; }).ToArray();
        }
    }


    // INITIALIZING //
    public void SetPosition(Vector3 pos)
    {
        setPosition = pos;
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
        Simplifications.AddOwnedChild(this, meshInstance);

        StaticBody3D colliderInstance = new StaticBody3D();
        CollisionShape3D collisionShape = new CollisionShape3D();
        SphereShape3D sphereShape = new SphereShape3D();
        sphereShape.Radius = radius;
        collisionShape.Shape = sphereShape;
        Simplifications.AddOwnedChild(this, colliderInstance);
        Simplifications.AddOwnedChild(colliderInstance, collisionShape);
    }

    public void RemovePhysicalRepresentation()
    {
        Simplifications.GetFirstChildOfType<MeshInstance3D>(this).Free();
        Simplifications.GetFirstChildOfType<StaticBody3D>(this).Free();
    }
}
