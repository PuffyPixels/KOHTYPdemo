using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Shop;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.NavRoute.Navigation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy
{
    public class EnemiesFactory
    {
        private readonly DIContainer _container;
        private readonly BrainsFactory _brainsFactory;
        private readonly GameProgress _gameProgress;

        public EnemiesFactory(DIContainer container)
        {
            _container = container;
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _gameProgress = _container.Resolve<GameProgress>();
        }

        public void CreateConsultant(ConsultantSettings settings)
        {
            ConsultantSettings settingsCached = settings;

            Assert.IsNotNull(settingsCached.Prefab);

            if (settingsCached.SpawnPoints.Count > 0)
            {
                foreach (var spawnPoint in settingsCached.SpawnPoints)
                {
                    if (spawnPoint == null)
                        continue;

                    ConsultantFacade newConsultant = Object.Instantiate(settingsCached.Prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

                    RouteService newRouteService = spawnPoint.GetComponentInParent<RouteService>();
                    newConsultant.Walker.Init(newRouteService);

                    StateMachineBrain newConsultantBrain = _brainsFactory.CreateConsultantBrain(settingsCached, newConsultant);
                    newConsultant.Init(newConsultantBrain);

                    _gameProgress.AddConsultant(newConsultant);
                }
            }
        }
    }
}
