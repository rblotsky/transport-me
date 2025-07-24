using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Route
{
    public partial class AStar : RefCounted
    {
        private static float Heuristic(Vector3 a, Vector3 b)
        {
            return a.DistanceTo(b);
        }
        public static RouteResult Compute(NavConnection start, NavConnection end)
        {
            if(start == null || end == null)
            {
                throw new ArgumentNullException("Pathfinding-AStar: Connections are null");
            }
            Dictionary<NavConnection, float> cost = new();
            Dictionary<NavConnection, NavSegment> cameFrom = new();

            PerformPathfindingThingy(start, end, ref cost, ref cameFrom);

            return new RouteResult(cost, cameFrom, start, end);
        }

        public static List<NavSegment> GetPathTo(NavConnection start, NavConnection end)
        {
            Dictionary<NavConnection, float> cost = new();
            Dictionary<NavConnection, NavSegment> cameFrom = new();

            PerformPathfindingThingy(start, end, ref cost, ref cameFrom);
            return Path(end, cost, cameFrom);
            
        }

        private static List<NavSegment> Path(NavConnection end, Dictionary<NavConnection, float> cost, Dictionary<NavConnection, NavSegment> cameFrom)
        {
            List<NavSegment> segments = new List<NavSegment>();
            NavConnection current = end;
            while (cameFrom[current] != null)
            {
                NavSegment path = cameFrom[current];
                segments.Add(path);
                current = path.StartConnection;
            }
            segments.Reverse();
            return segments;
        }

        private static void PerformPathfindingThingy(NavConnection start, NavConnection end, ref Dictionary<NavConnection, float> cost, ref Dictionary<NavConnection, NavSegment> cameFrom)
        {
            PriorityQueue<NavConnection, float> queue = new();

            queue.Enqueue(start, 0);
            cameFrom[start] = null;
            cost[start] = 0;

            while (queue.Count > 0)
            {
                NavConnection current = queue.Dequeue();
                if (current.Equals(end))
                {
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


        private void Pathfind(NavConnection end)
        {
            
        }
    }
}
