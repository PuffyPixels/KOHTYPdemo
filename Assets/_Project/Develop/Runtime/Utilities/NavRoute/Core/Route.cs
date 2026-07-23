using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Core
{
    [Serializable]
    public class Route
    {
        [field: SerializeField] public bool IsVisible {  get; set; }

        public Poi From;
        public Poi To;
        public List<Waypoint> Path = new();

        private Vector3 _cachedFromPosition;
        private Vector3 _cachedToPosition;
        private List<Vector3> _cachedPathPositions = new();
        private float _cachedDistance = -1;

        public float Distance
        {
            get
            {
                if (_cachedDistance < 0)
                    UpdateCaches();

                return _cachedDistance;
            }
        }

        public void UpdateCaches()
        {
            if (From == null && To == null)
            {
                Debug.LogError("Main points of the route are unknown");
                return;
            }

            _cachedFromPosition = From.Position;
            _cachedToPosition = To.Position;

            _cachedPathPositions.Clear();
            _cachedPathPositions.Capacity = Path.Count;

            foreach (Waypoint point in Path)
            {
                if (point != null)
                    _cachedPathPositions.Add(point.Position);
                else
                    _cachedPathPositions.Add(Vector3.zero);
            }

            CacheDistance();
        }

        public IEnumerable<Waypoint> GetAllPoints()
        {
            if (From == null)
                throw new NullReferenceException("Route.From is null");

            if (To == null)
                throw new NullReferenceException("Route.To is null");

            yield return From;

            foreach (Waypoint point in Path)
            {
                if (point != null)
                {
                    yield return point;
                }
            }

            yield return To;
        }

        public Route CreateReverse()
        {
            Route newRoute = new()
            {
                From = this.To,
                To = this.From,
            };

            if (Path.Count > 0)
            {
                List<Waypoint> reversedPath = new(Path);
                reversedPath.Reverse();

                newRoute.Path = reversedPath;
            }

            return newRoute;
        }

        private void CacheDistance()
        {
            _cachedDistance = 0;
            Vector3 prev = _cachedFromPosition;

            foreach (Vector3 point in _cachedPathPositions)
            {
                if (point != Vector3.zero)
                {
                    _cachedDistance += Vector3.Distance(prev, point);
                    prev = point;
                }
            }

            _cachedDistance += Vector3.Distance(prev, _cachedToPosition);
        }

#if UNITY_EDITOR
        private const float LINE_WIDTH = 2f;
        private readonly Color ROUTE_LINE_COLOR = Color.blue;

        public void DrawGizmos()
        {
            if (!IsVisible || From == null || To == null)
                return;

            Vector3 prev = _cachedFromPosition;

            foreach (Vector3 pointPos in _cachedPathPositions)
            {
                if (pointPos == Vector3.zero) 
                    continue;

                Vector3 current = pointPos;
                UnityEditor.Handles.DrawBezier(prev, current, prev, current, ROUTE_LINE_COLOR, null, LINE_WIDTH);
                prev = current;
            }

            Vector3 finalPos = _cachedToPosition;
            UnityEditor.Handles.DrawBezier(prev, finalPos, prev, finalPos, ROUTE_LINE_COLOR, null, LINE_WIDTH);
        }
#endif
    }
}