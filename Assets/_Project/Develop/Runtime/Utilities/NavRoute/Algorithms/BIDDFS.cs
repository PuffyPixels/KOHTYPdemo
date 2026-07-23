using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms
{
    public class BIDDFS : PathfindingAlgorithm
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

            for (int depth = 0; depth < int.MaxValue; depth++)
            {
                Dictionary<Poi, Route> cameFromForward = new() 
                { [startVertex] = null };
                Dictionary<Poi, Route> cameFromBackward = new() 
                { [endVertex] = null };
                HashSet<Poi> visitedForward = new() 
                { startVertex };
                HashSet<Poi> visitedBackward = new() 
                { endVertex };

                DFSLimited(adjList, startVertex, depth, cameFromForward, visitedForward);
                DFSLimited(adjList, endVertex, depth, cameFromBackward, visitedBackward);

                Poi meetingPoint = FindIntersection(visitedForward, visitedBackward);

                if (meetingPoint != null)
                {
                    return ReconstructBidirectionalPath(cameFromForward, cameFromBackward,
                                                         meetingPoint, startVertex, endVertex);
                }
            }

            return new List<Route>();
        }

        private void DFSLimited(
            Dictionary<Poi, List<Route>> adjList,
            Poi current,
            int depthLimit,
            Dictionary<Poi, Route> cameFrom,
            HashSet<Poi> visited)
        {
            if (depthLimit == 0 || !adjList.ContainsKey(current))
                return;

            foreach (Route way in adjList[current])
            {
                Poi next = way.To;

                if (visited.Contains(next))
                    continue;

                visited.Add(next);
                cameFrom[next] = way;

                DFSLimited(adjList, next, depthLimit - 1, cameFrom, visited);
            }
        }

        private Poi FindIntersection(
            HashSet<Poi> visitedForward,
            HashSet<Poi> visitedBackward)
        {
            foreach (Poi vertex in visitedForward)
            {
                if (visitedBackward.Contains(vertex))
                    return vertex;
            }

            return null;
        }
    }
}