using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class ConsultantAnimator : MonoBehaviour
    {
        private const string KICK_TRIGGER = "Kick";

        [SerializeField] private Animator _animator;

        public void Kick()
        {
            _animator.SetTrigger(KICK_TRIGGER);
        }
    }
}
