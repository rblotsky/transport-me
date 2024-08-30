using Godot;
using System;

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

    public static Vector3 BezierQuadratic3D(Vector3 p0, Vector2 p1, Vector3 p2, float t)
    {
        // Sets control to avg height before doing calculations
        Vector3 controlWithHeight = ControlAtAvgHeight(p0, p1, p2);

        Vector3 q0 = p0.Lerp(controlWithHeight, t);
        Vector3 q1 = controlWithHeight.Lerp(p2, t);

        return q0.Lerp(q1, t);
    }

    public static Vector2 BezierTangentQuadratic2D(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 abPoint = p0.Lerp(p1, t);
        Vector2 bcPoint = p1.Lerp(p2, t);

        return (bcPoint - abPoint).Normalized();
    }

    public static Vector3 BezierTangentQuadratic3D(Vector3 p0, Vector2 p1, Vector3 p2, float t)
    {
        // Sets control to avg height before doing calculations
        Vector3 controlWithHeight = ControlAtAvgHeight(p0, p1, p2);

        Vector3 abPoint = p0.Lerp(controlWithHeight, t);
        Vector3 bcPoint = controlWithHeight.Lerp(p2, t);

        return (bcPoint - abPoint).Normalized();
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
        return Vec2WithHeight(control, (end.Y - start.Y) / 2);
    }
}
