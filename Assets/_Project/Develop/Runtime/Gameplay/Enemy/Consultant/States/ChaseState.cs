using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class ChaseState : ConsultantState
    {
        private const float DEFAULT_SOUND_INTERVAL_MIN = 10f;
        private const float DEFAULT_SOUND_INTERVAL_MAX = 20f;

        public bool IsPlayerCaptured { get; private set; }

        private readonly float _detectionProgressStep = 0.2f;

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

            _isRandomSoundsActive = true;
        }

        public override void Enter()
        {
            base.Enter();

            IsPlayerCaptured = false;

            _consultant.Run();
        }

        protected override void UpdateLogic(float deltaTime)
        {
            UpdateDetectionLevel(_detectionProgressStep, deltaTime);

            _consultant.GoTo(_consultant.LastKnownPlayerPosition, () => IsPlayerCaptured = true);
        }

        public override void Exit()
        {
            base.Exit();

            _consultant.StopRun();
        }
    }
}
