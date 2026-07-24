using Assets._Project.Develop.Runtime.Utilities.Sound;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public abstract class ConsultantState : State, IUpdatableState
    {
        protected readonly ConsultantFacade _consultant;
        protected readonly SoundsManager _soundsManager;
        protected List<AudioClip> _audioClipList;
        protected bool _isRandomSoundsActive = false;
        private float _soundTimer;
        private float _soundsIntervalMin;
        private float _soundsIntervalMax;

        protected ConsultantState(
            ConsultantFacade consultant,
            List<AudioClip> audioClipList = null,
            SoundsManager soundsManager = null,
            float soundsIntervalMin = 5f,
            float soundsIntervalMax = 10f)
        {
            _consultant = consultant;

            if (_audioClipList != null)
                _audioClipList = audioClipList;

            if (_soundsManager != null)
                _soundsManager = soundsManager;

            if (soundsIntervalMin < 0f)
                Debug.LogError($"soundsIntervalMin can't be {soundsIntervalMin}");

            if (soundsIntervalMax < 0f)
                Debug.LogError($"soundsIntervalMax can't be {soundsIntervalMax}");

            if (soundsIntervalMin > soundsIntervalMax)
                Debug.LogError($"soundsIntervalMin can't be bigest than soundsIntervalMax");

            _soundsIntervalMin = soundsIntervalMin;
            _soundsIntervalMax = soundsIntervalMax;
        }

        public override void Enter()
        {
            base.Enter();

            _soundTimer = Random.Range(_soundsIntervalMin, _soundsIntervalMax);
        }

        public void Update(float deltaTime)
        {
            UpdateLogic(deltaTime);

            if (!_isRandomSoundsActive)
                return;

            _soundTimer -= deltaTime;

            if (_soundTimer <= 0f)
            {
                AudioClip clip = _audioClipList[Random.Range(0, _audioClipList.Count - 1)];
                PlaySound(clip);

                _soundTimer = Random.Range(_soundsIntervalMin, _soundsIntervalMax);
            }
        }

        protected virtual void UpdateLogic(float deltaTime) { }

        protected void PlaySound(AudioClip clip, float lowestPitch = 0.8f, float highestPitch = 1.2f)
        {
            if (_soundsManager == null || clip == null)
                return;

            _soundsManager.PlaySound(
                clip,
                spawnPosition: _consultant.transform,
                lowestPitch: lowestPitch,
                highestPitch: highestPitch
            );
        }

        protected void UpdateDetectionLevel(float progressStep, float deltaTime)
        {
            if (_consultant.Target != null)
                _consultant.DetectionProgress += progressStep * deltaTime;
            else
                _consultant.DetectionProgress -= progressStep * deltaTime;
        }
    }
}
