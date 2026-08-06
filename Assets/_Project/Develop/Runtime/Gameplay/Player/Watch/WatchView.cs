using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Watch
{
    public class WatchView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HeroWatch _heroWatch;
        [SerializeField] private WatchGlitchPlayer _glitchPlayer;

        [Header("Detection")]
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private float _detectRadius = 15f;
        [SerializeField] private AnimationCurve _signalCurve;

        private WatchDetector _detector;

        private void Awake()
        {
            Assert.IsNotNull(_heroWatch);
            Assert.IsNotNull(_glitchPlayer);

            _detector = new WatchDetector(
                _heroWatch.transform.root,
                _enemyMask,
                _detectRadius,
                _signalCurve);

            _detector.SignalChanged += OnSignalChanged;
        }

        private void Update()
        {
            if (!_heroWatch.IsOpened)
            {
                _glitchPlayer.SetSignal(
                    new WatchSignal(
                        0f,
                        WatchSignalSource.None),
                    false);

                return;
            }

            _detector.Tick();
        }

        public void CheckItem(bool required)
        {
            if (!_heroWatch.IsOpened)
                return;

            if (_detector.HasEnemyNearby)
                return;

            if (required)
                return;

            _glitchPlayer.SetSignal(
                new WatchSignal(
                    0.33f,
                    WatchSignalSource.Item),
                false);

            _glitchPlayer.PlayImmediate();
        }

        private void OnSignalChanged(WatchSignal signal, bool multipleEnemies)
        {
            _glitchPlayer.SetSignal(signal, multipleEnemies);
        }

        private void OnDestroy()
        {
            _detector.SignalChanged -= OnSignalChanged;
            _detector.Dispose();
        }
    }
}