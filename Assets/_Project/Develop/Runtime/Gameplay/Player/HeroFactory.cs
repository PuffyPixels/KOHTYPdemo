using Assets._Project.Develop.Runtime.Gameplay.Elevator;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using Assets._Project.Develop.Runtime.Utilities.StressSystem;
using Assets._Project.Develop.Runtime.Utilities.Timers;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Player
{
    public class HeroFactory : IDisposable
    {
        private const float SECOND = 1f;
        private const float PULSE_BPM_MIN = 70f;
        private const float PULSE_BPM_MAX = 200f;

        private Vector3 _heroPosition;
        private Quaternion _heroRotation;

        private readonly DIContainer _container;
        private readonly SceneSoundInstaller _sceneSoundInstaller;
        private ItemThrower _itemThrower;

        public HeroFactory(
            DIContainer container, 
            ElevatorInputArgs inputArgs)
        {
            _container = container;
            _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();
            _heroPosition = inputArgs.PlayerSpawnPointPosition;
            _heroRotation = inputArgs.PlayerSpawnPointRotation;
        }

        public void CreateHero(Hero heroPrefab)
        {
            LoopTimer loopTimer = new(SECOND, _container.Resolve<ICoroutinesPerformer>());
            Stress stress = new(loopTimer);
            loopTimer.Start();

            Pulse pulse = new(stress, PULSE_BPM_MIN, PULSE_BPM_MAX);

            Hero hero = Object.Instantiate(heroPrefab, _heroPosition, _heroRotation);
            hero.Init(stress, pulse);
            _sceneSoundInstaller.InitFootsteps(hero.transform);
            _itemThrower = new(_container.Resolve<Inventory.Inventory>(),
                _container.Resolve<InventoryItemsDatabase>(), hero.transform);
        }

        public void Dispose()
        {
            _itemThrower?.Dispose();
        }
    }
}
