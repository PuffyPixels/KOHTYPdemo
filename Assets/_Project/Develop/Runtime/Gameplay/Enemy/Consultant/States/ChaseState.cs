using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class ChaseState : ConsultantState
    {
        private const float DEFAULT_SOUND_INTERVAL_MIN = 10f;
        private const float DEFAULT_SOUND_INTERVAL_MAX = 20f;

        private float _minCaptureDistanse = 0.1f;

        public ChaseState(
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

            _consultant.SetRun();
        }

        protected override void UpdateLogic(float deltaTime)
        {
            _consultant.GoTo(_consultant.LastKnownPlayerPosition);
        }

        public override void Exit()
        {
            base.Enter();

            _consultant.StopRun();
        }
    }
}
