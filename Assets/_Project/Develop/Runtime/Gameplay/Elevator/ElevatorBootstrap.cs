using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorBootstrap : SceneBootstrap
    {
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] private Hero _playerPrefab;

        private DIContainer _container;
        private SceneSwitcherService _sceneSwitcherService;
        private SceneLoaderService _sceneLoaderService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private ElevatorSwitchManager _elevatorSwitchManager;
        private ElevatorInputArgs _inputArgs;
        private SceneSoundInstaller _sceneSoundInstaller;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not ElevatorInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(ElevatorInputArgs)} type");

            _inputArgs = gameplayInputArgs;

            ElevatorContextRegistrations.Process(_container, _inputArgs);
        }

        public override IEnumerator Initialize()
        {
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _sceneLoaderService = _container.Resolve<SceneLoaderService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();
            _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();

            yield break;
        }

        public override void Run()
        {
            _sceneSoundInstaller.Install();

            Assert.IsNotNull(elevatorController, "ElevatorController is null. Make sure it's assigned in the inspector or injected correctly.");

            _elevatorSwitchManager.AddElevator(elevatorController);

            _container.Resolve<HeroFactory>().CreateHero(_playerPrefab);
        }

        private IEnumerator UnloadEntranceAndLoadShop()
        {
            if (SceneManager.GetSceneByName(Scenes.Entrance).isLoaded)
                yield return _sceneLoaderService.UnloadAsync(Scenes.Entrance);

            if (!SceneManager.GetSceneByName(Scenes.Shop).isLoaded)
                yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.Shop, loadSceneMode: LoadSceneMode.Additive);
        }

        private IEnumerator UnloadShopAndLoadMainMenu()
        {
            if (SceneManager.GetSceneByName(Scenes.Shop).isLoaded)
                yield return _sceneLoaderService.UnloadAsync(Scenes.Shop);

            yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu);
        }

        private void OnDestroy()
        {
            _elevatorSwitchManager.RemoveElevator();
        }

        // FOR TEST = need to delete
        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                _coroutinesPerformer.StartPerform(UnloadEntranceAndLoadShop());
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                _coroutinesPerformer.StartPerform(UnloadShopAndLoadMainMenu());
            }
        }
    }
}
