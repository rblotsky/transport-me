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
        private readonly List<MeshInstance3D> _visuals = new();
        private int _trackedVisuals = 0;
        public void Refresh()
        {
            //var thing2 = GetParent();
            //var thing3 = thing2.GetTree();
            //var thing4 = thing3.GetNodesInGroup("DebugVisualProvider");
            //var thing = ;
            //var thing = GetParent().GetTree().GetNodesInGroup("DebugVisualProvider");
            //GD.Print(thing.Count().ToString());
            //GD.Print(GetTree().GetNodesInGroup("DebugVisualProvider").OfType<IDebugVisualizationProvider>().Count().ToString());
            var currerntVisualCount = 0;
            foreach (var provider in Simplifications.GetChildrenImplementingType<IDebugVisualizationProvider>(GetParent(), true))
            {
                foreach(var visual in provider.GetVisualization())
                {
                    
                    if(currerntVisualCount >= _trackedVisuals)
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
                    else
                    {
                        var debugVisual = _visuals[currerntVisualCount];
                        debugVisual.Mesh = visual.Mesh;
                        debugVisual.Position = visual.Position;
                        debugVisual.Quaternion = visual.Rotation;
                    }
                    currerntVisualCount++;
                }
            }
            Myron(currerntVisualCount);
            _trackedVisuals = currerntVisualCount;
            
        }
        private void Myron(int numItemsAllocated)
        {
            if(numItemsAllocated >= _visuals.Count)
            {
                return;
            }

            for (int i = _visuals.Count - 1; i >= numItemsAllocated; i--) { 
                var v = _visuals[i];
                if (IsInstanceValid(v))
                {
                    v.QueueFree();
                }
                _visuals.RemoveAt(i);
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
