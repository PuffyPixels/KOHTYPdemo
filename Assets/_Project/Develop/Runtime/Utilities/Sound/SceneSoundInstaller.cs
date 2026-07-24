using Assets._Project.Develop.Runtime.Gameplay.Player;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    /*
        In order to use on any scene:

        In Bootstrap:
            filed: private SceneSoundInstaller _sceneSoundInstaller;
            .Initialize(): _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();
            .Run(): _sceneSoundInstaller.InitEnvironmentSound();

        In Units factory (for footsteps):
            field: private SceneSoundInstaller _sceneSoundInstaller;
            .constructor(container): _sceneSoundInstaller = _container.Resolve<SceneSoundInstaller>();
            .CreateUnitMethod(): _sceneSoundInstaller.InitFootsteps(hero.transform);
    */

    public class SceneSoundInstaller
    {
        private readonly SoundsManager _soundsManager;

        public SceneSoundInstaller(SoundsManager soundsManager)
        {
            _soundsManager = soundsManager;
        }

        public void InitEnvironmentSound()
        {
            EnvironmentSound[] environmentSounds = Object.FindObjectsByType<EnvironmentSound>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (EnvironmentSound environmentSound in environmentSounds)
                environmentSound.Init(_soundsManager);
        }

        public void InitFootsteps(Transform target)
        {
            PlayerFootsteps[] playerFootsteps = target.GetComponentsInChildren<PlayerFootsteps>();

            foreach (PlayerFootsteps footsteps in playerFootsteps)
                footsteps.Init(_soundsManager);
        }
    }
}