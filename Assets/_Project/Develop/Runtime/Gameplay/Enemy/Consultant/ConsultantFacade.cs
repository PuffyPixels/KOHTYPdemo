using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Movement;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class ConsultantFacade : MonoBehaviour
    {
        public Action PlayerCaptured;

        [field: SerializeField] public RouteWalker Walker {  get; private set; }
        [field: SerializeField] public ConsultantAnimator Animator { get; private set; }

        private StateMachineBrain _brain;

        public Hero Target { get; private set; } = null;

        private float _detectionProgress;
        public float DetectionProgress
        {
            get
            {
                return _detectionProgress; 
            }
            set
            {
                _detectionProgress = Mathf.Clamp(_detectionProgress + value, 0f, 1f);
            }
        }
        public Vector3 LastKnownPlayerPosition { get; set; }

        public void Init(StateMachineBrain brain)
        {
            _brain = brain;
            _brain.Enable();
        }

        private void Update()
        {

        }

        public void Stun()
        {

        }

        public void DetectPlayer(Hero player)
            => Target = player;

        public void LostTarget()
            => Target = null;

        private void OnDestroy()
        {
            if (_brain != null)
            {
                _brain.Dispose();
                _brain.Disable();
                _brain = null;
            }
        }
    }
}
