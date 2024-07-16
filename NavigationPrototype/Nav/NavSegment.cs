using Godot;
using System;

[GlobalClass]

public partial class NavSegment : Resource
{
    // DATA
    // Exported Properties
    [Export] public float Thickness { get; set; }

    // Instance Data
    [Export] private NavNode[] endpoints = new NavNode[2];

    /// <summary>
    /// Gives the endpoints in an array: [startNode, endNode]
    /// </summary>
    public NavNode[] Endpoints { get { return endpoints; } }
    public NavNode Start { get { return Endpoints[0]; } private set { Endpoints[0] = value; } }
    public NavNode End { get { return Endpoints[1]; } private set { Endpoints[1] = value; } }
    public Vector3 DirectionalLine { get { return End.GlobalPosition - Start.GlobalPosition; }}


    // INITIALIZATION //
    public void SetEndpoints(NavNode start, NavNode end)
    {
        Start = start;
        End = end;

        // Note: We do not call the node ConnectSegment functions. The constructor
        // should instantiate THIS object but not make any changes to OTHER objects.
    }


    // FUNCTIONS //
    // Data Retrieval
    public NavNode GetOtherEnd(NavNode oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return null;
    }

    // Managing Structure
    public void ConnectToEndpoints()
    {
        Start.ConnectSegment(this);
        End.ConnectSegment(this);
    }

    public void DisconnectFromEndpoints()
    {
        Start.DisconnectSegment(this);
        End.DisconnectSegment(this);
    }


    // Managing Physical Representation
    public void SetPositionAndRotation()
    {
        GlobalPosition = Start.GlobalPosition + (DirectionalLine / 2);
        LookAt(Vector3.Down+GlobalPosition, DirectionalLine);
    }
    
    public void CreatePhysicalRepresentation()
    {
        // Gives itself a mesh and collider
        EasyShapes.AddShapeMesh(this, EasyShapes.CapsuleMesh(Thickness, DirectionalLine.Length()), true);
        CollisionObject3D collider = EasyShapes.AddShapeCollider(
            this, 
            EasyShapes.CapsuleShape(Thickness, DirectionalLine.Length()), 
            true
            );

        // Adds the endpoint indicator, moves it to the correct position
        MeshInstance3D endpointInstance = EasyShapes.AddShapeMesh(
            this,
            EasyShapes.SphereMesh(
                Thickness * 2,
                EasyShapes.ColouredMaterial(Colors.Red, 1)),
            true
            );
        endpointInstance.GlobalPosition = End.GlobalPosition - End.NodeRadius * DirectionalLine.Normalized();
        endpointInstance.Name = "LolEndpoint";
    }

    public void RemovePhysicalRepresentation()
    {
        Simplifications.FreeOwnedNode(Simplifications.GetFirstChildOfType<MeshInstance3D>(this));
        Simplifications.FreeOwnedNode(Simplifications.GetFirstChildOfType<MeshInstance3D>(this));
        Simplifications.FreeOwnedNode(Simplifications.GetFirstChildOfType<StaticBody3D>(this));
    }
}
