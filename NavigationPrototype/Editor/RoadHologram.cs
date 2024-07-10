using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class RoadHologram : Node3D
{
    // DATA //
    // Node Data
    private Array<Vector3> nodePositions = new Array<Vector3>(new Vector3[4]
    {
        new Vector3(-2, 0, 2),
        new Vector3(-2, 0, -2),
        new Vector3(2, 0, 2),
        new Vector3(2, 0, -2)
    });
    private Array<Vector2I> segments = new Array<Vector2I>(new Vector2I[4]
    {
        new Vector2I(0, 2),
        new Vector2I(0, 3),
        new Vector2I(1, 3),
        new Vector2I(1, 2),
    });

    // Properties
    public float CatchmentRadius { get; set; }
    public Color Colour { get; set; }
    public float Alpha { get; set; }
  


    // CONSTRUCTOR //
    public RoadHologram()
    {
        // Sets default values
        CatchmentRadius = 1;
        Colour = Colors.Blue;
        Alpha = 0.5f;
    }


    // FUNCTIONS //
    public override void _Ready()
    {
        CreatePhysicalRepresentation();
        base._Ready();
    }


    public void CreatePhysicalRepresentation()
    {
        // Adds a bunch of shapes representing itself
        foreach(Vector3 nodePos in nodePositions)
        {
            MeshInstance3D addedMesh = EasyShapes.AddGenericShapeMesh(
                this, 
                EasyShapes.CreateSphereMesh(
                    CatchmentRadius, 
                    EasyShapes.CreateMaterial(Colour, 0.5f)
                    )
                );

            // Sets the position
            addedMesh.Position = nodePos;
        }

        foreach(Vector2I segment in segments)
        {
            MeshInstance3D addedMesh = EasyShapes.AddGenericShapeMesh(
                this,
                EasyShapes.CreateCapsuleMesh(
                    0.2f,
                    GetSegmentLength(segment),
                    EasyShapes.CreateMaterial(Colour, 0.5f)
                    )
                );

            addedMesh.Position = nodePositions[segment.X] + GetLineBetweenNodes(segment) / 2;
            addedMesh.LookAt(Vector3.Down + GlobalPosition, GetLineBetweenNodes(segment));
        }
    }

    public void InstantiateRealRoad(Node container, NavGraph navGraph)
    {
        // First, create a dummy object to hold the nodes (important for future implementation of removing
        // an entire road square at a time)
        Node3D roadNode = new Node3D();
        Simplifications.AddOwnedChild(container, roadNode);
        roadNode.GlobalPosition = GlobalPosition;

        // Now, we add all of the NavNodes we created to the world, or connect to ones in the 
        // catchment radius
        NavNode[] connectableNodes = new NavNode[nodePositions.Count];
        for(int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 nodePos = nodePositions[i];

            // Tries catching a node to connect to. If none found, creates a new one.
            NavNode newNode = TryCatchingNode(ToGlobal(nodePos));

            if (newNode == null)
            {
                newNode = new NavNode();
                Simplifications.AddOwnedChild(roadNode, newNode);
                navGraph.AddNode(newNode, ToGlobal(nodePos));
            }

            connectableNodes[i] = newNode;
        }

        // Now create some segments and connect them
        foreach(Vector2I segment in segments)
        {
            // Ignores if theres already a segment here
            if (!connectableNodes[segment.X].ConnectedTo(connectableNodes[segment.Y]))
            {
                NavSegment newSegment = new NavSegment();
                Simplifications.AddOwnedChild(roadNode, newSegment);
                newSegment.SetEndpoints(connectableNodes[segment.X], connectableNodes[segment.Y]);
                navGraph.AddSegment(newSegment);
            }
        }
    }

    private NavNode TryCatchingNode(Vector3 origin)
    {
        CollisionObject3D[] foundCollisions = Simplifications.QueryCollidingObjects(
            GetWorld3D(), 
            Simplifications.CreateCollisionQuerySphere(CatchmentRadius, origin)
            );

        // Looks through all collisions with the catchment sphere to find NavNodes in it.
        foreach(CollisionObject3D collision in foundCollisions)
        {
            if(collision.GetParent() is NavNode foundNode)
            {
                return foundNode;
            }
        }

        return null;
    }

    private Vector3 GetLineBetweenNodes(Vector2I segment)
    {
        return nodePositions[segment.Y] - nodePositions[segment.X];
    }

    private float GetSegmentLength(Vector2I segment)
    {
        return GetLineBetweenNodes(segment).Length();
    }
}
