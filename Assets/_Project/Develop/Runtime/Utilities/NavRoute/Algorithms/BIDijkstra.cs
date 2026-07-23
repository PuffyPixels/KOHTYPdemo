using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Utils;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms
{
    public class BIDijkstra : PathfindingAlgorithm
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

            Dictionary<Poi, float> gScoreForward = new();
            Dictionary<Poi, Route> cameFromForward = new();
            PriorityQueue<Poi> openSetForward = new();

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
            openSetForward.Enqueue(startVertex, 0f);
            openSetBackward.Enqueue(endVertex, 0f);

            Poi meetingPoint = null;
            float bestPathCost = float.MaxValue;

            while (openSetForward.Count > 0 && openSetBackward.Count > 0)
            {
                ExpandBidirectional(
                    adjList,
                    openSetForward,
                    gScoreForward,
                    gScoreBackward,
                    cameFromForward,
                    ref meetingPoint,
                    ref bestPathCost);

                ExpandBidirectional(
                    adjList,
                    openSetBackward,
                    gScoreBackward,
                    gScoreForward,
                    cameFromBackward,
                    ref meetingPoint,
                    ref bestPathCost);

                float minForward = openSetForward.PeekPriority();
                float minBackward = openSetBackward.PeekPriority();

                if (minForward + minBackward >= bestPathCost)
                    break;
            }

            if (meetingPoint == null)
                return new List<Route>();

            return ReconstructBidirectionalPath(
                cameFromForward,
                cameFromBackward,
                meetingPoint,
                startVertex, 
                endVertex);
        }

        private void ExpandBidirectional(
            Dictionary<Poi, List<Route>> adjList,
            PriorityQueue<Poi> openSet,
            Dictionary<Poi, float> gScoreCurrent,
            Dictionary<Poi, float> gScoreOther,
            Dictionary<Poi, Route> cameFrom,
            ref Poi meetingPoint,
            ref float bestPathCost)
        {
            if (openSet.Count == 0)
                return;

            Poi current = openSet.Dequeue();
            float currentG = gScoreCurrent[current];

            if (gScoreOther[current] < float.MaxValue)
            {
                float totalCost = currentG + gScoreOther[current];

                if (totalCost < bestPathCost)
                {
                    bestPathCost = totalCost;
                    meetingPoint = current;
                }
            }

            if (!adjList.ContainsKey(current))
                return;

            foreach (Route way in adjList[current])
            {
                Poi next = way.To;
                float newG = currentG + way.Distance;

                if (newG < gScoreCurrent[next])
                {
                    gScoreCurrent[next] = newG;
                    cameFrom[next] = way;
                    openSet.Enqueue(next, newG);
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
        }
    }
}