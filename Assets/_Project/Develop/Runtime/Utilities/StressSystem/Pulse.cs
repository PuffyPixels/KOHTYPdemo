using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Utilities.StressSystem
{
    public class Pulse : IDisposable
    {
        public event Action FirstBpm;
        public event Action SecondBpm;

        private const float MINUTE = 60f;
        private const float SECOND_BPM_TIME_MIN = 0.06f;
        private const float SECOND_BPM_TIME_MAX = 0.16f;

        private Stress _stress;

        private readonly float _minBpm;
        private readonly float _maxBpm;
        private readonly float _bpmRange;

        private float _firstBeatInterval;
        private float _firstBeatTimer;
        private float _secondBeatTimer;

        private bool _isSecondBeat = false;

        public Pulse(Stress stress, float minBpm, float maxBpm)
        {
            _minBpm = minBpm;
            _maxBpm = maxBpm;
            _bpmRange = _maxBpm - _minBpm;

            _stress = stress;
            _stress.StressChanged += OnStressChanged;

            UpdateTimers();
        }

        public void Tick(float deltaTime)
        {
            UpdateHeartbeat(deltaTime);
        }

        private void OnStressChanged(float stress)
        {
            RecalculateStressState();
        }

        private void UpdateHeartbeat(float deltaTime)
        {
            FirstBeat(deltaTime);
            SecondBeat(deltaTime);
        }

        private void FirstBeat(float deltaTime)
        {
            _firstBeatTimer -= deltaTime;

            if (_firstBeatTimer <= 0f)
            {
                FirstBpm?.Invoke();

                UpdateTimers();
                _isSecondBeat = true;
            }
        }

        private void SecondBeat(float deltaTime)
        {
            if (_isSecondBeat)
            {
                _secondBeatTimer -= deltaTime;

                if (_secondBeatTimer <= 0f)
                {
                    SecondBpm?.Invoke();

                    _isSecondBeat = false;
                }
            }
        }

        private void RecalculateStressState()
        {
            float remaining = 0;

            if (_firstBeatInterval != 0)
                remaining = Mathf.Clamp01(_firstBeatTimer / _firstBeatInterval);

            _firstBeatInterval = GetIntervalFromStress();
            _firstBeatTimer = _firstBeatInterval * remaining;
            _secondBeatTimer = _isSecondBeat ? _secondBeatTimer * remaining : 0f;
        }

        private float GetIntervalFromStress()
        {
            float newBpm = _minBpm + (_bpmRange * _stress.NormalizedStress);

            if (newBpm <= 0f)
                return float.PositiveInfinity;

            return MINUTE / newBpm;
        }

        private float GetSecondInterval()
        {
            float secondRatio = Random.Range(SECOND_BPM_TIME_MIN, SECOND_BPM_TIME_MAX);

            return _firstBeatInterval * secondRatio;
        }

        private void UpdateTimers()
        {
            _firstBeatInterval = GetIntervalFromStress();
            _firstBeatTimer = _firstBeatInterval;
            _secondBeatTimer = GetSecondInterval();
        }

        public void Dispose()
        {
            if (_stress != null)
            {
                _stress.StressChanged -= OnStressChanged;
                _stress = null;
            }
        }
    }
}
