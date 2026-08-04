using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Movement;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant
{
    public class ConsultantFacade : MonoBehaviour
    {
        public event Action PlayerCaptured;

        [SerializeField] private BlindablePart _head;

        private const float AGENT_STOP_SPEED = 0f;
        private float _agentWalkSpeed;

        [SerializeField] private RouteWalker _walker;
        [SerializeField] private ConsultantAnimator _animatorController;

        [Header("Sounds")]
        [field: SerializeField] public List<AudioClip> AnabiosisSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> PatrolSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> AttentionSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> InvestigateSounds { get; private set; }
        [field: SerializeField] public List<AudioClip> ChaseSounds { get; private set; }
        [field: SerializeField] public AudioClip CaptureSound { get; private set; }

        private StateMachineBrain _brain;

        public Hero Target { get; private set; } = null;

        private Coroutine _blindCoroutine;

        private float _detectionProgress;

        private void Awake()
        {
            Assert.IsNotNull(_head);
            Assert.IsNotNull(_walker);
            Assert.IsNotNull(_animatorController);

            _head.Blind += OnBlind;
            _agentWalkSpeed = _walker.Agent.speed;
        }

        private void Update()
        {
            if (_brain != null)
                _brain.Update(Time.deltaTime);
        }

        public float DetectionProgress
        {
            get => _detectionProgress;
            set => _detectionProgress = Mathf.Clamp(_detectionProgress + value, 0f, 1f);
        }

        public Vector3 LastKnownPlayerPosition { get; set; }

        public void Init(StateMachineBrain brain, RouteService routeService)
        {
            _brain = brain;
            _brain.Enable();

            _walker.Init(routeService);
        }

        public void Walk()
        {
            _walker.SetSpeed(_agentWalkSpeed);
            _walker.StartWalk();
            _animatorController.Walk();
        }

        public void StopWalk()
        {
            _walker.SetSpeed(AGENT_STOP_SPEED);
            _walker.StopWalk();
            _animatorController.StopWalk();
        }

        public void Run()
        {
            _walker.SetSpeed(WalkerSpeed.CONSULTANT_RUN);
            _animatorController.Run();
        }

        public void StopRun()
        {
            _walker.SetSpeed(_agentWalkSpeed);
            _animatorController.StopRun();
        }

        public void GoTo(Vector3 target, Action onComplete)
        {
            _walker.GoTo(target, onComplete);
        }

        private void OnBlind(float blindTime)
        {
            _animatorController.Blinded();

            if (_blindCoroutine != null)
            {
                StopCoroutine(_blindCoroutine);
                _blindCoroutine = null;
            }

            _blindCoroutine = StartCoroutine(BlindRoutine(blindTime));
        }

        public void Stun()
        {
            _walker.StopWalk();
            _animatorController.Punch();
            PlayerCaptured?.Invoke();
        }

        private IEnumerator BlindRoutine(float blindTime)
        {
            _walker.Agent.speed = AGENT_STOP_SPEED;

            yield return new WaitForSeconds(blindTime);

            _walker.Agent.speed = _agentWalkSpeed;
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
