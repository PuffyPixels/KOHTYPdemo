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

            Scene elevatorScene = SceneManager.GetSceneByName(Scenes.Elevator);

            if (elevatorScene.isLoaded)
            {
                SceneManager.SetActiveScene(elevatorScene);
            }

            _container.Resolve<HeroFactory>().CreateHero(_playerPrefab);
            _container.Resolve<UIRoot>();
            _container.Resolve<GameplayScreenPresenter>().Initialize();
            _container.Resolve<ItemCollectPupupPresenter>().Initialize();
            _container.Resolve<InventoryWidgetPresenter>().Initialize();
            _container.Resolve<InteractCluePresenter>().Initialize();
            _container.Resolve<NotePopupPresenter>().Initialize();
            yield break;
        }

        public override void Run()
        {
            Assert.IsNotNull(elevatorController, "ElevatorController is null. Make sure it's assigned in the inspector or injected correctly.");

            

            _elevatorSwitchManager.AddElevator(elevatorController);
            
            _sceneSoundInstaller.InitEnvironmentSound();
            _coroutinesPerformer.StartPerform(InitElevator());
        }

        private IEnumerator InitElevator()
        {
            yield return new WaitForSeconds(0.5f);

            Hero hero = GameObject.FindFirstObjectByType<Hero>();
            Assert.IsNotNull(hero, "Hero is null. Make sure it's was created before elevator initialization.");
            BaseSceneEntitiesInitializer.InitElevatorPanel(_sceneLoaderService, _sceneSwitcherService,
                _coroutinesPerformer, _container, hero.transform);
            BaseSceneEntitiesInitializer.InitElevatorCallButton();
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
        //private void Update()
        //{
        //    if (Keyboard.current.aKey.wasPressedThisFrame)
        //    {
        //        _coroutinesPerformer.StartPerform(UnloadEntranceAndLoadShop());
        //    }

        //    if (Keyboard.current.sKey.wasPressedThisFrame)
        //    {
        //        _coroutinesPerformer.StartPerform(UnloadShopAndLoadMainMenu());
        //    }
        //}
    }
}
