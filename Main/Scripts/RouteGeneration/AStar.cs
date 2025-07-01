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

    public partial class AStar : RefCounted
    {
        private float Heuristic(Vector3 a, Vector3 b)
        {
            return a.DistanceTo(b);
        }

        private readonly Dictionary<NavConnection, float> cost = new();
        private readonly PriorityQueue<NavConnection, float> queue = new();
        private readonly Dictionary<NavConnection, NavSegment> cameFrom = new();
        private NavConnection _start;
        private NavConnection _end;

        public void Compute(NavConnection start, NavConnection end)
        {
            _start = start;
            _end = end;
            queue.Enqueue(start, 0);
            cameFrom[start] = null;
            cost[start] = 0;
            Pathfind(end);
        }

        public IEnumerable<NavSegment> GetPath() {
            List<NavSegment> segments = new List<NavSegment>();
            NavConnection current = _end;
            while (cameFrom[current] != null) {
                NavSegment path = cameFrom[current];
                segments.Insert(0, path);
                current = path.StartConnection;
            }
            segments.Reverse();
            return segments;
        }

        public List<AStarCosts> GetComputedPoints()
        {
            List<AStarCosts> costs = new();
            foreach((NavConnection point, float cost) in cost)
            {
                costs.Add(new()
                {
                    connection = point,
                    cost = cost
                });
            }
            return costs;

        }

        private void Pathfind(NavConnection end)
        {
            while (queue.Count > 0) {
                NavConnection current = queue.Dequeue();
                if (current.Equals(end)) {
                    break;
                }

                foreach (NavSegment path in current.Outbound)
                {
                    NavConnection next = path.EndConnection;
                    float newCost = cost[current] + path.Length; //optimally we take time into account
                    if (!cost.ContainsKey(next) || newCost < cost[next])
                    {
                        cost[next] = newCost;
                        float priority = newCost + Heuristic(end.Position, next.Position);
                        queue.Enqueue(next, priority);
                        cameFrom[next] = path;
                    }
                }
            }
        }
    }
}
