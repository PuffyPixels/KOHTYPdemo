using Assets._Project.Develop.Runtime.Gameplay.Enemy;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class ShopBootstrap : SceneBootstrap
    {
        [SerializeField] private ConsultantSettings _consultantSettings;

        private DIContainer _container;
        private ElevatorSwitchManager _elevatorSwitchManager;
        private EnemiesFactory _enemiesFactory;
        private ShopInputArgs _inputArgs;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not ShopInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(ShopInputArgs)} type");

            _inputArgs = gameplayInputArgs;

            ShopContextRegistrations.Process(_container, _inputArgs);
        }

        public override IEnumerator Initialize()
        {
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();
            _enemiesFactory = _container.Resolve<EnemiesFactory>();

            yield break;
        }

        public override void Run()
        {
            _enemiesFactory.CreateConsultant(_consultantSettings);
            _elevatorSwitchManager.SetElevator(1);
        }
    }
}
