using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Transportme.Main.DevTools;

namespace Transportme.Main.Scripts.DevTools
{
    public partial class DebugVisualizationUI : VBoxContainer
    {
        private DebugVisualizer _debugVisualizer;

        public override void _EnterTree()
        {
            _debugVisualizer = Simplifications.GetFirstChildOfType<DebugVisualizer>(GetNode("/root/"), true);
            if (_debugVisualizer == null) {
                GD.PrintErr("Could not find node of type: DebugVisualizer attached to the scene.");
                ShowError();
            }
            base._EnterTree();
        }

        public void ShowError()
        {
            GetNode<Panel>("DebugPanel").Visible = false;
            var errorLabel = new Label();
            errorLabel.Text = "Error: No debug visualizer attached to the scene!";
            AddChild(errorLabel);
        }


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
