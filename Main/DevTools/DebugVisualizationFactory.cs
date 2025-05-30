using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector3 = Godot.Vector3;

namespace Transportme.Main.DevTools
{
    public static class DebugVisualizationFactory
    {
        public static DebugVisualization Sphere(Vector3 position, float radius, Color colour, float alpha = 1f)
        {
            return new()
            {
                Mesh = EasyShapes.SphereMesh(radius, EasyShapes.ColouredMaterial(colour, alpha)),
                Position = position,
                Rotation = Quaternion.Identity,
            };
        }

        public static DebugVisualization Curve(Vector3 start, Vector3 end, Vector3 control, Color colour, int numSegments = 10)
        {
            return new()
            {
                Mesh = EasyShapes.CurveMesh(start, end, control, colour, numSegments),
                Position = Vector3.Zero,
                Rotation = Quaternion.Identity,
            };
        }

        public static DebugVisualization Arrow(Vector3 start, Vector3 end, Color colour, float alpha = 1f)
        {
            var forward = (end - start).Normalized();
            return new()
            {
                Mesh = EasyShapes.TrianglePointerMesh(colour, alpha),
                Position = start.Lerp(end, 0.5f),
                Rotation = Simplifications.LookRotation(start, end),
            };
        }

        public static DebugVisualization Line(Vector3 start, Vector3 end, Color colour, float alpha = 1f)
        {
            return new()
            {
                Mesh = EasyShapes.LineMesh(start, end, colour),
                Position = Vector3.Zero,
                Rotation = Quaternion.Identity,
            };
        }

        public static DebugVisualization Box(Vector3 position, Quaternion rotation, Vector3? size, Color colour, float alpha = 1f)
        {
            return new()
            {
                Mesh = new BoxMesh
                {
                    Size = size ?? new Vector3(1, 1, 1),
                    Material = EasyShapes.ColouredMaterial(colour, alpha),
                },
                Position = position,
                Rotation = rotation,
            };
        }
    }
}
