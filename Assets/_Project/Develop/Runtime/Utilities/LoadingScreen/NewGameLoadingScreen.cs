using Assets._Project.Develop.Runtime.Utilities.Sound;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Utilities.LoadingScreen
{
    public class NewGameLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        private const float FADE_ON = 1f;
        private const float FADE_OFF = 0f;

        [SerializeField] private AudioClip _startGameSound;
        [SerializeField] private Image _fade;

        private Action _fadedCallback;
        private Tween _currentTween;
        private SoundsManager _soundManager;
        public bool IsShown => gameObject.activeSelf;
        private float _lastAlpha = -1f;
        public bool IsFadeInProcess { get; private set; } = false;

        public void Init(SoundsManager soundManager)
        {
            _soundManager = soundManager;
        }

        private void Awake()
        {
            Hide();
            DontDestroyOnLoad(this);
        }

        public void Hide() => gameObject.SetActive(false);

        public void Show() => gameObject.SetActive(true);

        public void PlayLoadingSound() => _soundManager.PlaySound(_startGameSound, volume: 0.2f, is2D: true);

        public float GetSoundDuration() => _startGameSound.length;

        public void FadeIn(float fadeDuration, Action fadedCallback = null)
        {
            float currentAlpha = _lastAlpha >= 0 ? _lastAlpha : FADE_OFF;
            float remaining = FADE_ON - currentAlpha;
            float normalized = remaining / (FADE_ON - FADE_OFF);
            float adjustedDuration = fadeDuration * normalized;
            _lastAlpha = -1f;
            IsFadeInProcess = true;

            Fade(currentAlpha, FADE_ON, adjustedDuration, fadedCallback);
        }

        public void FadeOut(float fadeDuration, Action fadedCallback = null)
        {
            float currentAlpha = _lastAlpha >= 0 ? _lastAlpha : FADE_ON;
            float remaining = currentAlpha - FADE_OFF;
            float normalized = remaining / (FADE_ON - FADE_OFF);
            float adjustedDuration = fadeDuration * normalized;
            _lastAlpha = -1f;

            Fade(currentAlpha, FADE_OFF, adjustedDuration, fadedCallback);
        }

        private void Fade(float from, float to, float fadeDuration, Action fadedCallback)
        {
            _fadedCallback = fadedCallback;

            if (fadeDuration == 0)
            {
                _currentTween?.Kill();
                _currentTween = null;
                IsFadeInProcess = false;
                _lastAlpha = -1f;

                _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, to);
                fadedCallback?.Invoke();
                return;
            }


            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, from);

            _currentTween = _fade
                .DOFade(to, fadeDuration)
                .OnComplete(() =>
                {
                    IsFadeInProcess = false;
                    _currentTween = null;
                    _lastAlpha = -1f;
                    _fadedCallback?.Invoke();
                });
        }
    }
}
