using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class BlindablePart : MonoBehaviour
    {
        public event Action<float> Blind;

        private const int BLIND_RESISTANCE_MIN = 1;
        private const int BLIND_RESISTANCE_MAX = 3;

        [SerializeField] private float _blindChargeSpeed = 1f;
        [SerializeField] private float _blindDecaySpeed = 1f;

        [SerializeField] private float _firstBlindDuration = 5f;
        [SerializeField] private float _secondBlindDuration = 3f;
        [SerializeField] private float _thirdBlindDuration = 1f;

        [SerializeField] private float _resistanceRestoreInterval = 90f;

        private int _blindLevel = BLIND_RESISTANCE_MIN;
        private float _blindChargeProgress;

        private bool _isBlindActive = false;
        private float _resistanceRestoreTimer;

        private Coroutine _blindCoroutine;

        private void Awake()
        {
            _resistanceRestoreTimer = _resistanceRestoreInterval;
        }

        private void Update()
        {
            RestoreVision();
            RestoreBlindLevel();
        }

        private void RestoreVision()
        {
            if (_blindChargeProgress > 0f)
            {
                _blindChargeProgress = Mathf.Clamp(_blindChargeProgress - (_blindDecaySpeed * Time.deltaTime), 0f, 1f);
            }
        }

        private void RestoreBlindLevel()
        {
            if (_blindLevel <= BLIND_RESISTANCE_MIN)
                return;

            _resistanceRestoreTimer -= Time.deltaTime;

            if (_resistanceRestoreTimer < 0f)
            {
                _blindLevel--;
                _resistanceRestoreTimer = _resistanceRestoreInterval;
            }
        }

        public void ApplyBlind()
        {
            if (_isBlindActive)
                return;

            _resistanceRestoreTimer = _resistanceRestoreInterval;

            _blindChargeProgress += _blindChargeSpeed * Time.deltaTime;

            if (_blindChargeProgress >= 1f)
            {
                _blindChargeProgress = 0f;
                _isBlindActive = true;

                if (_blindLevel < BLIND_RESISTANCE_MAX)
                    _blindLevel++;

                float blindTime = GetBlindTime(_blindLevel);
                Blind?.Invoke(blindTime);

                if (_blindCoroutine != null)
                {
                    StopCoroutine(_blindCoroutine);
                    _blindCoroutine = null;
                }

                _blindCoroutine = StartCoroutine(WaitForBlindEnd(blindTime));
            }
        }

        public void ResetBlind()
        {
            if (_blindCoroutine != null)
            {
                StopCoroutine(_blindCoroutine);
                _blindCoroutine = null;
            }

            _isBlindActive = false;
            _blindChargeProgress = 0f;
            _blindLevel = BLIND_RESISTANCE_MIN;
            _resistanceRestoreTimer = _resistanceRestoreInterval;
        }

        private float GetBlindTime(int blindLevel)
        {
            return blindLevel switch
            {
                1 => _firstBlindDuration,
                2 => _secondBlindDuration,
                3 => _thirdBlindDuration,
                _ => throw new ArgumentException($"Wrong blindLevel: {blindLevel}")
            };
        }

        private IEnumerator WaitForBlindEnd(float time)
        {
            yield return new WaitForSeconds(time);

            _isBlindActive = false;
        }

        private void OnDestroy()
        {
            if (_blindCoroutine != null)
            {
                StopCoroutine(_blindCoroutine);
                _blindCoroutine = null;
            }
        }
    }
}
