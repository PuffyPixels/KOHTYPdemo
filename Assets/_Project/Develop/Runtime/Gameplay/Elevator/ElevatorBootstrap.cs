using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Shop;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.InteractClue;
using Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget;
using Assets._Project.Develop.Runtime.UI.Gameplay.ItemCollectPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.NotePopup;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using Project.Develop.Runtime.Utilities.Initializing;
using System;
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
        private Hero _hero;

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

            Scene elevatorScene = SceneManager.GetSceneByName(Scenes.Elevator);

            if (elevatorScene.isLoaded)
            {
                SceneManager.SetActiveScene(elevatorScene);
            }

            _hero = _container.Resolve<HeroFactory>().CreateHero(_playerPrefab);
            _container.Resolve<UIRoot>();
            _container.Resolve<GameplayScreenPresenter>().Initialize();
            _container.Resolve<ItemCollectPupupPresenter>().Initialize();
            _container.Resolve<InventoryWidgetPresenter>().Initialize();
            _container.Resolve<InteractCluePresenter>().Initialize();
            _container.Resolve<NotePopupPresenter>().Initialize();

            elevatorController.Init(_container.Resolve<SoundsManager>());

            yield break;
        }

        public override void Run()
        {
            Assert.IsNotNull(elevatorController, "ElevatorController is null. Make sure it's assigned in the inspector or injected correctly.");

            _elevatorSwitchManager.AddElevatorController(elevatorController);
            _elevatorSwitchManager.SetElevator(0);

            _sceneSoundInstaller.InitEnvironmentSound();
            InitElevator();
        }

        private void InitElevator()
        {
            BaseSceneEntitiesInitializer.InitElevatorPanel(_sceneLoaderService, _sceneSwitcherService,
                _coroutinesPerformer, _container, _hero.transform, _elevatorSwitchManager);
            BaseSceneEntitiesInitializer.InitElevatorCallButton();
        }

        private IEnumerator UnloadShopAndLoadMainMenu()
        {
            if (SceneManager.GetSceneByName(Scenes.Shop).isLoaded)
                yield return _sceneLoaderService.UnloadAsync(Scenes.Shop);

            yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu);
        }

        private IEnumerator UnloadEntranceAndLoadShopTest()
        {
            if (SceneManager.GetSceneByName(Scenes.Entrance).isLoaded)
                yield return _sceneLoaderService.UnloadAsync(Scenes.Entrance);

            if (!SceneManager.GetSceneByName(Scenes.Shop).isLoaded)
                yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.Shop, loadSceneMode: LoadSceneMode.Additive,
                    sceneArgs: new ShopInputArgs(_container));
        }



        private void OnDestroy()
        {
            _elevatorSwitchManager.ReleaseElevatorController();
        }

        // FOR TEST = need to delete
        private void Update()
        {
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                _coroutinesPerformer.StartPerform(UnloadEntranceAndLoadShopTest());
            }

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                _coroutinesPerformer.StartPerform(UnloadShopAndLoadMainMenu());
            }
        }
    }
}
