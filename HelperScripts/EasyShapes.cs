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
	/// <summary>
	/// Creates a triangle mesh, pointing in the direction of the two positions given.
	/// </summary>
	/// <param name="startLocal">Starting position</param>
	/// <param name="endLocal">ending position</param>
	/// <param name="colourToUse">Color of arrow</param>
	/// <param name="length">size of the arrow. Should use very small values</param>
	/// <returns></returns>
	public static ImmediateMesh TrianglePointerMesh(Color colourToUse, float size = 0.05f)
	{
		ImmediateMesh mesh = new ImmediateMesh();
		mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles, ColouredMaterial(colourToUse,1));

		Vector3 forward = Vector3.Forward * size;
		Vector3 side = Vector3.Right * size / 2;
		mesh.SurfaceAddVertex(-forward + side);
		mesh.SurfaceAddVertex(-forward - side);
		mesh.SurfaceAddVertex(forward);
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
					Curves.CalculateBezierQuadraticWithHeight(
						startLocal,
						controlLocal,
						endLocal,
						t / (float)segments)
				);
			mesh.SurfaceAddVertex(
					Curves.CalculateBezierQuadraticWithHeight(
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
}
