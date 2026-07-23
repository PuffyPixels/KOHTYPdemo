using Assets.NavRoute.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NavRoute.Algorithms
{
    public abstract class PathfindingAlgorithm
    {
        public abstract List<Route> FindShortestWay(
            Dictionary<Poi, List<Route>> adjList,
            Poi startVertex,
            Poi endVertex);

        protected float Heuristic(Poi a, Poi b) => 
            Vector3.Distance(a.Position, b.Position);

        protected List<Route> ReconstructPath(
            Dictionary<Poi, Route> cameFromEdge,
            Poi endVertex)
        {
            List<Route> route = new();
            Poi current = endVertex;

            while (cameFromEdge.TryGetValue(current, out Route way) && way != null)
            {
                route.Add(way);
                current = way.From;
            }

            route.Reverse();

            return route;
        }

        protected List<Route> ReconstructBidirectionalPath(
            Dictionary<Poi, Route> cameFromForward,
            Dictionary<Poi, Route> cameFromBackward,
            Poi meetingPoint,
            Poi startVertex,
            Poi endVertex)
        {
            if (meetingPoint == null)
                return new List<Route>();

            List<Route> route = new();
            Poi current = meetingPoint;

            // Forward part
            List<Route> forwardPath = new();
            while (current != startVertex && cameFromForward.TryGetValue(current, out Route way))
            {
                forwardPath.Add(way);
                current = way.From;
            }

            if (current != startVertex)
                return new List<Route>();

            forwardPath.Reverse();
            route.AddRange(forwardPath);

            // Backward part
            current = meetingPoint;
            List<Route> backwardPath = new();
            while (current != endVertex && cameFromBackward.TryGetValue(current, out Route way))
            {
                Route reversedWay = way.CreateReverse();
                backwardPath.Add(reversedWay);
                current = way.From;
            }

            if (current != endVertex)
                return new List<Route>();

            route.AddRange(backwardPath);

            return route;
        }
    }
}