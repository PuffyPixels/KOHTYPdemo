using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private HeroStress _heroStress;
        [SerializeField] private LayerMask _enemyMask;
        private SphereCollider _aura;

        private void Awake()
        {
            Assert.IsNotNull(_heroStress);
        }

        private void Start()
        {
            _aura = _heroStress.Aura;
        }

        private void Update()
        {
            GetAngryEnemyByStressAura();
        }

        private void GetAngryEnemyByStressAura()
        {
            if (_heroStress.InPanic)
            {
                Collider[] targetsInStressRadius = Physics.OverlapSphere(transform.position, _aura.radius, _enemyMask);

                foreach (Collider targetCollider in targetsInStressRadius)
                {
                    if (!targetCollider.TryGetComponent(out ConsultantFacade consultant))
                        continue;

                    consultant.DetectPlayer(this);
                }
            }
        }

        public void Init(Stress stress, Pulse pulse)
        {
            _heroStress.Init(stress, pulse);
        }
    }
}
