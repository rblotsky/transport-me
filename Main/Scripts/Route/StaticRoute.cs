using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Route
{
    //represents a static class
    public partial class StaticRoute : Node
    {
        [Export] private NavConnection start;
        [Export] private NavConnection end;
        private RouteResult _routeResult;

        public override void _Ready()
        {
           _routeResult = AStar.Compute(start, end);
            base._Ready();
        }

    }
}
