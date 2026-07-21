using Assets._Project.Develop.Runtime.Gameplay.Elevator;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Entrance
{
    public class EntranceBootstrap : SceneBootstrap
    {
        [SerializeField] Transform _playerSpawnPoint;

        private DIContainer _container;
        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private ElevatorSwitchManager _elevatorSwitchManager;
        private SceneSoundInstaller _sceneSoundInstaller;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            container.RegisterAsSingle(c => new SceneSoundInstaller(c.Resolve<SoundsManager>()));
        }

        public override IEnumerator Initialize()
        {
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();
            _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();

            yield break;
        }

        public override void Run()
        {
            _coroutinesPerformer.StartPerform(LoadElevator());
            _sceneSoundInstaller.Install();
        }

        private IEnumerator LoadElevator()
        {
            bool isElevatorSceneReady = false;

            yield return _sceneSwitcherService.ProcessSwitchTo(
                Scenes.Elevator,
                loadSceneMode: LoadSceneMode.Additive,
                sceneArgs: new ElevatorInputArgs(_playerSpawnPoint),
                callback: () => isElevatorSceneReady = true);

            yield return new WaitWhile(() => isElevatorSceneReady == false);

            _elevatorSwitchManager.SetElevator(0);
        }
    }
}
