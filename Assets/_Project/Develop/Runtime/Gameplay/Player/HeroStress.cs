using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using DG.Tweening;
using DyrdaDev.FirstPersonController;
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

        [SerializeField] private float _runStressValue = 0.3f;
        [SerializeField] private float _crouchStressValue = 0.3f;

        [SerializeField] private Image _stressVignette;
        [SerializeField] private EnvironmentSound _firstHeartBeat;
        [SerializeField] private EnvironmentSound _secondHeartBeat;
        [SerializeField] private EnvironmentSound _breath;
        [SerializeField] private InputActionBasedFirstPersonControllerInput _input;
        [SerializeField] private LayerMask _enemyMask;

        [field: SerializeField] public SphereCollider Aura {  get; private set; }
        
        private Stress _stress;
        private Pulse _pulse;
        private StressState _currentStress;
        private CompositeDisposable _disposables = new();

        private Tween _vignetteTween;
        private float _vignetteBeatAlpha = 0.2f;
        private float _vignetteAlpha;

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
        }

        private void Start()
        {
            SubscribeToInput();
        }

        private void Update()
        {
            _pulse.Tick(Time.deltaTime);
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
            _currentStress = stressState;

            InPanic = _currentStress == StressState.Panic;

            float volume = GetVolumeByStress(_currentStress);
            _breath.FadeVolume(volume);

            _vignetteAlpha = GetAlpha(_currentStress);
            _stressVignette.color = new(_stressVignette.color.r, _stressVignette.color.g, _stressVignette.color.b, _vignetteAlpha);
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
            float volume = GetVolumeByStress(_currentStress);
            heartBeat.PlaySound(volume);
        }

        private float GetVolumeByStress(StressState stressState)
        {
            return stressState switch
            {
                StressState.Troubled => 0.2f,
                StressState.Scared => 0.5f,
                StressState.Panic => 1f,
                _ => 0f
            };
        }

        private float GetAlpha(StressState stressState)
        {
            float volume = stressState switch
            {
                StressState.Troubled => 0.2f,
                StressState.Scared => 0.5f,
                StressState.Panic => 0.7f,
                _ => 0f
            };

            return volume;
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
                        _stress.AddStressSource(StressSourceName.Running, _runStressValue);
                    else
                        _stress.RemoveStressSource(StressSourceName.Running);
                })
                .AddTo(_disposables);

            _input.Move
                .CombineLatest(_input.CrouchState, (move, isCrouching) => new { Move = move, IsCrouching = isCrouching })
                .Subscribe(data =>
                {
                    bool isMoving = data.Move.sqrMagnitude > 0.01f;
                    bool isCrouching = isMoving && data.IsCrouching;

                    if (isCrouching)
                        _stress.AddStressSource(StressSourceName.Crouching, _crouchStressValue);
                    else
                        _stress.RemoveStressSource(StressSourceName.Crouching);
                })
                .AddTo(_disposables);
        }
    }
}
