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
        [SerializeField] private SphereCollider _aura;

        private Stress _stress;
        private Pulse _pulse;

        private Tween _vignetteTween;
        private float _vignetteBeatAlpha = 0.2f;
        private float _vignetteAlpha;

        private bool _inPanic = false;

        private void Awake()
        {
            Assert.IsNotNull(_stressVignette);
            Assert.IsNotNull(_firstHeartBeat);
            Assert.IsNotNull(_secondHeartBeat);
            Assert.IsNotNull(_breath);
            Assert.IsNotNull(_aura);
        }

        public void Init(Stress stress, Pulse pulse)
        {
            _stress = stress;
            _stress.StressChanged += OnStressChanged;
            _stress.StressStateChanged += OnStressStateChanged;

            _pulse = pulse;
            _pulse.FirstBpm += OnFirstBpm;
            _pulse.SecondBpm += OnSecondBpm;
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
            SetBreathVolume(stressState);

            _inPanic = stressState == StressState.Panic;

            _vignetteAlpha = GetAlpha(stressState);
            _stressVignette.color = new(_stressVignette.color.r, _stressVignette.color.g, _stressVignette.color.b, _vignetteAlpha);
        }

        private void DetectAuraRadius(float stress)
        {
            if (_inPanic)
            {
                _aura.enabled = true;
                _aura.radius = PANIC_DETECT_RADIUS + (stress * PANIC_DETECT_MULTIPLIER);
            }
            else
            {
                _aura.enabled = false;
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
            heartBeat.PlaySound();
        }

        private void SetBreathVolume(StressState stressState)
        {
            float volume = stressState switch
            {
                StressState.Troubled => 0.2f,
                StressState.Scared => 0.5f,
                StressState.Panic => 1f,
                _ => 0f
            };

            _breath.FadeVolume(volume);

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
            _pulse.SecondBpm -= OnSecondBpm;
            _pulse.Dispose();
            _pulse = null;
        }
    }
}
