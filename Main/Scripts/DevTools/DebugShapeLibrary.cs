using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DebugShapeType
{
    Sphere,
    Box,
    Capsule
}

namespace Transportme.Main.Scripts.DevTools
{
    /// <summary>
    /// A static libary to re-use meshes for debug visualizations
    /// Easily add on new meshes, and any debug visualization which uses this mesh optimally will collectively use this mesh
    /// </summary>
    public static class DebugShapeLibrary
    {
        private static readonly Dictionary<DebugShapeType, Mesh> _shapes = new();
        private static bool isInitialized = false;

        public static void Init()
        {
            _shapes[DebugShapeType.Sphere] = new SphereMesh { Radius = 0.5f };
            _shapes[DebugShapeType.Box] = new BoxMesh { Size = new Vector3(1, 1, 1) };
            _shapes[DebugShapeType.Capsule] = new CapsuleMesh { Height = 1f, Radius = 1f };
        }

        public static Mesh Get(DebugShapeType type) {
            if (!isInitialized)
            {
                Init();
                isInitialized = true;
            }
            return _shapes[type];
        }
    }
}
