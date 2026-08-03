using Assets._Project.Develop.Runtime.Utilities.Sound;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class HeroStress : MonoBehaviour
    {
        private const float PANIC_DETECT_RADIUS = 2.0f;
        private const float PANIC_DETECT_MULTIPLIER = 0.4f;

        [SerializeField] private Image _stressVignette;
        [SerializeField] private EnvironmentSound _firstHeartBeat;
        [SerializeField] private EnvironmentSound _secondHeartBeat;
        [SerializeField] private EnvironmentSound _breath;

        [field: SerializeField] public SphereCollider Aura {  get; private set; }
        
        private Stress _stress;
        private Pulse _pulse;
        private StressState _currentStress;

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

        private void Update()
        {
            _pulse.Tick(Time.deltaTime);
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
            _stress.StressChanged -= OnStressChanged;
            _stress.StressStateChanged -= OnStressStateChanged;
            _stress.Dispose();
            _stress = null;

            _pulse.FirstBpm -= OnFirstBpm;
            //_pulse.SecondBpm -= OnSecondBpm;
            _pulse.Dispose();
            _pulse = null;
        }
    }
}
