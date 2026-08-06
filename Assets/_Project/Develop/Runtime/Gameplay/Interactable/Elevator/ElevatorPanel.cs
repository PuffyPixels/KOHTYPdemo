using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Shop;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using Project.Develop.Runtime.Utilities.Initializing;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.Elevator
{
    public class ElevatorPanel : Selectable
    {
        [SerializeField]
        ElevatorHandler _elevatorHandler;
        [SerializeField]
        private AudioClip _buttonClicked;
        [SerializeField]
        private AudioClip _doorClose;
        [SerializeField]
        private AudioClip _doorOpen;
        private SceneLoaderService _sceneLoaderService;
        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private ElevatorSwitchManager _elevatorSwitchManager;
        private SoundsManager _soundsManager;
        private DIContainer _gameContainer;
        private Hero _hero;

        public override float InteractionDistance => Settings.Settings.ELEVATOR_PANEL_INTERACTION_DISTANCE;

        public void Init(SceneLoaderService sceneLoaderService, SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer, DIContainer gameContainer, Hero hero, ElevatorSwitchManager elevatorSwitchManager)
        {
            _sceneLoaderService = sceneLoaderService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _gameContainer = gameContainer;
            _soundsManager = gameContainer.Resolve<SoundsManager>();
            _hero = hero;
            _elevatorSwitchManager = elevatorSwitchManager;
        }

        public override void Interact()
        {
            _coroutinesPerformer.StartPerform(UnloadEntranceAndLoadShop());
            Destroy(this);
        }

        private IEnumerator UnloadEntranceAndLoadShop()
        {
            _soundsManager.PlaySound(_buttonClicked);
            Transform elevatorParent = transform;
            _soundsManager.PlaySound(_doorClose);
            yield return _elevatorHandler.CloseDoors();
            Transform heroParent = _hero.transform.parent;
            _hero.transform.parent = elevatorParent;

            if (SceneManager.GetSceneByName(Scenes.Entrance).isLoaded)
                yield return _sceneLoaderService.UnloadAsync(Scenes.Entrance);

            if (!SceneManager.GetSceneByName(Scenes.Shop).isLoaded)
                yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.Shop, loadSceneMode: LoadSceneMode.Additive,
                    sceneArgs: new ShopInputArgs(_gameContainer, _hero));

            yield return _elevatorHandler.Move();
            _hero.transform.parent = heroParent;
            _soundsManager.PlaySound(_doorOpen);
            yield return _elevatorHandler.OpenDoors();
            yield return _elevatorHandler.ShowDoorsImageRoutine();

            _elevatorSwitchManager.SetElevator(1);
            BaseSceneEntitiesInitializer.InitFinalElevator(_coroutinesPerformer, _sceneSwitcherService);
        }
    }
}