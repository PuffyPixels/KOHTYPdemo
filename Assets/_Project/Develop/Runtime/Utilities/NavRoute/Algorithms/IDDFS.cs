using Assets.NavRoute.Core;
using System.Collections.Generic;

namespace Assets.NavRoute.Algorithms
{
    public class IDDFS : PathfindingAlgorithm
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

            int maxDepth = adjList.Count;

            for (int depthLimit = 0; depthLimit <= maxDepth; depthLimit++)
            {
                Dictionary<Poi, Route> cameFrom = new()
                { [startVertex] = null };

                List<Route> result = DFSLimited(
                    adjList,
                    startVertex,
                    endVertex,
                    depthLimit,
                    cameFrom);

                if (result != null)
                    return result;
            }

            return new List<Route>();
        }

        private List<Route> DFSLimited(
            Dictionary<Poi, List<Route>> adjList,
            Poi current,
            Poi endVertex,
            int depthLimit,
            Dictionary<Poi, Route> cameFrom)
        {
            if (current == endVertex)
                return ReconstructPath(cameFrom, endVertex);

            if (depthLimit == 0)
                return null;

            if (!adjList.ContainsKey(current))
                return null;

            foreach (Route way in adjList[current])
            {
                Poi next = way.To;

                if (cameFrom.ContainsKey(next))
                    continue;

                cameFrom[next] = way;

                List<Route> result = DFSLimited(
                    adjList,
                    next,
                    endVertex,
                    depthLimit - 1,
                    cameFrom);

                if (result != null)
                    return result;

                cameFrom.Remove(next);
            }

            return null;
        }
    }
}