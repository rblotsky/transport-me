using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.RouteGeneration
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
                throw new ArgumentNullException("connections are null");
            }
            Dictionary<NavConnection, float> cost = new();
            PriorityQueue<NavConnection, float> queue = new();
            Dictionary<NavConnection, NavSegment> cameFrom = new();

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
            return new RouteResult(cost, cameFrom, start, end);
        }


        private void Pathfind(NavConnection end)
        {
            
        }
    }
}
