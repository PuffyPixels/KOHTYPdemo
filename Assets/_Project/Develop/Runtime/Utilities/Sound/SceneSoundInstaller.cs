using Assets._Project.Develop.Runtime.Gameplay.Player;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    //In order to use on any scene:
    //In Bootstrap:
    //private SceneSoundInstaller _sceneSoundInstaller;
    //Initialize() _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();
    //Run() _sceneSoundInstaller.Install();
    //In ContextRegistration:
    //container.RegisterAsSingle(c => new SceneSoundInstaller(c.Resolve<SoundsManager>()));

    public class SceneSoundInstaller
    {
        private readonly SoundsManager _soundsManager;

        public SceneSoundInstaller(SoundsManager soundsManager)
        {
            _soundsManager = soundsManager;
        }

        public void Install()
        {
            foreach (EnvironmentSound environmentSound in Object.FindObjectsByType<EnvironmentSound>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                environmentSound.Init(_soundsManager);
            }

            foreach (PlayerFootsteps footsteps in Object.FindObjectsByType<PlayerFootsteps>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                footsteps.Init(_soundsManager);
            }
        }
    }
}