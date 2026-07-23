using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Navigation
{
    [ExecuteAlways]
    public class RouteVisualizator : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float UPDATE_TRANSFORMS_DELAY = 1.0f;

        private List<Route> _activeRoutes;

        [SerializeField] private RouteService _routeService;

        [SerializeField] private ChildCountChangeNotifier _poisParent;
        [SerializeField] private ChildCountChangeNotifier _waypointsParent;

        [SerializeField] private bool _isPoisVisible;
        private bool _cachedIsPoisVisible;

        [SerializeField] private bool _isWaypointsVisible;
        private bool _cachedIsWaypointsVisible;

        [SerializeField] private bool _isWaysVisible;
        private bool _cachedIsWaysVisible;

        private List<Waypoint> _pois = new();
        private List<Waypoint> _waypoints = new();

        private float _updateTimer = 0f;
        private Coroutine _updateCoroutine;

        private void OnEnable()
        {
            _activeRoutes = GetComponent<RouteService>().ActiveRoutes;
            _poisParent.ChildCountChanged += OnPoiChanged;
            _waypointsParent.ChildCountChanged += OnWaypointChanged;

            UpdatePointsList(_poisParent.transform, _pois, _poisParent.transform.childCount);
            UpdatePointsList(_waypointsParent.transform, _waypoints, _waypointsParent.transform.childCount);

            _updateCoroutine = StartCoroutine(UpdateTransforms());
        }

        private void OnDisable()
        {
            _poisParent.ChildCountChanged -= OnPoiChanged;
            _waypointsParent.ChildCountChanged -= OnWaypointChanged;

            if (_updateCoroutine != null)
            {
                StopCoroutine(_updateCoroutine);
                _updateCoroutine = null;
            }
        }

        private void OnPoiChanged(int childCount)
            => UpdatePointsList(_poisParent.transform, _pois, childCount);

        private void OnWaypointChanged(int childCount)
            => UpdatePointsList(_waypointsParent.transform, _waypoints, childCount);

        private void UpdatePointsList(Transform parent, List<Waypoint> cachedPoints, int childCount)
        {
            cachedPoints.Clear();

            for (int i = 0; i < childCount; i++)
            {
                Waypoint child = parent.GetChild(i).GetComponentInChildren<Waypoint>();

                if (child != null)
                    cachedPoints.Add(child);
            }
        }

        private void OnDrawGizmos()
        {
            CheckboxChecker();

            if (_isWaysVisible)
                DrawWays();
        }

        private void CheckboxChecker()
        {
            if (_isPoisVisible != _cachedIsPoisVisible)
            {
                _cachedIsPoisVisible = _isPoisVisible;

                foreach (Waypoint point in _pois)
                    point.IsVisible = _cachedIsPoisVisible;
            }

            if (_isWaypointsVisible != _cachedIsWaypointsVisible)
            {
                _cachedIsWaypointsVisible = _isWaypointsVisible;

                foreach (Waypoint point in _waypoints)
                    point.IsVisible = _cachedIsWaypointsVisible;
            }

            if (_isWaysVisible != _cachedIsWaysVisible)
            {
                _cachedIsWaysVisible = _isWaysVisible;

                foreach (Route way in _activeRoutes)
                    if (way != null)
                        way.IsVisible = _cachedIsWaysVisible;
            }
        }

        private IEnumerator UpdateTransforms()
        {
            while (isActiveAndEnabled)
            {
                if(!_isWaysVisible && !_isPoisVisible && !_isWaypointsVisible)
                {
                    yield return null;
                    continue;
                }

                if (Time.realtimeSinceStartup - _updateTimer > UPDATE_TRANSFORMS_DELAY)
                {
                    _updateTimer = Time.realtimeSinceStartup;

                    foreach (Route way in _activeRoutes)
                    {
                        way.UpdateCaches();
                    }
                }

                yield return null;
            }
        }

        private void DrawWays()
        {
            foreach (Route way in _activeRoutes)
            {
                way.DrawGizmos();
            }
        }
#endif
    }
}