using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class WatchGlitchPlayer : MonoBehaviour
    {
        private const string GLYPHS = "0123456789ABCDEFHILOSZ";
        [SerializeField] private HeroWatch _heroWatch;
        [Header("Digits")]
        [SerializeField] private TMP_Text _hourLeft;
        [SerializeField] private TMP_Text _hourRight;
        [SerializeField] private TMP_Text _colon;
        [SerializeField] private TMP_Text _minuteLeft;
        [SerializeField] private TMP_Text _minuteRight;

        [Header("Timings")]
        [SerializeField] private Vector2 _effectDelay = new(0.4f, 1.2f);
        [SerializeField] private Vector2 _effectDuration = new(0.08f, 0.2f);

        [Header("Shake")]
        [SerializeField] private float _shakeDistance = 2f;
        [SerializeField] private int _shakeVibrato = 18;

        private TMP_Text[] _digits;
        private WatchSignal _signal;
        private bool _multipleEnemies;
        private float _nextEffectTime;

        private WatchGlitchType _lastEffect = WatchGlitchType.None;

        private void Awake()
        {
            Assert.IsNotNull(_hourLeft);
            Assert.IsNotNull(_hourRight);
            Assert.IsNotNull(_colon);
            Assert.IsNotNull(_minuteLeft);
            Assert.IsNotNull(_minuteRight);

            _digits = new[]
            {
                _hourLeft,
                _hourRight,
                _colon,
                _minuteLeft,
                _minuteRight
            };
            _heroWatch.TimeChanged += OnTimeChanged;
        }

        private void Update()
        {
            if (_signal.Source == WatchSignalSource.None)
                return;

            if (Time.time < _nextEffectTime)
                return;

            PlayRandomEffect();

            float multiplier = Mathf.Lerp(1f, 0.35f, _signal.Strength);

            _nextEffectTime =
                Time.time +
                Random.Range(_effectDelay.x, _effectDelay.y) * multiplier;
        }

        public void PlayImmediate()
        {
            PlayRandomEffect();
        }

        public void SetSignal(WatchSignal signal, bool multipleEnemies)
        {
            _signal = signal;
            _multipleEnemies = multipleEnemies;
        }

        private void OnTimeChanged(int hour, int minute)
        {
            string h = hour.ToString("00");
            string m = minute.ToString("00");

            _hourLeft.text = h[0].ToString();
            _hourRight.text = h[1].ToString();

            _colon.text = ":";

            _minuteLeft.text = m[0].ToString();
            _minuteRight.text = m[1].ToString();
        }

        private void PlayRandomEffect()
        {
            WatchGlitchType effect = GetRandomEffect();

            switch (effect)
            {
                case WatchGlitchType.Shake:
                    PlayShake();
                    break;

                case WatchGlitchType.Flicker:
                    PlayFlicker();
                    break;

                case WatchGlitchType.MissingDigit:
                    PlayMissingDigit();
                    break;

                case WatchGlitchType.SwapCharacter:
                    PlaySwapCharacter();
                    break;

                case WatchGlitchType.Scramble:
                    PlayScramble();
                    break;
            }

            _lastEffect = effect;
        }

        private WatchGlitchType GetRandomEffect()
        {
            WatchGlitchType effect;

            do
            {
                if (_multipleEnemies &&
    Random.value < 0.45f)
                {
                    effect = WatchGlitchType.Flicker;
                }
                else
                {
                    effect = (WatchGlitchType)Random.Range(1, 6);
                }
            }
            while (effect == _lastEffect);

            return effect;
        }

        private TMP_Text GetRandomDigit(bool includeColon = false)
        {
            int max = includeColon ? _digits.Length : _digits.Length - 1;

            return _digits[Random.Range(0, max)];
        }

        private void PlayShake()
        {
            TMP_Text digit = GetRandomDigit();

            digit.transform
                .DOShakePosition(
                    Random.Range(_effectDuration.x, _effectDuration.y),
                    _shakeDistance,
                    _shakeVibrato);
        }

        private void PlayFlicker()
        {
            TMP_Text digit = GetRandomDigit(true);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(digit.DOFade(0f, 0.03f));
            sequence.Append(digit.DOFade(1f, 0.03f));
            sequence.Append(digit.DOFade(0f, 0.03f));
            sequence.Append(digit.DOFade(1f, 0.03f));
        }

        private void PlayMissingDigit()
        {
            TMP_Text digit = GetRandomDigit();

            string value = digit.text;

            digit.text = " ";

            DOVirtual.DelayedCall(
                Random.Range(_effectDuration.x, _effectDuration.y),
                () =>
                {
                    if (digit != null)
                        digit.text = value;
                });
        }

        private void PlaySwapCharacter()
        {
            TMP_Text digit = GetRandomDigit();

            string value = digit.text;

            digit.text = GetReplacement(value);

            DOVirtual.DelayedCall(
                Random.Range(_effectDuration.x, _effectDuration.y),
                () =>
                {
                    if (digit != null)
                        digit.text = value;
                });
        }

        private void PlayScramble()
        {
            TMP_Text digit = GetRandomDigit();

            string original = digit.text;

            int iterations = Mathf.RoundToInt(Mathf.Lerp(3, 10, _signal.Strength));

            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < iterations; i++)
            {
                sequence.AppendCallback(() =>
                {
                    digit.text = GLYPHS[Random.Range(0, GLYPHS.Length)].ToString();
                });

                sequence.AppendInterval(0.02f);
            }

            sequence.AppendCallback(() =>
            {
                digit.text = original;
            });
        }

        private string GetReplacement(string symbol)
        {
            return symbol switch
            {
                "0" => "C",
                "1" => "I",
                "2" => "Z",
                "3" => "E",
                "4" => "A",
                "5" => "S",
                "6" => "G",
                "7" => "L",
                "8" => "B",
                "9" => "P",
                ":" => ".",
                _ => GLYPHS[Random.Range(0, GLYPHS.Length)].ToString()
            };
        }

        private void OnDestroy()
        {
            _heroWatch.TimeChanged -= OnTimeChanged;
        }

        private enum WatchGlitchType
        {
            None,
            Shake,
            Flicker,
            MissingDigit,
            SwapCharacter,
            Scramble
        }
    }
}