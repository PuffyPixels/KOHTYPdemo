using Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Core
{
    public class RouteGraph
    {
        private Dictionary<Poi, List<Route>> _adjList = new();
        private PathfindingAlgorithm _algorithm;

        private int _totalPriority;
        private bool _isDebug = false;
        private bool _isInited = false;

        public HashSet<Poi> Vertices {  get; private set; } = new();

        public void Init(PathfindingAlgorithm algorithm, List<Route> edges, bool isDebug)
        {
            _algorithm = algorithm;
            _isDebug = isDebug;

            BuildIndex(edges);
            InitVertices();

            _isInited = true;
        }

        public List<Route> GetShortestRoute(Poi startVertex, Poi endVertex)
        {
            if (!_isInited)
                Debug.LogError("NPCRouteGraph is not inited!");

            List<Route> route = new();

            // For perfomance test
#if UNITY_EDITOR
            if (_isDebug)
            {
                System.Diagnostics.Stopwatch sw = new();
                sw.Start();

                route = _algorithm.FindShortestWay(_adjList, startVertex, endVertex);

                sw.Stop();
                Debug.Log($"Route calculation took: {sw.ElapsedTicks} ticks");
                Debug.Log($"Route has: {route.Count} egdes");
                float routeLenght = 0f;
                string routePoints = "Route has points: ";

                for (int i = 0; i < route.Count; i++)
                {
                    routeLenght += Vector3.Distance(route[i].From.Position, route[i].To.Position);
                    routePoints += route[i].From.name + ", ";

                    if (i == route.Count - 1)
                    {
                        routePoints += route[i].To.name;
                    }
                }

                Debug.Log(routePoints);
                Debug.Log($"Route length: {routeLenght}");
            }
#endif

            if (route.Count == 0)
                route = _algorithm.FindShortestWay(_adjList, startVertex, endVertex);

            return route;
        }

        private void BuildIndex(List<Route> edges)
        {
            _adjList.Clear();
            _totalPriority = 0;

            foreach (Route edge in edges)
            {
                if (edge.From == null || edge.To == null)
                    continue;

                AddWayToIndex(edge);

                Route reverseEdge = edge.CreateReverse();
                AddWayToIndex(reverseEdge);
            }
        }

        private void AddWayToIndex(Route edge)
        {
            Poi fromVertex = edge.From;
            Poi toVertex = edge.To;

            if (!_adjList.ContainsKey(fromVertex))
            {
                _adjList[fromVertex] = new List<Route>();
                _totalPriority += fromVertex.Priority;
            }

            if (!_adjList[fromVertex].Any(existingVertex => existingVertex.To == toVertex))
            {
                _adjList[fromVertex].Add(edge);
                _ = edge.Distance;
            }
        }

        private void InitVertices()
        {
            foreach (Poi vertex in _adjList.Keys)
            {
                int currentVertexWeightWithoutThis = _totalPriority - vertex.Priority;
                vertex.Init(currentVertexWeightWithoutThis);

                Vertices.Add(vertex);
            }
        }
    }
}