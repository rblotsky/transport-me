using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

public static class Simplifications
{

    // FUNCTIONS //
    // Collider detection
    private static PhysicsShapeQueryParameters3D CreateCollisionQueryParameters(Shape3D shape, Vector3 position, uint collisionMask)
    {
        // Creates the transform for the query
        Transform3D scanTransform = Transform3D.Identity;
        scanTransform.Origin = position;

        // Creates the physics query for the shape - this includes extra info like collision mask
        PhysicsShapeQueryParameters3D scanParams = new PhysicsShapeQueryParameters3D();
        scanParams.CollisionMask = collisionMask;
        scanParams.Shape = shape;
        scanParams.Transform = scanTransform;

        return scanParams;
    }

    public static PhysicsShapeQueryParameters3D CreateCollisionQuerySphere(float radius, Vector3 position, uint collisionMask = 0xffffffff)
    {
        // Creates a sphere to check
        SphereShape3D checkSphere = new SphereShape3D();
        checkSphere.Radius = radius;

        return CreateCollisionQueryParameters(checkSphere, position, collisionMask);
    }

    public static CollisionObject3D[] QueryCollidingObjects(World3D world, PhysicsShapeQueryParameters3D scanParams)
    {
        // Runs the query
        Array<Dictionary> intersections = world.DirectSpaceState.IntersectShape(scanParams);

        // Returns an array of all the colliders of the intersecting objects, or empty if none
        if (intersections.Count > 0)
        {
            CollisionObject3D[] colliders = new CollisionObject3D[intersections.Count];

            for(int i = 0; i < intersections.Count; i++)
            {
                colliders[i] = (CollisionObject3D)intersections[i]["collider"];
            }

            return colliders;
        }

        return new CollisionObject3D[0];
    }
	

    // Retrieving Nodes
    /// <summary>
    /// Gets all children of a node. Note: This includes internal children.
    /// </summary>
    public static List<Node> GetChildrenOfNode(Node parent, bool recursive = false)
    {
        List<Node> allChildren = new List<Node>(parent.GetChildren(true));

        // If it's recursive, add all children of children
        if (recursive)
        {
            Queue<Node> childrenToScan = new Queue<Node>(parent.GetChildren(false));

            while(childrenToScan.Count > 0)
            {
                Node currentChild = childrenToScan.Dequeue();
                Node[] currentChildChildren = currentChild.GetChildren(true).ToArray();

                allChildren.AddRange(currentChildChildren);

                foreach(Node child in currentChildChildren)
                {
                    childrenToScan.Enqueue(child);
                }
            }
        }

        return allChildren;
        
    }

    /// <summary>
    /// Gets all child nodes of a specific type. Note: This includes internal children.
    /// </summary>
    public static List<T> GetChildrenOfType<T>(Node parent, bool recursive = false) where T : Node
    {
        // Gets all children to operate on, then takes only the ones that match the right type.
        List<Node> children = GetChildrenOfNode(parent, recursive);
        List<T> childrenOfRightType = new List<T>();
        foreach(Node child in children)
        {
            if(child is T)
            {
                childrenOfRightType.Add((T)child);
            }
        }

        // Returns what it found
        children.Clear();
        return childrenOfRightType;
    }

    /// <summary>
    /// Gets the first child of the given type, or null if there isn't one.
    /// </summary>
    public static T GetFirstChildOfType<T>(Node parent, bool recursive = false) where T : Node
    {
        // Returns the first item from the get children function, or null if it returns an empty array.
        List<T> childrenOfType = GetChildrenOfType<T>(parent, recursive);

        if (childrenOfType.Count > 0) return childrenOfType[0];
        else return null;
    }

    /// <summary>
    /// Returns true if the first parent of this node is of the given type.
    /// </summary>
    public static bool IsParentOfType<T>(Node node) where T : Node
    {
        return (node.GetParent() != null) && (node.GetParent() is T);
    }
}
