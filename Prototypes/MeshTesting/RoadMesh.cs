using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
[Tool]
public partial class RoadMesh : Resource
{
    // DATA //
    [Export] private Array<Vector2> points = new Array<Vector2>();
    [Export] private int numSegments = 9;
    [Export] private Material meshMaterial;


    // FUNCTIONS //
    // Mesh Generation
    public ArrayMesh GenerateRoadMesh(CurvedRoad road)
    {
        // Creates a surface array and lists of values to assign to it
        Array surfaceArray = new Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);


        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        // Creates all the vertices with their associated values
        float vValue = 0;
        for (int t = 0; t <= numSegments; t++)
        {
            // Get a transform for this point on the curve
            Vector3 sliceOrigin = Curves.BezierQuadratic3D(
                road.Start, 
                road.Control, 
                road.End, 
                t / (float)numSegments
                );

            Vector3 sliceFacing = Curves.BezierTangentQuadratic3D(
                road.Start,
                road.Control,
                road.End,
                t / (float)numSegments);

            if (t == 0)
            {
                GD.Print($"At first point. Value is {t / (float)numSegments}! Facing: {sliceFacing}");
            }
            if (t == numSegments)
            {
                GD.Print($"At last point. Value is {t / (float)numSegments}! Facing: {sliceFacing}");
            }
            //Debugger3D.main.SphereEffect(road.ToGlobal(sliceOrigin), 0.1f, Colors.Orange, 1, 20);
            //Debugger3D.main.LineEffect(road.ToGlobal(sliceOrigin), road.ToGlobal(sliceOrigin + (sliceFacing)), Colors.Orange, 20);

            Transform3D sliceTransform = new Transform3D(Basis.LookingAt(sliceFacing), sliceOrigin);

            // Prepare UV values for this slice (u = 0, v += segment length)
            float uValue = 0;
            vValue += t / (float)numSegments;

            // Assign vertices for this specific slice
            for(int i = 0; i < points.Count; i++)
            {
                // Put the point as a v3 and transform it to the space of the point on the curve
                Vector3 pointRelToSlice = sliceTransform * new Vector3(points[i].X, points[i].Y, 0);
                Vector3 pointVert = pointRelToSlice;

                verts.Add(pointVert);
                //Debugger3D.main.SphereEffect(road.ToGlobal(pointVert + sliceOrigin), 0.1f, Colors.Lime, 1, 20);

                // Increment the current U value by the distance from the last point to this one, or 1 if at the end
                if (i != 0)
                {
                    uValue += (points[i] - points[i - 1]).Length();
                }
                if(i == points.Count-1)
                {
                    uValue = 1;
                }

                // Save the UV
                uvs.Add(new Vector2(uValue, vValue));

                // Set the normal to origin -> vertex, normalized
                normals.Add(pointVert.Normalized());
            }
        }

        // Connects vertices to form triangles
        
        for(int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++)
        {
            for(int i = 0; i < points.Count; i++)
            {
                int startPoint = i + (segmentIndex * points.Count);

                // Formula for all regular points (except last slice)
                if (i != points.Count - 1)
                {
                    indices.Add(startPoint);
                    indices.Add(startPoint + points.Count);
                    indices.Add(startPoint + 1);

                    indices.Add(startPoint + points.Count);
                    indices.Add(startPoint + points.Count + 1);
                    indices.Add(startPoint + 1);
                }

                // Formula for last slice
                else
                {
                    indices.Add(startPoint);
                    indices.Add(startPoint + points.Count);
                    indices.Add(segmentIndex * points.Count);

                    indices.Add(startPoint + points.Count);
                    indices.Add(startPoint + 1);
                    indices.Add(segmentIndex * points.Count);
                }
            }
        }
        

        // Convert Lists to arrays and assign to surface array
        surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

        // Commit to an ArrayMesh and return it
        ArrayMesh generatedMesh = new ArrayMesh();
        generatedMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        generatedMesh.RegenNormalMaps();
        generatedMesh.SurfaceSetMaterial(0, meshMaterial);

        return generatedMesh;
    }
}
