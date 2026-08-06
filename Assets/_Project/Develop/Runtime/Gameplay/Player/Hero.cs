using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.AudioListenerService;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using DyrdaDev.FirstPersonController;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private HeroStress _heroStress;
        [SerializeField] private InputActionBasedFirstPersonControllerInput _input;
        [SerializeField] private Image _stunImage;

        private ListenerService _listenerService;

        private const float STUN_DURATION = 2f;
        private readonly WaitForSeconds _waitForStun = new(STUN_DURATION);

        private Coroutine _stunCoroutine = null;
       
        private void Awake()
        {
            Assert.IsNotNull(_heroStress);
        }

        public void Init(Stress stress, Pulse pulse, ListenerService listenerService)
        {
            _heroStress.Init(stress, pulse);
            _listenerService = listenerService;
            _stunImage.enabled = false;
        }

        public void Stun()
        {
            _heroStress.Stun();

            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
                _stunCoroutine = null;
            }

            _stunCoroutine = StartCoroutine(StunRoutine());
        }

        private IEnumerator StunRoutine()
        {
            _input.enabled = false;
            _stunImage.enabled = true;

            yield return _waitForStun;

            _stunImage.enabled = false;
            _input.enabled = true;

        }

        private void OnDestroy()
        {
            if (_listenerService != null)
                _listenerService.Enable();

            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
                _stunCoroutine = null;
            }
                
        }
    }
}
