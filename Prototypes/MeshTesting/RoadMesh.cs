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
    public ImmediateMesh GenerateRoadNormalsMesh(CurvedRoad road)
    {
        // Creates a regular mesh to get normals from
        ArrayMesh generatedMesh = GenerateRoadMesh(road);
        MeshDataTool meshData = new MeshDataTool();
        meshData.CreateFromSurface(generatedMesh, 0);

        // Creates the normals mesh
        ImmediateMesh normalsMesh = new ImmediateMesh();
        normalsMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        // Creates a line for every normal
        for (int v = 0; v <= meshData.GetVertexCount(); v++)
        {
            Vector3 pointVert = meshData.GetVertex(v);
            Vector3 normal3D = meshData.GetVertexNormal(v);

            normalsMesh.SurfaceAddVertex(pointVert);
            normalsMesh.SurfaceAddVertex(normal3D.Normalized() + pointVert);
        }

        // Finishes and returns
        normalsMesh.SurfaceEnd();

        return normalsMesh;
    }

    public ArrayMesh GenerateRoadMesh(CurvedRoad road)
    {
        // Creates a surface array and lists of values to assign to it
        Array surfaceArray = new Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);


        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        // Extrudes the slice along the curve for each segment
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

            Transform3D sliceTransform = new Transform3D(Basis.LookingAt(sliceFacing * new Vector3(1, 0, 1), road.Transform.Basis.Y), sliceOrigin);

            // Prepare UV values for this slice (u = 0, v += segment length)
            float uValue = 0;
            vValue += t / (float)numSegments;

            // Prepare cached values for storing previous vertex
            Vector3 previousPointVert = Vector3.Zero;
            float previousPointU = 0;

            // Create vertices for this specific slice
            for (int i = 0; i < points.Count; i++)
            {
                // Prepare the point, U value and normal for this vertex
                Vector3 pointVert = sliceTransform * new Vector3(points[i].X, points[i].Y, 0);

                // Increment the current U value by the distance from the last point to this one, or 1 if at the end
                if (i != 0)
                {
                    uValue += (points[i] - points[i - 1]).Length() / CalculateSliceLength();
                }
                if(i == points.Count-1)
                {
                    uValue = 1;
                }                

                Vector2 pointNormal = CalculateNormal2D(i, i - 1);
                Vector3 normal3D = sliceTransform * new Vector3(pointNormal.X, pointNormal.Y, 0) - sliceOrigin;

                // Creates a copy of the previous point and creates this point (only if this isn't the first)
                if (i != 0)
                {
                    normals.Add(normal3D.Normalized());
                    uvs.Add(new Vector2(previousPointU, vValue));
                    verts.Add(previousPointVert);

                    normals.Add(normal3D.Normalized());
                    uvs.Add(new Vector2(uValue, vValue));
                    verts.Add(pointVert);
                }

                // Stores this vertex as the previous vertex
                previousPointVert = pointVert;
                previousPointU = uValue;
            }
        }

        // Connects vertices to form triangles
        int vertsPerSlice = points.Count * 2 - 2;
        for(int segmentIndex = 0; segmentIndex < numSegments; segmentIndex++)
        {
            for(int i = 0; i < vertsPerSlice; i+=2)
            {
                int startPoint = i + (segmentIndex * vertsPerSlice);

                // Formula for all regular points (except last slice)
                indices.Add(startPoint);
                indices.Add(startPoint + points.Count*2-2);
                indices.Add(startPoint+1);

                indices.Add(startPoint + points.Count*2 - 2);
                indices.Add(startPoint + points.Count*2 - 1);
                indices.Add(startPoint + 1);
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

        // Uses SurfaceTool to generate normals
        /*
        SurfaceTool surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(generatedMesh, 0);
        surfaceTool.GenerateNormals();
        generatedMesh = surfaceTool.Commit();
        */

        generatedMesh.SurfaceSetMaterial(0, meshMaterial);

        return generatedMesh;
    }


    private Vector2 CalculateNormal2D(int currentIndex, int previousIndex)
    {
        // Gets a normal vector, assuming LEFT is outside of the mesh (only works on clockwise meshes)
        Vector2 prevToCurrent = points[Simplifications.RealModulo(currentIndex,points.Count)] - 
            points[Simplifications.RealModulo(previousIndex, points.Count)];

        return prevToCurrent.PerpendicularCounterClockwise().Normalized();
    }

    private float CalculateSliceLength()
    {
        float totalLength = 0;
        for(int i = 1; i < points.Count; i++)
        {
            totalLength += points[i].DistanceTo(points[i - 1]);
        }

        return totalLength;
    }
}
