using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    public partial class DebugVisualizer: Node
    {
        private readonly List<Node3D> _visuals = new();
        public void Refresh()
        {
            ClearVisuals();
            var thing = GetParent().GetTree().GetNodesInGroup("DebugVisualProvider").OfType<IDebugVisualizationProvider>();

            GD.Print(thing.Count().ToString());
            GD.Print(GetTree().GetNodesInGroup("DebugVisualProvider").OfType<IDebugVisualizationProvider>().Count().ToString());

            foreach (var provider in GetParent().GetTree().GetNodesInGroup("DebugVisualProvider").OfType<IDebugVisualizationProvider>())
            {
                foreach(var visual in provider.GetVisualization())
                {
                    MeshInstance3D debugVisual = new MeshInstance3D
                    {
                        Mesh = visual.Mesh,
                        Position = visual.Position,
                        Quaternion = visual.Rotation,
                    };

                    _visuals.Add(debugVisual);
                    AddChild(debugVisual);
                }
            }
        }

        private void ClearVisuals()
        {
            foreach (var v in _visuals)
            {
                if (IsInstanceValid(v))
                    v.QueueFree();
            }
            _visuals.Clear();
        }
    }
}
