using Assets._Project.Develop.Runtime.Utilities.Timers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.StressSystem
{
    public enum StressState
    {
        Calm = 0,
        Troubled = 30,
        Scared = 60,
        Panic = 80,
    }

    public class Stress : IDisposable
    {
        public event Action<float> StressChanged;
        public event Action<StressState> StressStateChanged;

        private const float STRESS_LEVEL_MIN = 0f;
        private const float STRESS_LEVEL_MAX = 100f;

        private const float STRESS_PANIC_DEFAULT = 1f;
        private const float STRESS_PANIC_MULTIPLIER = 0.006f;
        private const float STRESS_PANIC_MULTIPLIER_LEVEL = 15f;

        private const float STRESS_RECOVERY_BASE = 2.2f;
        private const float STRESS_RECOVERY_MODIFY = 1f;
        private const float STRESS_RECOVERY_MAX = 120f;

        private Dictionary<string, float> _stressSources = new();

        private LoopTimer _stressTimer;
        private float _currentStress = STRESS_LEVEL_MIN;

        public float NormalizedStress => _currentStress / STRESS_LEVEL_MAX;
        public StressState CurrentStressState { get; private set; } = StressState.Calm;

        public Stress(LoopTimer stressTimer)
        {
            _stressTimer = stressTimer;
            _stressTimer.Tick += OnTick;
        }

        private void OnTick()
        {
            RecalculateStress();
        }

        public void AddStressSource(string sourceId, float value)
        {
            _stressSources[sourceId] = value;
        }

        public void RemoveStressSource(string sourceId)
        {
            _stressSources.Remove(sourceId);
        }

        private void RecalculateStress()
        {
            float newStress = 0f;

            if (_stressSources.Count == 0)
            {
                newStress -= GetRecovery();
            }
            else
            {
                foreach (float sourceStress in _stressSources.Values)
                    newStress += sourceStress;

                float newStressMultiplier = GetMultiplier(_currentStress + newStress);
                newStress *= newStressMultiplier;
            }

            float totalStress = _currentStress + newStress;

            SetStress(totalStress);
        }

        private float GetMultiplier(float stress)
        {
            if (stress < STRESS_PANIC_MULTIPLIER_LEVEL)
                return STRESS_PANIC_DEFAULT;
            else
                return STRESS_PANIC_DEFAULT + (STRESS_PANIC_MULTIPLIER * stress);
        }

        private float GetRecovery()
        {
            float recovery = 0f;

            if (_currentStress != 0f)
                recovery = STRESS_RECOVERY_BASE * (STRESS_RECOVERY_MODIFY - _currentStress / STRESS_RECOVERY_MAX);

            return recovery;
        }

        private void SetStress(float stress) 
        {
            float newStress = Mathf.Clamp(stress, STRESS_LEVEL_MIN, STRESS_LEVEL_MAX);

            if (_currentStress == newStress)
                return;

            _currentStress = newStress;

            StressChanged?.Invoke(_currentStress);

            UpdateStressState();
        }

        public void Reset()
        {
            _currentStress = 0f;
            _stressSources.Clear();

            RecalculateStress();
        }

        private void UpdateStressState()
        {
            StressState oldStressState = CurrentStressState;

            StressState newStressState = _currentStress switch
            {
                >= (float)StressState.Panic => StressState.Panic,
                >= (float)StressState.Scared => StressState.Scared,
                >= (float)StressState.Troubled => StressState.Troubled,
                _ => StressState.Calm
            };

            if (oldStressState != newStressState)
            {
                CurrentStressState = newStressState;

                StressStateChanged?.Invoke(CurrentStressState);
            }
        }

        public void Dispose()
        {
            if (_stressTimer != null)
            {
                _stressTimer.Tick -= OnTick;
                _stressTimer.Dispose();
                _stressTimer = null;
            }
        }
    }
}