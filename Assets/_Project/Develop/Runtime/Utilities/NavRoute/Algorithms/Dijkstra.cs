using Assets.NavRoute.Core;
using Assets.NavRoute.Utils;
using System.Collections.Generic;

namespace Assets.NavRoute.Algorithms
{
    public class Dijkstra : PathfindingAlgorithm
    {
        public override List<Route> FindShortestWay(
            Dictionary<Poi, List<Route>> adjList,
            Poi startVertex,
            Poi endVertex)
        {
            if (!adjList.ContainsKey(startVertex) ||
                !adjList.ContainsKey(endVertex) ||
                startVertex == endVertex)
            {
                return new();
            }

            int estimatedSize = adjList.Count;
            PriorityQueue<Poi> openSet = new(estimatedSize);
            Dictionary<Poi, float> gScore = new(estimatedSize);
            Dictionary<Poi, Route> cameFromEdge = new(estimatedSize);
            Poi current;
            Poi next;

            foreach (Poi vertex in adjList.Keys)
                gScore[vertex] = float.MaxValue;

            gScore[startVertex] = 0f;
            openSet.Enqueue(startVertex, 0f);

            while (openSet.Count > 0)
            {
                openSet.TryDequeue(out current, out float currentG);

                if (currentG > gScore[current])
                    continue;

                if (current == endVertex)
                    break;

                if (!adjList.ContainsKey(current))
                    continue;

                foreach (Route way in adjList[current])
                {
                    next = way.To;
                    float newG = currentG + way.Distance;

                    if (newG < gScore[next])
                    {
                        gScore[next] = newG;
                        cameFromEdge[next] = way;
                        openSet.Enqueue(next, newG);
                    }
                }
            }

            if (!cameFromEdge.ContainsKey(endVertex))
                return new List<Route>();

            return ReconstructPath(cameFromEdge, endVertex);
        }
    }
}