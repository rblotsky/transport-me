using Godot;
using System;

public static class EasyShapes
{
    // Creating Shapes and Meshes
    public static Material CreateMaterial(Color colour, float alpha)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        colour.A = Mathf.Clamp(alpha, 0, 1);
        material.AlbedoColor = colour;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        return material;
    }

    public static SphereShape3D CreateSphereShape(float radius)
    {
        SphereShape3D shape = new SphereShape3D();
        shape.Radius = radius;
        return shape;
    }

    public static CapsuleShape3D CreateCapsuleShape(float radius, float height)
    {
        CapsuleShape3D shape = new CapsuleShape3D();
        shape.Radius = radius;
        shape.Height = height;
        return shape;
    }

    public static SphereMesh CreateSphereMesh(float radius, Material materialToUse = null)
    {
        SphereMesh mesh = new SphereMesh();
        mesh.Radius = radius;
        mesh.Height = radius * 2;

        if (materialToUse != null)
        {
            mesh.Material = materialToUse;
        }

        return mesh;
    }

    public static CapsuleMesh CreateCapsuleMesh(float radius, float height, Material materialToUse = null)
    {
        CapsuleMesh mesh = new CapsuleMesh();
        mesh.Radius = radius;
        mesh.Height = height;

        if (materialToUse != null)
        {
            mesh.Material = materialToUse;
        }

        return mesh;
    }

    public static CollisionShape3D CreateCollisionShape(Shape3D shapeToUse)
    {
        CollisionShape3D instance = new CollisionShape3D();
        instance.Shape = shapeToUse;
        return instance;
    }


    // Instantiating shapes into the scene
    public static CollisionObject3D AddGenericShapeCollider(Node node, Shape3D shape, bool owned = false, bool isStatic = true)
    {
        CollisionObject3D collider;
        if(isStatic)
        {
            collider = new StaticBody3D();
        }
        else
        {
            collider = new RigidBody3D();
        }
        CollisionShape3D collisionShape = CreateCollisionShape(shape);
        
        if(owned)
        {
            Simplifications.AddOwnedChild(node, collider);
            Simplifications.AddOwnedChild(collider, collisionShape);
        }
        else
        {
            node.AddChild(collider);
            collider.AddChild(collisionShape);
        }

        return collider;
    }

    public static MeshInstance3D AddGenericShapeMesh(Node node, Mesh mesh, bool owned = false)
    {
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;
        if (owned)
        {
            Simplifications.AddOwnedChild(node, meshInstance);
        }
        else
        {
            node.AddChild(meshInstance);
        }
        return meshInstance;
    }

}
