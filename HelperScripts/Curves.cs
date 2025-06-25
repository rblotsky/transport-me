using Godot;
using System;
using System.Collections.Generic;
public static class Curves
{

    // FUNCTIONS //
    // Curve Calculations
    public static Vector3 BezierQuadratic3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 q0 = p0.Lerp(p1, t);
        Vector3 q1 = p1.Lerp(p2, t);

        return q0.Lerp(q1, t);
    }

    public static Vector3 BezierTangentQuadratic3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 abPoint = p0.Lerp(p1, t);
        Vector3 bcPoint = p1.Lerp(p2, t);

        return (bcPoint - abPoint).Normalized();
    }

    public static Vector3 BezierNormalQuadratic3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 tangent = BezierTangentQuadratic3D(p0, p1, p2, t);
        Vector3 upVector = new Plane(p0, p1, p2).Normal;
        return tangent.Rotated(upVector, Mathf.DegToRad(90));
    }

    /// <summary>
    /// Generates a unit vector perpendicular to the given point on a bezier curve, 
    /// projected to the XZ plane.
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 BezierRightVectorQuadratic3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 tangentOnXZPlane = Plane.PlaneXZ.Project(BezierTangentQuadratic3D(p0, p1, p2, t));
        return tangentOnXZPlane.Rotated(Vector3.Up, Mathf.DegToRad(90));
    }

    public static Vector3 BezierQuadraticWithOffset3D(Vector3 p0, Vector3 p1, Vector3 p2, float startOffset, float endOffset, float t)
    {
        float offsetAtPoint = Mathf.Lerp(startOffset, endOffset, t);
        Vector3 curvePoint = BezierQuadratic3D(p0, p1, p2, t);
        Vector3 rightVector = BezierRightVectorQuadratic3D(p0, p1, p2, t);

        return rightVector * offsetAtPoint + curvePoint;
    }


    // Curve Point Methods
    /// <summary>
    /// Generates a list of points on a 3D quadratic bezier curve, essentially turning
    /// the continuous curve into a discrete one. 
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="numPoints">Number of points to generate. Minimum value of 2.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If given a point value less than 2.</exception>
    public static Vector3[] BezierQuadratic3DToPoints(Vector3 p0, Vector3 p1, Vector3 p2, int numPoints)
    {
        if (numPoints < 2)
        {
            throw new ArgumentException("numPoints must be 2 or greater.");
        }

        Vector3[] points = new Vector3[numPoints];

        for(int i = 0; i < numPoints; i++)
        {
            if (i == 0)
            {
                points[i] = BezierQuadratic3D(p0, p1, p2, 0);
            }
            else if (i == numPoints - 1)
            {
                points[i] = BezierQuadratic3D(p0, p1, p2, 1);
            }
            else
            {
                points[i] = BezierQuadratic3D(p0, p1, p2, (float)i/(numPoints-1));
            }
        }

        return points;
    }

    public static Vector3[] BezierQuadraticWithOffset3DToPoints(Vector3 p0, Vector3 p1, Vector3 p2, float startOffset, float endOffset, int numPoints)
    {
        if (numPoints < 2)
        {
            throw new ArgumentException("numPoints must be 2 or greater.");
        }

        Vector3[] points = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            if (i == 0)
            {
                points[i] = BezierQuadraticWithOffset3D(p0, p1, p2, startOffset, endOffset, 0);
            }
            else if (i == numPoints - 1)
            {
                points[i] = BezierQuadraticWithOffset3D(p0, p1, p2, startOffset, endOffset, 1);
            }
            else
            {
                points[i] = BezierQuadraticWithOffset3D(p0, p1, p2, startOffset, endOffset, (float)i / (numPoints - 1));
            }
        }

        return points;
    }


    // Helpers
    public static Vector3 Vec2WithHeight(Vector2 vec, float height)
    {
        return new Vector3(vec.X, height, vec.Y);
    }

    public static Vector2 Vec3RemoveHeight(Vector3 vec)
    {
        return new Vector2(vec.X, vec.Z);
    }
}
