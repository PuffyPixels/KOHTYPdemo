using Assets.NavRoute.Core;
using System.Collections.Generic;

namespace Assets.NavRoute.Algorithms
{
    public class BIBFS : PathfindingAlgorithm
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

            // Forward
            Queue<Poi> openSetForward = new();
            Dictionary<Poi, Route> cameFromForward = new()
            { [startVertex] = null };

            // Backward
            Queue<Poi> openSetBackward = new();
            Dictionary<Poi, Route> cameFromBackward = new()
            { [endVertex] = null };

            openSetForward.Enqueue(startVertex);
            openSetBackward.Enqueue(endVertex);

            Poi meetingPoint = null;

            while (openSetForward.Count > 0 && openSetBackward.Count > 0)
            {
                // Forward step
                if (ExpandBidirectional(adjList, openSetForward, cameFromForward,
                                        cameFromBackward, ref meetingPoint))
                    break;

                // Backward step
                if (ExpandBidirectional(adjList, openSetBackward, cameFromBackward,
                                        cameFromForward, ref meetingPoint))
                    break;
            }

            if (meetingPoint == null)
                return new List<Route>();

            return ReconstructBidirectionalPath(cameFromForward, cameFromBackward,
                                                 meetingPoint, startVertex, endVertex);
        }

        private bool ExpandBidirectional(
            Dictionary<Poi, List<Route>> adjList,
            Queue<Poi> openSet,
            Dictionary<Poi, Route> cameFromCurrent,
            Dictionary<Poi, Route> cameFromOther,
            ref Poi meetingPoint)
        {
            if (openSet.Count == 0)
                return false;

            Poi current = openSet.Dequeue();

            if (!adjList.ContainsKey(current))
                return false;

            foreach (Route way in adjList[current])
            {
                Poi next = way.To;

                if (cameFromCurrent.ContainsKey(next))
                    continue;

                cameFromCurrent[next] = way;
                openSet.Enqueue(next);

                if (cameFromOther.ContainsKey(next))
                {
                    meetingPoint = next;
                    return true;
                }
            }

            return false;
        }
    }
}