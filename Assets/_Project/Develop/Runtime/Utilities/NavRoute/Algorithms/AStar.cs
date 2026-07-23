using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Utils;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms
{
    public class AStar : PathfindingAlgorithm
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
            Dictionary<Poi, float> gScore = new(estimatedSize);
            Dictionary<Poi, Route> cameFromEdge = new(estimatedSize);
            PriorityQueue<Poi> openSet = new(estimatedSize);
            Poi current;
            Poi next;

            foreach (Poi vertex in adjList.Keys)
                gScore[vertex] = float.MaxValue;

            gScore[startVertex] = 0f;
            openSet.Enqueue(startVertex, Heuristic(startVertex, endVertex));

            while (openSet.Count > 0)
            {
                openSet.TryDequeue(out current, out float currentF);

                if (currentF > gScore[current] + Heuristic(current, endVertex))
                    continue;

                if (current == endVertex)
                    break;

                if (!adjList.ContainsKey(current))
                    continue;

                foreach (Route way in adjList[current])
                {
                    next = way.To;
                    float newG = gScore[current] + way.Distance;

                    if (newG < gScore[next])
                    {
                        gScore[next] = newG;
                        cameFromEdge[next] = way;
                        float newF = newG + Heuristic(next, endVertex);
                        openSet.Enqueue(next, newF);
                    }
                }
            }

            if (!cameFromEdge.ContainsKey(endVertex))
                return new List<Route>();

            return ReconstructPath(cameFromEdge, endVertex);
        }
    }
}