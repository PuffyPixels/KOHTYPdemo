using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class LightBlink : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private float _maxValue = 1f;
        [SerializeField] private float _timeToBlinkMin = 1f;
        [SerializeField] private float _timeToBlinkMax = 10f;

        private float _defaultValue;
        private Coroutine _blinkCoroutine;
        private bool _blinking = false;

        private WaitForSeconds _blinkWait = new(0.03f);

        private void Start()
        {
            Assert.IsNotNull( _light );

            _defaultValue = _light.intensity;

            _blinking = true;
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }

        private IEnumerator BlinkRoutine()
        {
            while (_blinking)
            {
                float blinkCD = Random.Range(_timeToBlinkMin, _timeToBlinkMax);

                _light.intensity = _maxValue;

                yield return _blinkWait;

                _light.intensity = _defaultValue;

                yield return new WaitForSeconds(blinkCD);
            }
        }

        private void OnDestroy()
        {
            _blinking = false;

            if ( _blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
        }
    }
}
