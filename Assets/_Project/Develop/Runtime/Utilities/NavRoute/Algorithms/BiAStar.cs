using Assets.NavRoute.Core;
using Assets.NavRoute.Utils;
using System.Collections.Generic;

namespace Assets.NavRoute.Algorithms
{
    public class BIAStar : PathfindingAlgorithm
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

            // Forward search
            Dictionary<Poi, float> gScoreForward = new();
            Dictionary<Poi, Route> cameFromForward = new();
            PriorityQueue<Poi> openSetForward = new();

            // Backward search
            Dictionary<Poi, float> gScoreBackward = new();
            Dictionary<Poi, Route> cameFromBackward = new();
            PriorityQueue<Poi> openSetBackward = new();

            foreach (Poi vertex in adjList.Keys)
            {
                gScoreForward[vertex] = float.MaxValue;
                gScoreBackward[vertex] = float.MaxValue;
            }

            gScoreForward[startVertex] = 0f;
            gScoreBackward[endVertex] = 0f;
            openSetForward.Enqueue(startVertex, Heuristic(startVertex, endVertex));
            openSetBackward.Enqueue(endVertex, Heuristic(endVertex, startVertex));

            Poi meetingPoint = null;
            float bestPathCost = float.MaxValue;

            while (openSetForward.Count > 0 && openSetBackward.Count > 0)
            {
                // Forward step
                if (ExpandBidirectional(adjList, openSetForward, gScoreForward, gScoreBackward,
                                        cameFromForward, endVertex,
                                        ref meetingPoint, ref bestPathCost))
                    break;

                // Backward step
                if (ExpandBidirectional(adjList, openSetBackward, gScoreBackward, gScoreForward,
                                        cameFromBackward, startVertex,
                                        ref meetingPoint, ref bestPathCost))
                    break;
            }

            return ReconstructBidirectionalPath(cameFromForward, cameFromBackward,
                                                 meetingPoint, startVertex, endVertex);
        }

        private bool ExpandBidirectional(
            Dictionary<Poi, List<Route>> adjList,
            PriorityQueue<Poi> openSet,
            Dictionary<Poi, float> gScoreCurrent,
            Dictionary<Poi, float> gScoreOther,
            Dictionary<Poi, Route> cameFrom,
            Poi targetVertex,
            ref Poi meetingPoint,
            ref float bestPathCost)
        {
            if (openSet.Count == 0)
                return false;

            Poi current = openSet.Dequeue();
            float currentG = gScoreCurrent[current];

            if (gScoreOther[current] < float.MaxValue)
            {
                float totalCost = currentG + gScoreOther[current];

                if (totalCost < bestPathCost)
                {
                    bestPathCost = totalCost;
                    meetingPoint = current;
                    return true;
                }
            }

            if (!adjList.ContainsKey(current))
                return false;

            foreach (Route way in adjList[current])
            {
                Poi next = way.To;
                float newG = currentG + way.Distance;

                if (newG < gScoreCurrent[next])
                {
                    gScoreCurrent[next] = newG;
                    cameFrom[next] = way;
                    openSet.Enqueue(next, newG + Heuristic(next, targetVertex));
                }

                if (gScoreOther[next] < float.MaxValue)
                {
                    float totalCost = newG + gScoreOther[next];

                    if (totalCost < bestPathCost)
                    {
                        bestPathCost = totalCost;
                        meetingPoint = next;
                    }
                }
            }

            return false;
        }
    }
}