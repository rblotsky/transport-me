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
        private static readonly Mesh ArrowMesh = EasyShapes.TrianglePointerMesh(Colors.Red, 0.2f);
        public static DebugVisualization Sphere(IEnumerable<DebugVisualizationFilters> filters, Vector3 position, float radius, Color colour, float alpha = 1f)
        {
            return new()
            {
                Mesh = EasyShapes.SphereMesh(radius, EasyShapes.ColouredMaterial(colour, alpha)),
                Position = position,
                Rotation = Quaternion.Identity,
                Type = DebugVisualizationType.Zone,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination & next)
            };
        }

        public static DebugVisualization Curve(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Vector3 control, Color colour, int numSegments = 10)
        {
            return new()
            {
                Mesh = EasyShapes.CurveMesh(start, end, control, colour, numSegments),
                Position = Vector3.Zero,
                Rotation = Quaternion.Identity,
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination & next)
            };
        }

        public static DebugVisualization Arrow(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Color? colour, float alpha = 1f)
        {
            var forward = (end - start).Normalized();
            return new()
            {
                Mesh = colour != null ? EasyShapes.TrianglePointerMesh((Color)colour, alpha) : ArrowMesh,
                Position = start.Lerp(end, 0.5f),
                Rotation = Simplifications.LookRotation(start, end),
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination & next)
            };
        }

        public static DebugVisualization Line(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Color colour, float alpha = 1f)
        {
            return new()
            {
                Mesh = EasyShapes.LineMesh(start, end, colour),
                Position = Vector3.Zero,
                Rotation = Quaternion.Identity,
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination & next)
            };
        }

        public static DebugVisualization Box(IEnumerable<DebugVisualizationFilters> filters, Vector3 position, Quaternion rotation, Vector3? size, Color colour, float alpha = 1f)
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
                Type = DebugVisualizationType.Zone,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination & next)
            };
        }
    }
}
