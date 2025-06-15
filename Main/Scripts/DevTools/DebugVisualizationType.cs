using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.DevTools
{
    public enum DebugVisualizationType
    {
        Line = 0,
        Zone = 1
    }
    [Flags]
    public enum DebugVisualizationFilters
    {
        None = 0,
        NavSegments = 1,
        VehicleCollisions = 2,
    }
}
