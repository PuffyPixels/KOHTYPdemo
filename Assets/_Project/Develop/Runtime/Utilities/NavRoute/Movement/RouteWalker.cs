using Assets.NavRoute.Core;
using Assets.NavRoute.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.NavRoute.Movement
{
    public class RouteWalker : MonoBehaviour
    {
        private const float MAX_DISTANCE_TO_NAVMESH = 3f;
        private const float NAVMESH_OBSTACLE_WAIT_TIME = 0.1f;

        protected readonly int WalkBool = Animator.StringToHash("isWalk");

        [SerializeField] private RouteService _routeService;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private NavMeshObstacle _obstacle;
        [SerializeField] private Animator _animator;

        [SerializeField] private float _minDelayTimeInPoint = 1f;
        [SerializeField] private float _maxDelayTimeInPoint = 5f;

        private Queue<Waypoint> _currentRoute;
        private Poi _lastPoi;
        private Poi _nextPoi;

        private Coroutine _walkCoroutine;
        private Coroutine _lookCoroutine;

        private readonly WaitWhile _cachedWaitWhilePause = new(() => Time.timeScale == 0);
        private WaitUntil _cachedArrivedCondition;

        private bool _isInited;

        private void Start()
        {
            Init();
            StartWalk();
        }

        private void Init()
        {
            if (_isInited)
                return;

            _cachedArrivedCondition = new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
            _isInited = true;
        }

        public void StartWalk()
        {
            Init();

            if (_nextPoi == null || _currentRoute == null)
            {
                GoToNearestPoi();
            }
            else
            {
                GoToNextPoi();
            }
        }

        public void StopWalk()
        {
            _animator.SetBool(WalkBool, false);

            ClearCoroutine(ref _walkCoroutine);
            ClearCoroutine(ref _lookCoroutine);

            if (_agent != null && _agent.isActiveAndEnabled)
                _agent.ResetPath();
        }

        public void GoTo(Vector3 target, Action onComplete)
        {
            ClearCoroutine(ref _walkCoroutine);

            _walkCoroutine = StartCoroutine(GoToRoutine(target, onComplete));
        }

        private IEnumerator GoToRoutine(Vector3 target, Action onComplete)
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, MAX_DISTANCE_TO_NAVMESH, NavMesh.AllAreas))
            {
                target = hit.position;
            }

            _agent.SetDestination(target);
            _animator.SetBool(WalkBool, true);

            yield return _cachedArrivedCondition;

            _animator.SetBool(WalkBool, false);

            onComplete?.Invoke();
        }

        public void LookAt(Vector3 targetPoint, Action onComplete)
        {
            ClearCoroutine(ref _lookCoroutine);

            _lookCoroutine = StartCoroutine(LookAtRoutine(targetPoint, onComplete));
        }

        private IEnumerator LookAtRoutine(Vector3 targetPoint, Action onComplete)
        {
            Vector3 targetDirection = targetPoint - _agent.transform.position;
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
            {
                onComplete?.Invoke();
                yield break;
            }

            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float step = Time.deltaTime * _agent.angularSpeed;

            while (true)
            {
                if (_agent == null || !_agent.isActiveAndEnabled)
                {
                    Debug.LogWarning($"{name}: agent is not found");
                    onComplete?.Invoke();
                    yield break;
                }

                float currentAngle = _agent.transform.eulerAngles.y;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

                if (Mathf.Abs(angleDiff) <= 0.01f)
                    break;

                float newAngle = currentAngle + Mathf.Sign(angleDiff) * Mathf.Min(Mathf.Abs(angleDiff), step);
                _agent.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

                yield return _cachedWaitWhilePause;
            }

            onComplete?.Invoke();
        }

        private void GoToNearestPoi()
        {
            ClearCoroutine(ref _walkCoroutine);

            _nextPoi = _routeService.GetNearstPoi(transform.position);

            if (_nextPoi == null)
                throw new NullReferenceException();

            if ((_nextPoi.Position - transform.position).sqrMagnitude > _agent.stoppingDistance * _agent.stoppingDistance)
            {
                _walkCoroutine = StartCoroutine(GoToNearestPoiRoutine());
            }
            else
            {
                _lastPoi = _nextPoi;
                _walkCoroutine = StartCoroutine(OnPoiReachedRoutine());
            }
        }

        private IEnumerator GoToNearestPoiRoutine()
        {
            _agent.SetDestination(_nextPoi.Position);
            _animator.SetBool(WalkBool, true);

            yield return _cachedArrivedCondition;

            _lastPoi = _nextPoi;

            yield return OnPoiReachedRoutine();
        }

        private async void GoToNextPoi()
        {
            try
            {
                await AgentOn(destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            ClearCoroutine(ref _walkCoroutine);
            _walkCoroutine = StartCoroutine(GoToNextPoiRoutine());
        }

        private IEnumerator GoToNextPoiRoutine()
        {
            Waypoint nextPoint = null;

            while (_currentRoute.Count > 0)
            {
                nextPoint = _currentRoute.Dequeue();

                _agent.SetDestination(nextPoint.Position);
                _animator.SetBool(WalkBool, true);

                yield return _cachedArrivedCondition;
            }

            if (nextPoint is not null and Poi poi)
                _lastPoi = poi;

            yield return OnPoiReachedRoutine();
        }

        private IEnumerator OnPoiReachedRoutine()
        {
            _animator.SetBool(WalkBool, false);

            ObstacleOn();

            if (_lastPoi != null)
            {
                _animator.SetTrigger(_lastPoi.OnPoiAnimation.ToString());

                yield return new WaitForSeconds(Random.Range(_minDelayTimeInPoint, _maxDelayTimeInPoint) * _lastPoi.TimeModificator);

                _currentRoute = _routeService.GetRandomRouteFrom(_lastPoi);
            }

            GoToNextPoi();
        }

        private void ClearCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private async Awaitable AgentOn(CancellationToken ct = default)
        {
            using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, ct))
            {
                _obstacle.enabled = false;

                try
                {
                    await Awaitable.WaitForSecondsAsync(NAVMESH_OBSTACLE_WAIT_TIME, linkedCts.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log(gameObject.name + " was destroyed while AgentOn");
                    throw;
                }

                if (!NavMesh.SamplePosition(transform.position, out _, _agent.radius, NavMesh.AllAreas))
                    Debug.LogWarning($"{name}: too small value NAVMESH_OBSTACLE_WAIT_TIME = {NAVMESH_OBSTACLE_WAIT_TIME}");

                _agent.enabled = true;
            }
        }

        private void ObstacleOn()
        {
            if (_agent.enabled)
            {
                _agent.ResetPath();
                _agent.enabled = false;
            }

            _obstacle.enabled = true;
        }

        private void OnDisable()
        {
            StopWalk();
        }
    }
}