using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections;
using UnityEngine.InputSystem;

namespace Assets._Project.Develop.Runtime.Gameplay.MainMenu
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
        }

        public override IEnumerator Initialize()
        {
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            yield break;
        }

        public override void Run()
        {

        }

        // FOR TEST = need to delete
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Entrance));
            }
        }
    }
}
