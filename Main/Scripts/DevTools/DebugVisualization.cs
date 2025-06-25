using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    public enum DebugGeometryKind
    {
        StandardMesh,
        ImmediateMesh
    }
    public struct DebugVisualization
    {
        public DebugGeometryKind Kind;
        public Transform3D Transform;

        public DebugVisualizationType Type;
        public DebugVisualizationFilters Filters;
        public Color colour;


        public Mesh Mesh;
        public List<Vector3> Vertices;
        public Mesh.PrimitiveType PrimitiveType;
    }

    public interface IDebugVisualizationProvider
    {
        IEnumerable<DebugVisualization> GetVisualization();
        IEnumerable<DebugVisualization> GetVisualizationStatic() {
            yield break;
        }
    }
}
