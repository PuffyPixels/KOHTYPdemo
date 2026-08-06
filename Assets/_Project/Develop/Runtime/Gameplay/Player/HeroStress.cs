using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using DG.Tweening;
using DyrdaDev.FirstPersonController;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class HeroStress : MonoBehaviour
    {
        private const float PANIC_DETECT_RADIUS = 2.0f;
        private const float PANIC_DETECT_MULTIPLIER = 0.4f;

        private const string RUN_SOURCE_NAME = "Running";
        private const string CROUCH_SOURCE_NAME = "Crouching";

        [SerializeField] private float _runStressValue = 1.0f;
        [SerializeField] private float _crouchStressValue = 1.0f;

        [SerializeField] private Image _stressVignette;
        [SerializeField] private Image _detectVignette;
        [SerializeField] private EnvironmentSound _firstHeartBeat;
        [SerializeField] private EnvironmentSound _secondHeartBeat;
        [SerializeField] private EnvironmentSound _breath;
        [SerializeField] private InputActionBasedFirstPersonControllerInput _input;
        [SerializeField] private LayerMask _enemyMask;

        [field: SerializeField] public SphereCollider Aura {  get; private set; }
        
        private Stress _stress;
        private Pulse _pulse;
        private StressState _currentStressState;
        private CompositeDisposable _disposables = new();

        private Tween _vignetteTween;
        private float _vignetteBeatAlpha = 0.2f;
        private float _vignetteAlpha;
        private Color _detectDefault;
        private Color _detectActual;
        private float _detectCheckTime = 0.2f;
        private float _detectTimer;

        public bool InPanic { get; private set; } = false;

        private void Awake()
        {
            Assert.IsNotNull(_stressVignette);
            Assert.IsNotNull(_firstHeartBeat);
            //Assert.IsNotNull(_secondHeartBeat);
            Assert.IsNotNull(_breath);
            Assert.IsNotNull(Aura);
            Assert.IsNotNull(_input);
        }

        public void Init(Stress stress, Pulse pulse)
        {
            _stress = stress;
            _stress.StressChanged += OnStressChanged;
            _stress.StressStateChanged += OnStressStateChanged;

            _pulse = pulse;
            _pulse.FirstBpm += OnFirstBpm;
            //_pulse.SecondBpm += OnSecondBpm;

            _detectDefault = _detectVignette.color;
        }

        private void Start()
        {
            SubscribeToInput();
        }

        private void Update()
        {
            _pulse.Tick(Time.deltaTime);
            CheckDetect();
            GetAngryEnemyByStressAura();
        }

        private void OnFirstBpm()
        {
            VignetteBeat();
            HeartBeat(_firstHeartBeat);
        }

        private void OnSecondBpm()
        {
            VignetteBeat();
            HeartBeat(_secondHeartBeat);
        }

        private void OnStressChanged(float stress)
        {
            DetectAuraRadius(stress);
        }

        public void AddStressSource(string stressName, float value)
        {
            _stress.AddStressSource(stressName, value);
        }

        public void RemoveStressSource(string stressName)
        {
            _stress.RemoveStressSource(stressName);
        }

        public void SetDetect(float detectLevel)
        {
            _detectActual = new(_detectVignette.color.r, _detectVignette.color.g, _detectVignette.color.b, detectLevel);
            _detectVignette.color = _detectActual;
            _detectTimer = _detectCheckTime;
        }

        private void CheckDetect()
        {
            if (_detectVignette.color == _detectDefault)
                return;

            _detectTimer -= Time.deltaTime;

            if (_detectTimer < 0)
            {
                _detectVignette.color = _detectDefault;
            }
        }

        public void Stun() => _stress.Stun();

        private void GetAngryEnemyByStressAura()
        {
            if (InPanic)
            {
                Collider[] targetsInStressRadius = Physics.OverlapSphere(transform.position, Aura.radius, _enemyMask);

                foreach (Collider targetCollider in targetsInStressRadius)
                {
                    if (!targetCollider.TryGetComponent(out ConsultantFacade consultant))
                        continue;

                    consultant.DetectPlayer(this);
                }
            }
        }

        private void OnStressStateChanged(StressState stressState)
        {
            _currentStressState = stressState;

            InPanic = _currentStressState == StressState.Panic;

            float volume = GetVolumeByStress();
            _breath.FadeVolume(volume);
        }

        private void DetectAuraRadius(float stress)
        {
            if (InPanic)
            {
                Aura.enabled = true;
                Aura.radius = PANIC_DETECT_RADIUS + (stress * PANIC_DETECT_MULTIPLIER);
            }
            else
            {
                Aura.enabled = false;
            }
        }

        private void VignetteBeat()
        {
            _vignetteAlpha = _stress.NormalizedStress;
            _stressVignette.color = new(_stressVignette.color.r, _stressVignette.color.g, _stressVignette.color.b, _vignetteAlpha);

            _vignetteTween?.Kill();
            _vignetteTween = null;

            float targetAlpha = Mathf.Clamp01(_vignetteAlpha + _vignetteBeatAlpha);
            float duration = 1 - _vignetteAlpha;

            _vignetteTween = _stressVignette
                .DOFade(targetAlpha, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    _stressVignette
                        .DOFade(_vignetteAlpha, duration)
                        .SetEase(Ease.InOutQuad);
                });
        }

        private void HeartBeat(EnvironmentSound heartBeat)
        {
            float volume = GetVolumeByStress();
            heartBeat.PlaySound(volume);
        }

        private float GetVolumeByStress()
        {
            return _currentStressState switch
            {
                StressState.Troubled => 0.2f,
                StressState.Scared => 0.5f,
                StressState.Panic => 1f,
                _ => 0.0f
            };
        }

        private void OnDestroy()
        {
            _disposables.Dispose();

            _stress.StressChanged -= OnStressChanged;
            _stress.StressStateChanged -= OnStressStateChanged;
            _stress.Dispose();
            _stress = null;

            _pulse.FirstBpm -= OnFirstBpm;
            //_pulse.SecondBpm -= OnSecondBpm;
            _pulse.Dispose();
            _pulse = null;
        }

        private void SubscribeToInput()
        {
            if (_input == null || _stress == null)
                return;

            _input.Move
                .CombineLatest(_input.Run, _input.CrouchState,
                    (move, isRunning, isCrouching) => new { Move = move, IsRunning = isRunning, IsCrouching = isCrouching })
                .Subscribe(data =>
                {
                    bool isMoving = data.Move.sqrMagnitude > 0.01f;

                    bool isRunning = isMoving && data.IsRunning && !data.IsCrouching;

                    if (isRunning)
                        AddStressSource(RUN_SOURCE_NAME, _runStressValue);
                    else
                        RemoveStressSource(RUN_SOURCE_NAME);
                })
                .AddTo(_disposables);

            _input.Move
                .CombineLatest(_input.CrouchState, (move, isCrouching) => new { Move = move, IsCrouching = isCrouching })
                .Subscribe(data =>
                {
                    bool isMoving = data.Move.sqrMagnitude > 0.01f;
                    bool isCrouching = isMoving && data.IsCrouching;

                    if (isCrouching)
                        AddStressSource(CROUCH_SOURCE_NAME, _crouchStressValue);
                    else
                        RemoveStressSource(CROUCH_SOURCE_NAME);
                })
                .AddTo(_disposables);
        }
    }
}
