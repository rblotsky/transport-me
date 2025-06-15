using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    public partial class DebugVisualizer: Node
    {
        private readonly List<MeshInstance3D> _visuals = [];
        private int _trackedVisuals = 0;
        private readonly HashSet<DebugVisualizationType> _activeTypes = new();
        private DebugVisualizationFilters _activeFilters = 0;

        public HashSet<DebugVisualizationType> ActiveTypes { get { return _activeTypes; } }
        public DebugVisualizationFilters ActiveFilters { get { return _activeFilters; } set { _activeFilters = value; } }

        public List<IDebugVisualizationProvider> providersCache = [];
        public override void _Ready()
        {
            providersCache.AddRange(Simplifications.GetChildrenImplementingType<IDebugVisualizationProvider>(GetParent(), true));
            base._Ready();
        }
        public void Refresh()
        {
            var currerntVisualCount = 0;
            foreach (IDebugVisualizationProvider provider in providersCache)
            {
                foreach(var visual in provider.GetVisualization())
                {
                    // if type or filter doesn't match
                    if(!ActiveTypes.Contains(visual.Type) || ((_activeFilters & visual.Filters) == 0))
                    {
                        continue;
                    }

                    // allocate new mesh instances when there are none left to use
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
                        //update mesh to save on creating new meshes
                        var debugVisual = _visuals[currerntVisualCount];
                        debugVisual.Mesh = visual.Mesh;
                        debugVisual.Position = visual.Position;
                        debugVisual.Quaternion = visual.Rotation;
                    }
                    currerntVisualCount++;
                }
            }
            FreeMeshes(currerntVisualCount);
            _trackedVisuals = currerntVisualCount;
        }

        public override void _Process(double delta)
        {
            if(_activeTypes.Count > 0)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Bot Method
        /// <seealso cref="_visuals"/>
        /// </summary>
        /// <param name="numItemsAllocated"></param>
        private void FreeMeshes(int numItemsAllocated)
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
    }
}
