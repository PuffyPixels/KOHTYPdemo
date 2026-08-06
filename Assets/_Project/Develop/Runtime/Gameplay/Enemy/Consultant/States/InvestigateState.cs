using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class InvestigateState : ConsultantState
    {
        private const float DEFAULT_SOUND_INTERVAL_MIN = 10f;
        private const float DEFAULT_SOUND_INTERVAL_MAX = 20f;

        private const float DETECTION_LEVEL_MIN = 0.5f;

        public bool IsInvestigationComplete { get; private set; }

        private float _investigationTimer;
        private float _investigationDuration = 5f;
        private Vector3 _investigationPoint = Vector3.zero;
        private bool _hasReachedPoint;

        public InvestigateState(
            ConsultantFacade consultant,
            List<AudioClip> audioClipList,
            SoundsManager soundsManager,
            float soundsIntervalMin = DEFAULT_SOUND_INTERVAL_MIN,
            float soundsIntervalMax = DEFAULT_SOUND_INTERVAL_MAX)
            : base(consultant, audioClipList, soundsManager, soundsIntervalMin, soundsIntervalMax)
        {
            Assert.IsNotNull(consultant);
            Assert.IsNotNull(audioClipList);
            Assert.IsNotNull(soundsManager);
        }

        public override void Enter()
        {
            base.Enter();

            _isRandomSoundsActive = true;
            IsInvestigationComplete = false;

            _consultant.SetWalk();
        }

        protected override void UpdateLogic(float deltaTime)
        {
            if (_consultant.LastKnownPlayerPosition != Vector3.zero 
                && _consultant.LastKnownPlayerPosition != _investigationPoint)
            {
                _hasReachedPoint = false;
                _investigationPoint = _consultant.LastKnownPlayerPosition;
                _consultant.GoTo(_investigationPoint, () => _hasReachedPoint = true);
                _investigationTimer = _investigationDuration;
            }

            if (_hasReachedPoint)
            {
                _investigationTimer -= deltaTime;

                if (_investigationTimer <= 0)
                {
                    if (_consultant.DetectionProgress < DETECTION_LEVEL_MIN)
                    {
                        IsInvestigationComplete = true;
                    }
                }
            }
        }
    }
}
