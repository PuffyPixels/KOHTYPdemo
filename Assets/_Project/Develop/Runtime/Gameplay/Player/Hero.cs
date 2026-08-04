using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.AudioListenerService;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private HeroStress _heroStress;

        private ListenerService _listenerService;
       
        private void Awake()
        {
            Assert.IsNotNull(_heroStress);
        }

        public void Init(Stress stress, Pulse pulse, ListenerService listenerService)
        {
            _heroStress.Init(stress, pulse);
            _listenerService = listenerService;
        }

        private void OnDestroy()
        {
            if (_listenerService != null)
                _listenerService.Enable();
        }
    }
}
