using Godot;
using System;

public static class EasyShapes
{
    // Creating Shapes and Meshes
    public static Material ColouredMaterial(Color colour, float alpha)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        colour.A = Mathf.Clamp(alpha, 0, 1);
        material.AlbedoColor = colour;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        return material;
    }

    public static SphereShape3D SphereShape(float radius)
    {
        SphereShape3D shape = new SphereShape3D();
        shape.Radius = radius;
        return shape;
    }

    public static CapsuleShape3D CapsuleShape(float radius, float height)
    {
        CapsuleShape3D shape = new CapsuleShape3D();
        shape.Radius = radius;
        shape.Height = height;
        return shape;
    }

    public static SphereMesh SphereMesh(float radius, Material materialToUse = null)
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

    public static ImmediateMesh LineMesh(Vector3 startLocal, Vector3 endLocal, Color colourToUse)
    {
        ImmediateMesh mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, ColouredMaterial(colourToUse, 1));
        mesh.SurfaceAddVertex(startLocal);
        mesh.SurfaceAddVertex(endLocal);
        mesh.SurfaceEnd();

        return mesh;
    }

    public static ImmediateMesh CurveMesh(Vector3 startLocal, Vector3 endLocal, Vector3 controlLocal, Color colourToUse, int segments)
    {        
        // Creates a Bezier curve
        ImmediateMesh mesh = new ImmediateMesh();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, ColouredMaterial(colourToUse, 1));
        mesh.SurfaceAddVertex(startLocal);

        // Loop through the curve, add a point for each increment
        for (int t = 1; t <= segments; t++)
        {

            // I add the vertex twice because every other line seems to be invisible,
            // and I bypassed that by just adding each line twice. I have no idea why this
            // happens and I do not care.
            mesh.SurfaceAddVertex(
                    CalculateBezierQuadraticWithHeight(
                        startLocal,
                        controlLocal,
                        endLocal,
                        t / (float)segments)
                );
            mesh.SurfaceAddVertex(
                    CalculateBezierQuadraticWithHeight(
                        startLocal,
                        controlLocal,
                        endLocal,
                        t / (float)segments)
                );
        }
        mesh.SurfaceAddVertex(endLocal);
        mesh.SurfaceEnd();

        return mesh;
    }

    public static CapsuleMesh CapsuleMesh(float radius, float height, Material materialToUse = null)
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
    public static CollisionObject3D AddShapeCollider(Node node, Shape3D shape, bool owned = false, bool isStatic = true)
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

    public static MeshInstance3D AddShapeMesh(Node node, Mesh mesh, bool owned = false)
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


    // Curve Calculations
    public static Vector2 CalculateBezierQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);

        Vector2 r = q0.Lerp(q1, t);
        return r;
    }

    public static Vector3 CalculateBezierQuadraticWithHeight(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float heightDifference = p2.Y - p0.Y;
        float pointHeight = heightDifference * t;

        // Returns a regular bezier with the added height
        return Vec2ToVec3(CalculateBezierQuadratic(Vec3ToVec2(p0), Vec3ToVec2(p1), Vec3ToVec2(p2), t), pointHeight);
    }

    public static Vector3 Vec2ToVec3(Vector2 vec, float height)
    {
        return new Vector3(vec.X, height, vec.Y);
    }

    public static Vector2 Vec3ToVec2(Vector3 vec)
    {
        return new Vector2(vec.X, vec.Z);
    }
}
