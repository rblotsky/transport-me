using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

public static class Simplifications
{
    // DATA //
    public static readonly Vector3 GRID_SNAP = new Vector3(1, 1, 1);


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

    /// <summary>
    /// Instantaneously checks for any objects colliding with a shape - use CreateCollisionQueryParameters to 
    /// specify a shape and location, as well as a collision mask.
    /// </summary>
    /// <param name="world">Generally, use GetWorld3D() on a Node3D.</param>
    /// <param name="scanParams">Run CreateCollisionQueryParameters</param>
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
	
    public static PhysicsRayQueryParameters3D CreateMousePosRaycastQuery(Camera3D camera, float distance)
    {
        Vector2 mousePosScreen = GetMousePosScreen(camera);

        PhysicsRayQueryParameters3D mouseRaycast = PhysicsRayQueryParameters3D.Create(
                        camera.ProjectRayOrigin(mousePosScreen),
                        camera.ProjectRayOrigin(mousePosScreen) + camera.ProjectRayNormal(mousePosScreen) * distance);

        return mouseRaycast;
    }


    // Mouse Tracking
    public static Vector2 GetMousePosScreen(Node worldNode)
    {
        return worldNode.GetViewport().GetMousePosition();
    }

    public static Vector3 GetWorldMousePosition(Camera3D usedCamera)
    {
        // Gets mouse position in the world
        Vector2 mousePosScreen = GetMousePosScreen(usedCamera);
        Plane placementPlane = Plane.PlaneXZ;
        Vector3? mousePosWorld =
            placementPlane.IntersectsRay(
            usedCamera.ProjectRayOrigin(mousePosScreen),
            usedCamera.ProjectRayNormal(mousePosScreen));


        // Returns the placement point (at height 0) or a zero vector if mouse position wasn't found
        if (mousePosWorld != null)
        {
            return new Vector3(mousePosWorld.Value.X, 0, mousePosWorld.Value.Z);
        }
        else
        {
            return Vector3.Zero;
        }
    }

    public static bool V3ApproximatelyEqual(Vector3 p1, Vector3 p2, float radius = 0.01f)
    {
        return (p1 - p2).Length() <= radius;
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


    // Miscellaneous
    public static Vector3I FloorVector3(Vector3 vec)
    {
        return (Vector3I)vec.Floor();
    }

    public static Vector3I SnapV3ToGrid(Vector3 vec)
    {
        return (Vector3I)vec.Snapped(GRID_SNAP);
    }

    public static void AddOwnedChild(Node parent, Node child)
    {
        parent.AddChild(child, true);
        child.Owner = parent.GetTree().CurrentScene;
    }

    public static void FreeOwnedNode(Node node)
    {
        // Removes owner from this node and its children
        node.Owner = null;

        foreach(Node childNode in GetChildrenOfNode(node, true))
        {
            childNode.Owner = null;
        }

        node.QueueFree();
    }
}
