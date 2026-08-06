using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Infrastructure;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.MainMenu
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        [SerializeField] private AudioClip _menuMusic;

        private DIContainer _container;
        private SceneSoundInstaller _sceneSoundInstaller;
        private MusicManager _musicManager;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            MainMenuContextRegistrations.Process(_container);
        }

        public override IEnumerator Initialize()
        {
            _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();
            _musicManager = _container.Resolve<MusicManager>();

            yield break;
        }

        public override void Run()
        {
            _musicManager.PlayMusic(_menuMusic);

            _sceneSoundInstaller.InitEnvironmentSound();
        }
    }
}
