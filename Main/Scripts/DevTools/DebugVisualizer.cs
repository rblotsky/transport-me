using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    /// <summary>
    /// A manager for most debug visualizations that occur during runtime. Any node which uses this must implement the 
    /// <seealso cref="IDebugVisualizationProvider"/> interface to provide the visualizations.
    /// 
    /// The main goal is to be able to implement a pretty intuitive system while reducing the amount of code within the components. This mostly applies to visualizations
    /// which require complex visualizations meant for code. 
    /// <br/>
    /// Visualizations are sorted into two types: standard and custom meshes.
    /// Standard meshes use meshes provided by <seealso cref="Scripts.DevTools.DebugShapeLibrary"/>, and renders all shapes under one multimesh
    /// Custom meshes use the primitive immediate meshes, and would be geometry such as lines, points and triangles.
    /// </summary>
    public partial class DebugVisualizer: Node
    {
        private readonly Dictionary<Mesh, MultiMeshInstance3D> debugStandardMeshes = new();
        private MeshInstance3D customMeshBatch;
        private StandardMaterial3D standardMaterial;

        private readonly HashSet<DebugVisualizationType> _activeTypes = new();
        private DebugVisualizationFilters _activeFilters = 0;

        public HashSet<DebugVisualizationType> ActiveTypes { get { return _activeTypes; } }
        public DebugVisualizationFilters ActiveFilters { get { return _activeFilters; } set { _activeFilters = value; } }

        public List<IDebugVisualizationProvider> providersCache = [];

        public void OnFilterToggled(DebugVisualizationFilters filter, bool toggledOn)
        {
            if (toggledOn) ActiveFilters |= filter;
            else ActiveFilters &= ~filter;
        }

        public void OnTypeToggled(DebugVisualizationType type, bool toggledOn)
        {
            if(toggledOn) ActiveTypes.Add(type);
            else ActiveTypes.Remove(type);
        }


        public override void _Ready()
        {
            providersCache.AddRange(Simplifications.GetChildrenImplementingType<IDebugVisualizationProvider>(GetParent(), true));
            customMeshBatch = new MeshInstance3D();
            ArrayMesh mesh = new();
            var mat = new StandardMaterial3D();
            mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            mat.AlbedoColor = new Color(1,1,1);
            mat.VertexColorUseAsAlbedo = true;
            standardMaterial = mat;
            customMeshBatch.Mesh = mesh;
            AddChild(customMeshBatch);
            base._Ready();
        }

        private bool shouldRender(DebugVisualization vis)
        {
            return _activeTypes.Contains(vis.Type) && (_activeFilters & vis.Filters) != 0;
        }

        private void ClearVisuals()
        {
            (customMeshBatch.Mesh as ArrayMesh).ClearSurfaces();
            foreach(MultiMeshInstance3D multiMesh in debugStandardMeshes.Values)
            {
                multiMesh.Multimesh.InstanceCount = 0;
            }

        }
        public void Refresh()
        {
            ClearVisuals();
            List < DebugVisualization > debugVisuals = [];

            foreach (IDebugVisualizationProvider provider in providersCache)
            {
                debugVisuals.AddRange(provider.GetVisualization());
            }

            // filter out any things which shouldn't render, then group them by their type (kind) as well as their kind specific properties
            // then arrange them for processing
            var debugVisualGroups = debugVisuals
                .Where(x=> shouldRender(x))
                .GroupBy(x => new { x.Kind, x.PrimitiveType, x.Mesh })
                .Select(g => new { Keys = g.Key, DebugVisuals = g.ToList() });

            foreach (var debugVisualGroup in debugVisualGroups)
            {
                //standard meshes can be combined into mesh specific multi-meshes for fast computing
                if(debugVisualGroup.Keys.Kind == DebugGeometryKind.StandardMesh)
                {
                    //if we havne't instantiated the multi-mesh for this specific mesh
                    if (!debugStandardMeshes.ContainsKey(debugVisualGroup.Keys.Mesh))
                    { 
                        MultiMesh mesh = new MultiMesh();
                        mesh.UseColors = true;
                        mesh.Mesh = debugVisualGroup.Keys.Mesh;
                        mesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
                        mesh.InstanceCount = debugVisualGroup.DebugVisuals.Count; //initial count

                        var multiMeshInstance3D = new MultiMeshInstance3D();
                        multiMeshInstance3D.Multimesh = mesh;

                        debugStandardMeshes[debugVisualGroup.Keys.Mesh] = multiMeshInstance3D;
                        AddChild(debugStandardMeshes[debugVisualGroup.Keys.Mesh]);
                    }

                    var multiMeshInstance = debugStandardMeshes[debugVisualGroup.Keys.Mesh];
                    var count = debugVisualGroup.DebugVisuals.Count;
                    if (multiMeshInstance.Multimesh.InstanceCount != count)
                    {
                        multiMeshInstance.Multimesh.InstanceCount = count;
                    }

                    for (var i = 0; i < count; i++) {
                        multiMeshInstance.Multimesh.SetInstanceTransform(i, debugVisualGroup.DebugVisuals[i].Transform);
                        multiMeshInstance.Multimesh.SetInstanceColor(i, debugVisualGroup.DebugVisuals[i].colour);
                    }
                }
                else //custom meshes (which use fast meshes via points) can all be combined into one array mesh
                {
                    SurfaceTool surfaceTool = new SurfaceTool();
                    surfaceTool.Begin(debugVisualGroup.Keys.PrimitiveType);
                    surfaceTool.SetMaterial(standardMaterial);

                    //add each vertex specified in the debug vis object
                    for (var i = 0; i < debugVisualGroup.DebugVisuals.Count; i++) {
                        for (var j = 0; j < debugVisualGroup.DebugVisuals[i].Vertices.Count; j++) {
                            surfaceTool.SetColor(debugVisualGroup.DebugVisuals[i].colour);
                            surfaceTool.AddVertex(debugVisualGroup.DebugVisuals[i].Vertices[j]);
                        }
                    }

                    ArrayMesh mesh = customMeshBatch.Mesh as ArrayMesh;
                    surfaceTool.Commit(mesh);
                }
            }
        }

        public override void _Process(double delta)
        {
            Refresh();
        }
    }
}
