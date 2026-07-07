using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.ElevatorManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Entrance
{
    public class EntranceBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;
        private ElevatorSwitchManager _elevatorSwitchManager;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
        }

        public override IEnumerator Initialize()
        {
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            _elevatorSwitchManager = _container.Resolve<ElevatorSwitchManager>();

            yield break;
        }

        public override void Run()
        {
            _coroutinesPerformer.StartPerform(LoadElevator1());
        }

        private IEnumerator LoadElevator1()
        {
            yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.Elevator, loadSceneMode: LoadSceneMode.Additive);

            _elevatorSwitchManager.SetElevator(0);
        }
    }
}
