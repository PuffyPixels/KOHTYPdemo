using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private HeroStress _heroStress;

        private void Awake()
        {
            Assert.IsNotNull(_heroStress);
        }

        public void Init(Stress stress, Pulse pulse)
        {
            _heroStress.Init(stress, pulse);
        }
    }
}
