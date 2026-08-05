using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class HeroWatch : MonoBehaviour
    {
        [SerializeField] private Transform _watch;
        [SerializeField] private Vector3 _hiddenPosition;
        [SerializeField] private Vector3 _shownPosition;
        [SerializeField] private Vector3 _hiddenRotation;
        [SerializeField] private Vector3 _shownRotation;
        [SerializeField] private float _animationDuration = 0.25f;

        private readonly WatchClock _clock = new();

        private Tween _moveTween;
        private Tween _rotationTween;

        public bool IsOpened { get; private set; }

        public event Action<int, int> TimeChanged;

        private void Awake()
        {
            Assert.IsNotNull(_watch);

            _watch.localPosition = _hiddenPosition;
            _watch.localEulerAngles = _hiddenRotation;

            _clock.TimeChanged += OnTimeChanged;
        }

        private void Update()
        {
            _clock.Tick();
        }

        public void Open()
        {
            if (IsOpened)
                return;

            IsOpened = true;
            Animate(_shownPosition, _shownRotation);
        }

        public void Close()
        {
            if (!IsOpened)
                return;

            IsOpened = false;
            Animate(_hiddenPosition, _hiddenRotation);
        }

        private void Animate(Vector3 position, Vector3 rotation)
        {
            _moveTween?.Kill();
            _rotationTween?.Kill();

            _moveTween = _watch
                .DOLocalMove(position, _animationDuration)
                .SetEase(Ease.OutQuad);

            _rotationTween = _watch
                .DOLocalRotate(rotation, _animationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void OnTimeChanged(int hours, int minutes)
        {
            TimeChanged?.Invoke(hours, minutes);
        }

        private void OnDestroy()
        {
            _clock.TimeChanged -= OnTimeChanged;

            _clock.Dispose();

            _moveTween?.Kill();
            _rotationTween?.Kill();
        }
    }

    public enum WatchSignalSource
    {
        None,
        Enemy,
        Item
    }

    public readonly struct WatchSignal
    {
        public readonly float Strength;
        public readonly WatchSignalSource Source;

        public WatchSignal(float strength, WatchSignalSource source)
        {
            Strength = strength;
            Source = source;
        }
    }
}