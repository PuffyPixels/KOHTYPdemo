using Assets.NavRoute.Core;
using System.Collections.Generic;

namespace Assets.NavRoute.Algorithms
{
    public class DFS : PathfindingAlgorithm
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
            Stack<Poi> openSet = new(estimatedSize);
            Dictionary<Poi, Route> cameFromEdge = new(estimatedSize)
                {[startVertex] = null};
            Poi current;
            Poi next;

            openSet.Push(startVertex);

            while (openSet.Count > 0)
            {
                current = openSet.Pop();

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
                    openSet.Push(next);
                }
            }

            if (!cameFromEdge.ContainsKey(endVertex))
                return new List<Route>();

            return ReconstructPath(cameFromEdge, endVertex);
        }
    }
}