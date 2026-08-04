using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Core;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Utilities.NavRoute.Movement
{
    public class WalkerSpeed
    {
        public const float CONSULTANT_RUN = 4f;
        public const float CONSULTANT_BLINDED = 1f;
    }

    public class RouteWalker : MonoBehaviour
    {
        private const float MAX_DISTANCE_TO_NAVMESH = 3f;
        private const float NAVMESH_OBSTACLE_WAIT_TIME = 0.1f;

        [field: SerializeField] public NavMeshAgent Agent {  get; private set; }
        [SerializeField] private NavMeshObstacle _obstacle;
        [SerializeField] private ConsultantAnimator _animator;

        [SerializeField] private float _minDelayTimeInPoint = 1f;
        [SerializeField] private float _maxDelayTimeInPoint = 5f;

        private RouteService _routeService;
        private Queue<Waypoint> _currentRoute;
        private Poi _lastPoi;
        private Poi _nextPoi;

        private Coroutine _walkCoroutine;
        private Coroutine _lookCoroutine;

        private readonly WaitWhile _cachedWaitWhilePause = new(() => Time.timeScale == 0);
        private WaitUntil _cachedArrivedCondition;

        private bool _isInited;

        public float DefaultAgentSpeed { get; private set; }

        public void Init(RouteService routeService)
        {
            if (_isInited)
                return;

            Assert.IsNotNull(routeService);
            _routeService = routeService;

            DefaultAgentSpeed = Agent.speed;

            _cachedArrivedCondition = new WaitUntil(() => !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance);
            _isInited = true;
        }

        public void SetSpeed(float speed)
        {
            float newSpeed = Mathf.Clamp(speed, 0f, 10f);
            Agent.speed = newSpeed;
        }

        public void StartWalk()
        {
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
            _animator.StopWalk();

            ClearCoroutine(ref _walkCoroutine);
            ClearCoroutine(ref _lookCoroutine);

            if (Agent != null && Agent.isActiveAndEnabled)
                Agent.ResetPath();
        }

        public void GoTo(Vector3 target, Action onComplete)
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, MAX_DISTANCE_TO_NAVMESH, NavMesh.AllAreas))
            {
                target = hit.position;

                ClearCoroutine(ref _walkCoroutine);
                _walkCoroutine = StartCoroutine(GoToRoutine(target, onComplete));
            }
        }

        private IEnumerator GoToRoutine(Vector3 target, Action onComplete)
        {
            Agent.SetDestination(target);

            _animator.Walk();

            yield return _cachedArrivedCondition;

            _animator.StopWalk();

            onComplete?.Invoke();
        }

        public void LookAt(Vector3 targetPoint, Action onComplete)
        {
            ClearCoroutine(ref _lookCoroutine);

            _lookCoroutine = StartCoroutine(LookAtRoutine(targetPoint, onComplete));
        }

        private IEnumerator LookAtRoutine(Vector3 targetPoint, Action onComplete)
        {
            Vector3 targetDirection = targetPoint - Agent.transform.position;
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
            {
                onComplete?.Invoke();
                yield break;
            }

            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float step = Time.deltaTime * Agent.angularSpeed;

            while (true)
            {
                if (Agent == null || !Agent.isActiveAndEnabled)
                {
                    Debug.LogWarning($"{name}: agent is not found");
                    onComplete?.Invoke();
                    yield break;
                }

                float currentAngle = Agent.transform.eulerAngles.y;
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);

                if (Mathf.Abs(angleDiff) <= 0.01f)
                    break;

                float newAngle = currentAngle + Mathf.Sign(angleDiff) * Mathf.Min(Mathf.Abs(angleDiff), step);
                Agent.transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

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

            if ((_nextPoi.Position - transform.position).sqrMagnitude > Agent.stoppingDistance * Agent.stoppingDistance)
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
            Agent.SetDestination(_nextPoi.Position);

            _animator.Walk();

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

                Agent.SetDestination(nextPoint.Position);

                _animator.Walk();

                yield return _cachedArrivedCondition;
            }

            if (nextPoint is not null and Poi poi)
                _lastPoi = poi;

            yield return OnPoiReachedRoutine();
        }

        private IEnumerator OnPoiReachedRoutine()
        {
            _animator.StopWalk();

            ObstacleOn();

            if (_lastPoi != null)
            {
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

                if (!NavMesh.SamplePosition(transform.position, out _, Agent.radius, NavMesh.AllAreas))
                    Debug.LogWarning($"{name}: too small value NAVMESH_OBSTACLE_WAIT_TIME = {NAVMESH_OBSTACLE_WAIT_TIME}");

                Agent.enabled = true;
            }
        }

        private void ObstacleOn()
        {
            if (Agent.enabled)
            {
                Agent.ResetPath();
                Agent.enabled = false;
            }

            _obstacle.enabled = true;
        }

        private void OnDisable()
        {
            StopWalk();
        }
    }
}