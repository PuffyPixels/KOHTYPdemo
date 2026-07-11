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

        public bool IsFading { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void FadeIn(float fadeDuration, Action fadedCallback = null)
        {
            Fade(FADE_OFF, FADE_ON, fadeDuration, fadedCallback);
        }

        public void FadeOut(float fadeDuration, Action fadedCallback = null)
        {
            Fade(FADE_ON, FADE_OFF, fadeDuration, fadedCallback);
        }

        private void Fade(float from, float to, float fadeDuration, Action fadedCallback)
        {
            Debug.Log($"from: {from} & to: {to}");
            Debug.Log(IsFading);
            if (IsFading)
                return;

            _fadedCallback = fadedCallback;

            if (fadeDuration == 0)
            {
                _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, to);
                _fadedCallback?.Invoke();

                return;
            }

            IsFading = true;

            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, from);

            _fade
                .DOFade(to, fadeDuration)
                .OnComplete(() =>
                {
                    IsFading = false;
                    _fadedCallback?.Invoke();
                });
        }

        public void StopFade(Action fadedCallback)
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                _currentTween = null;
            }

            IsFading = false;

            fadedCallback?.Invoke();
        }
    }
}
