using Godot;
using System;

public static class Curves
{

    // FUNCTIONS //
    // Curve Calculations
    public static Vector2 CalculateBezierQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);

        Vector2 r = q0.Lerp(q1, t);
        return r;
    }

    public static Vector3 CalculateBezierQuadraticIn3D(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    { 
        Vector3 q0 = p0.Lerp(p1, t);
        Vector3 q1 = p1.Lerp(p2, t);

        Vector3 r = q0.Lerp(q1, t);
        return r;
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
