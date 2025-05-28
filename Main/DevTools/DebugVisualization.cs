using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    public struct DebugVisualization
    {
        public Mesh Mesh;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public interface IDebugVisualizationProvider
    {
        IEnumerable<DebugVisualization> GetVisualization();
    }
}
