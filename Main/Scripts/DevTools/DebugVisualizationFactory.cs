using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.Scripts.DevTools;
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
                Kind = DebugGeometryKind.StandardMesh,
                Mesh = DebugShapeLibrary.Get(DebugShapeType.Sphere),
                Transform = new Transform3D(Basis.Identity.Scaled(new Vector3(radius, radius, radius)), position),
                Type = DebugVisualizationType.Zone,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination | next),
            };
        }

        public static DebugVisualization Curve(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Vector3 control, Color colour, int numSegments = 10)
        {
            return new()
            {
                Kind = DebugGeometryKind.ImmediateMesh,
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination | next),
                Vertices = Curves.BezierQuadraticCurve3D(start, end, control, numSegments).ToList(),
                PrimitiveType = Mesh.PrimitiveType.Lines,
            };
        }

        public static DebugVisualization Arrow(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Color? colour, float alpha = 1f)
        {
            var arrowSize = (end - start).Length() / 2;
            // idk how to handle straight up...
            var sideDirectionXZ = Simplifications.GetVectorXZ((end - start).Normalized()).Orthogonal();
            Vector3 orthogonalSideDirection = new Vector3(sideDirectionXZ.X, 0f, sideDirectionXZ.Y);
            return new()
            {
                Kind = DebugGeometryKind.ImmediateMesh,
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination | next),
                Vertices = [start, end + orthogonalSideDirection * arrowSize, end - orthogonalSideDirection * arrowSize],
                PrimitiveType = Mesh.PrimitiveType.Triangles,
            };
        }

        public static DebugVisualization Line(IEnumerable<DebugVisualizationFilters> filters, Vector3 start, Vector3 end, Color colour, float alpha = 1f)
        {
            return new()
            {
                Kind = DebugGeometryKind.ImmediateMesh,
                Type = DebugVisualizationType.Line,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination | next),
                Vertices = [start, end],
                PrimitiveType = Mesh.PrimitiveType.Lines,
            };
        }

        public static DebugVisualization Box(IEnumerable<DebugVisualizationFilters> filters, Vector3 position, Quaternion rotation, Vector3 size, Color colour, float alpha = 1f)
        {
            return new()
            {
                Kind = DebugGeometryKind.StandardMesh,
                Type = DebugVisualizationType.Zone,
                Filters = filters.Aggregate(DebugVisualizationFilters.None, static (combination, next) => combination | next),
                Mesh = DebugShapeLibrary.Get(DebugShapeType.Box),
                Transform = new Transform3D(new Basis(rotation), position)
            };
        }
    }
}
