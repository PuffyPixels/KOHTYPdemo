using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms
{
    public class BFS : PathfindingAlgorithm
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
            Queue<Poi> openSet = new(estimatedSize);
            Dictionary<Poi, Route> cameFromEdge = new(estimatedSize)
                {[startVertex] = null};
            Poi current;
            Poi next;

            openSet.Enqueue(startVertex);

            while (openSet.Count > 0)
            {
                current = openSet.Dequeue();

                if (current == endVertex)
                    break;

                if (!adjList.ContainsKey(current))
                    continue;

                foreach (Route way in adjList[current])
                {
                    next = way.To;

                    if (cameFromEdge.ContainsKey(next))
                        continue;

                    cameFromEdge[next] = way;
                    openSet.Enqueue(next);
                }
            }

            if (!cameFromEdge.ContainsKey(endVertex))
                return new List<Route>();

            return ReconstructPath(cameFromEdge, endVertex);
        }
    }
}