using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.DevTools;

namespace Transportme.Main.Scripts.DevTools
{
    public partial class DebugVisualizationUI : VBoxContainer
    {
        [Export] private DebugVisualizer _debugVisualizer;


        public void OnVehiclesFilterToggled(bool toggledOn)
        {
            _debugVisualizer.OnFilterToggled(DebugVisualizationFilters.VehicleCollisions, toggledOn);
        }

        public void OnNavigationFilterToggled(bool toggledOn)
        {
            _debugVisualizer.OnFilterToggled(DebugVisualizationFilters.NavSegments, toggledOn);
        }

        public void OnLineFilterEnabled(bool toggledOn)
        {
            _debugVisualizer.OnTypeToggled(DebugVisualizationType.Line, toggledOn);
        }

        public void OnZoneFilterEnabled(bool toggledOn)
        {
            _debugVisualizer.OnTypeToggled(DebugVisualizationType.Zone, toggledOn);
        }
    }
}
