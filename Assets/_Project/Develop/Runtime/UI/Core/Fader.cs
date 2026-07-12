using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public class Fader : MonoBehaviour
    {
        private const float FADE_ON = 1f;
        private const float FADE_OFF = 0f;

        [SerializeField] private Image _fade;

        private Action _fadedCallback;
        private Tween _currentTween;
        private float _lastAlpha = -1f;

        public bool IsFadeInProcess { get; private set; } = false;
        public float CurrentAlpha => _fade.color.a;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

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

        public void StopFade(Action fadedCallback = null)
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _lastAlpha = _fade.color.a;

                _currentTween.Kill();
                _currentTween = null;
            }

            IsFadeInProcess = false;
            fadedCallback?.Invoke();
        }

        public void SetAlpha(float alpha)
        {
            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, alpha);
            _lastAlpha = alpha;
        }

        private void OnDestroy()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                _currentTween = null;
            }
        }
    }
}
