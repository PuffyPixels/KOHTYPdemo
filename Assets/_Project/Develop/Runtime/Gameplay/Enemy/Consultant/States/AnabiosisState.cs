using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class AnabiosisState : ConsultantState
    {
        private const float DEFAULT_SOUND_INTERVAL_MIN = 3f;
        private const float DEFAULT_SOUND_INTERVAL_MAX = 8f;

        public AnabiosisState(
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

            _consultant.Walker.Agent.isStopped = true;
            _consultant.Walker.Agent.ResetPath();
            _consultant.Walker.StopWalk();
        }

        public override void Exit()
        {
            base.Exit();

            _consultant.Walker.Agent.isStopped = false;
        }
    }
}
