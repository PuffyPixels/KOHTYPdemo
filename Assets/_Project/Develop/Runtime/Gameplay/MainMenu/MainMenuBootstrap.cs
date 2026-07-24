using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections;

namespace Assets._Project.Develop.Runtime.Gameplay.MainMenu
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private SceneSoundInstaller _sceneSoundInstaller;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            MainMenuContextRegistrations.Process(_container);
        }

        public override IEnumerator Initialize()
        {
            _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();

            yield break;
        }

        public override void Run()
        {
            _sceneSoundInstaller.InitEnvironmentSound();
        }
    }
}
