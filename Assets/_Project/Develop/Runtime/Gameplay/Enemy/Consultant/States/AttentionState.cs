using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class AttentionState : ConsultantState
    {
        private const float DEFAULT_SOUND_INTERVAL_MIN = 10f;
        private const float DEFAULT_SOUND_INTERVAL_MAX = 20f;

        public AttentionState(
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

            _consultant.StopWalk();
        }
    }
}
