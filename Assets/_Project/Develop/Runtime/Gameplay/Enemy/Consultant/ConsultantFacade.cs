using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Movement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class ConsultantFacade : MonoBehaviour
    {
        public event Action PlayerCaptured;

        [SerializeField] private BlindablePart _head;

        private const float AGENT_STOP_SPEED = 0f;
        private float _agentDefaultSpeed;

        [field: SerializeField] public RouteWalker Walker {  get; private set; }
        [field: SerializeField] public ConsultantAnimator Animator { get; private set; }

        private StateMachineBrain _brain;

        public Hero Target { get; private set; } = null;

        private Coroutine _blindCoroutine;

        private float _detectionProgress;

        private void Awake()
        {
            Assert.IsNotNull(_head);
            Assert.IsNotNull(Walker);
            Assert.IsNotNull(Animator);
        }

        private void Start()
        {
            _head.Blind += OnBlind;
            _agentDefaultSpeed = Walker.Agent.speed;
        }

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
            PlayerCaptured?.Invoke();
        }

        private void OnBlind(float blindTime)
        {
            if (_blindCoroutine != null)
            {
                StopCoroutine(_blindCoroutine);
                _blindCoroutine = null;
            }

            _blindCoroutine = StartCoroutine(BlindRoutine(blindTime));
        }

        private IEnumerator BlindRoutine(float blindTime)
        {
            Walker.Agent.speed = AGENT_STOP_SPEED;

            yield return new WaitForSeconds(blindTime);

            Walker.Agent.speed = _agentDefaultSpeed;
        }

        public void DetectPlayer(Hero player)
            => Target = player;

        public void LostTarget()
            => Target = null;

        private void OnDestroy()
        {
            _head.Blind -= OnBlind;

            if (_brain != null)
            {
                _brain.Dispose();
                _brain.Disable();
                _brain = null;
            }
        }
    }
}
