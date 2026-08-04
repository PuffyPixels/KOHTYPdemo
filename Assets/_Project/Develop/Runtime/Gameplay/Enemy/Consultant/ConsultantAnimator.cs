using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class ConsultantAnimator : MonoBehaviour
    {
        private readonly int WalkBool = Animator.StringToHash("IsWalk");
        private readonly int RunBool = Animator.StringToHash("IsRun");

        private readonly int BlindedTrigger = Animator.StringToHash("Blinded");
        private readonly int PunchTrigger = Animator.StringToHash("Punch");

        [SerializeField] private Animator _animator;

        private void Awake()
        {
            Assert.IsNotNull(_animator);
        }

        public void Walk() => _animator.SetBool(WalkBool, true);
        public void StopWalk() => _animator.SetBool(WalkBool, false);
        public void Run() => _animator.SetBool(RunBool, true);
        public void StopRun() => _animator.SetBool(RunBool, false);

        public void Blinded() => _animator.SetTrigger(BlindedTrigger);
        public void Punch() => _animator.SetTrigger(PunchTrigger);
    }
}
