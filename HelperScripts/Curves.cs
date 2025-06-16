using Godot;
using System;
using System.Collections.Generic;
public static class Curves
{

    // FUNCTIONS //
    // Curve Calculations
    public static Vector2 BezierQuadratic2D(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);

        Vector2 r = q0.Lerp(q1, t);
        return r;
    }

    /// <summary>
    /// Generates a bezier quadratic
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 BezierQuadratic3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 q0 = p0.Lerp(p1, t);
        Vector3 q1 = p1.Lerp(p2, t);

        return q0.Lerp(q1, t);
    }

    /// <summary>
    /// Generates and returns all points along a bezier curve
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <param name="segments">Number of points to generate</param>
    /// <returns></returns>
    public static IEnumerable<Vector3> BezierQuadraticCurve3D(Vector3 start, Vector3 end, Vector3 control, int segments = 10)
    {
        yield return start;
        // Loop through the curve, add a point for each increment
        for (int t = 1; t <= segments; t++)
        {
            yield return BezierQuadratic3D(
                start,
                control,
                end,
                t / (float)segments);
        }
    }

    public static Vector2 BezierTangentQuadratic2D(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 abPoint = p0.Lerp(p1, t);
        Vector2 bcPoint = p1.Lerp(p2, t);

        return (bcPoint - abPoint).Normalized();
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

    public static Vector3 BezierQuadratic3DWithOffset(Vector3 p0, Vector3 p1, Vector3 p2, float startOffset, float endOffset, float t)
    {
        float offsetAtPoint = Mathf.Lerp(startOffset, endOffset, t);
        Vector3 curvePoint = BezierQuadratic3D(p0, p1, p2, t);
        Vector3 rightVector = BezierRightVectorQuadratic3D(p0, p1, p2, t);

        return rightVector * offsetAtPoint + curvePoint;
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

    public static Vector3 ControlAtAvgHeight(Vector3 start, Vector2 control, Vector3 end)
    {
        return Vec2WithHeight(control, ((end.Y - start.Y) / 2) + start.Y);
        
    }
}
