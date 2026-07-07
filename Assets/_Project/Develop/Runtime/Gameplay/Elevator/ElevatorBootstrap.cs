using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Elevator
{
    public class ElevatorBootstrap : SceneBootstrap
    {
        [SerializeField] private ElevatorController elevatorController;

        private DIContainer _container;
        private SceneSwitcherService _sceneSwitcherService;
        private SceneLoaderService _sceneLoaderService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private ElevatorSwitchManager _elevatorSwitchManager;


        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
        }

        public override IEnumerator Initialize()
        {
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _sceneLoaderService = _container.Resolve<SceneLoaderService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();

            yield break;
        }

        public override void Run()
        {
            if (elevatorController == null)
                throw new ArgumentNullException(nameof(elevatorController));

            _elevatorSwitchManager.AddElevator(elevatorController);
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
