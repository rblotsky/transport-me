using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.RouteGeneration
{
    public struct AStarCosts
    {
        public NavConnection connection;
        public float cost;
    }
    public partial class RouteResult : RefCounted
    {
        private readonly Dictionary<NavConnection, float> cost;
        private readonly Dictionary<NavConnection, NavSegment> cameFrom;
        public NavConnection _start;
        public NavConnection _end;

        public RouteResult(Dictionary<NavConnection, float> cost, Dictionary<NavConnection, NavSegment> cameFrom, NavConnection start, NavConnection end) : base()
        {
            this.cost = cost;
            this.cameFrom = cameFrom;
            _start = start;
            _end = end;
        }

        public List<NavSegment> GetPath()
        {
            List<NavSegment> segments = new List<NavSegment>();
            NavConnection current = _end;
            while (cameFrom[current] != null)
            {
                NavSegment path = cameFrom[current];
                segments.Add(path);
                current = path.StartConnection;
            }
            segments.Reverse();
            return segments;
        }
        public List<AStarCosts> GetComputedPoints()
        {
            List<AStarCosts> costs = new();
            foreach ((NavConnection point, float cost) in cost)
            {
                costs.Add(new()
                {
                    connection = point,
                    cost = cost
                });
            }
            return costs;

        }
    }
}
