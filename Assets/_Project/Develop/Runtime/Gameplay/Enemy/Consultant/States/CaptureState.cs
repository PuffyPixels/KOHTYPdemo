using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant.States
{
    public class CaptureState : ConsultantState
    {
        public CaptureState(
            ConsultantFacade consultant,
            List<AudioClip> audioClipList,
            SoundsManager soundsManager)
            : base(consultant, audioClipList, soundsManager)
        {
            Assert.IsNotNull(consultant);
            Assert.IsNotNull(audioClipList);
            Assert.IsNotNull(soundsManager);
        }

        public bool IsCaptureComplete { get; private set; } = false;

        public override void Enter()
        {
            base.Enter();

            IsCaptureComplete = false;

            PlayCaptureSound(_audioClipList[0]);

            _consultant.Walker.StopWalk();
            _consultant.Animator.Kick();
            _consultant.Stun();

            _consultant.PlayerCaptured?.Invoke();
            
            IsCaptureComplete = true;
        }

        private void PlayCaptureSound(AudioClip clip)
        {
            if (_soundsManager == null || clip == null)
                return;

            _soundsManager.PlaySound(clip, spawnPosition: _consultant.transform);
        }
    }
}
