using Assets._Project.Develop.Runtime.Utilities.NavRoute.Algorithms;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Navigation
{
    public class RouteService : MonoBehaviour
    {
        public enum AlgorithmType
        {
            DFS,
            IDDFS,
            BIDDFS,
            BFS,
            BiBFS,
            Dijkstra,
            BiDijkstra,
            AStar,
            BiAStar,
        }

        [SerializeField] private bool _isDebug = false;
        [SerializeField] private Poi _testPoint;

        [SerializeField] private AlgorithmType _algorithmType = AlgorithmType.BFS;

        [field: SerializeField] public List<Route> ActiveRoutes { get; private set; } = new();
        private RouteGraph _graph;

        private void Awake()
        {
            PathfindingAlgorithm currentAlgorithm = _algorithmType switch
            {
                AlgorithmType.DFS => new DFS(),
                AlgorithmType.IDDFS => new IDDFS(),
                AlgorithmType.BIDDFS => new BIDDFS(),
                AlgorithmType.BFS => new BFS(),
                AlgorithmType.BiBFS => new BIBFS(),
                AlgorithmType.Dijkstra => new Dijkstra(),
                AlgorithmType.BiDijkstra => new BIDijkstra(),
                AlgorithmType.AStar => new AStar(),
                AlgorithmType.BiAStar => new BIAStar(),
                _ => throw new ArgumentOutOfRangeException(nameof(_algorithmType), _algorithmType, null)
            };

            _graph = new RouteGraph();
            _graph.Init(currentAlgorithm, ActiveRoutes, _isDebug);
        }

        public Poi GetNearstPoi(Vector3 targetPosition)
        {
            Poi nearestPoint = null;
            float nearestDistance = float.MaxValue;

            foreach (Poi point in _graph.Vertices)
            {
                float pointDistance = Vector3.Distance(targetPosition, point.Position);

                if (pointDistance < nearestDistance)
                {
                    nearestDistance = pointDistance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        public Queue<Waypoint> GetRandomRouteFrom(Poi startPoint)
        {
            HashSet<Poi> availablePoints = new(_graph.Vertices.Where(point => point != startPoint));
            Poi endPoint;

            if (availablePoints.Count == 0)
                Debug.LogError("Empty collection: availablePoints");

            if (availablePoints.Count == 1)
            {
                endPoint = availablePoints.First();
            }
            else
            {
                endPoint = GetRandomPoi(availablePoints, startPoint);
            }

            return GetRoute(startPoint, endPoint);
        }

        private Poi GetRandomPoi(HashSet<Poi> targetPoints, Poi startPoint)
        {
            int totalWeight = startPoint.AllPoisPriorityWithoutThis;

            float random = Random.Range(0f, totalWeight);
            float accumulate = 0f;

            foreach (Poi point in targetPoints)
            {
                accumulate += point.Priority;

                if (random <= accumulate) 
                    return point;
            }

            return null;
        }

        private Queue<Waypoint> GetRoute(Poi startPoint, Poi endPoint)
        {
            List<Route> routes;

            if (_isDebug && _testPoint != null)
            {
                endPoint = _testPoint;
            }
            
            if (ActiveRoutes.Count == 1)
            {
                routes = ActiveRoutes;
            }
            else
            {
                routes = _graph.GetShortestRoute(startPoint, endPoint);
            }

            if (routes.Count == 0)
                throw new ArgumentOutOfRangeException();

            Queue<Waypoint> queue = new();

            foreach (Route route in routes)
            {
                foreach (Waypoint point in route.GetAllPoints())
                {
                    queue.Enqueue(point);
                }
            }

            return queue;
        }
    }
}