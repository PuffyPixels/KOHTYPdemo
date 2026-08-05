using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AudioListenerService;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Infrastructure.EntryPoint
{
    public class GameEntryPoint : MonoBehaviour
    {
        private void Awake()
        {
            SetupAppSettings();

            DIContainer projectContainer = new DIContainer();

            ProjectContextRegistrations.Process(projectContainer);

            projectContainer.Initialize();

            projectContainer.Resolve<ICoroutinesPerformer>().StartPerform(Initialize(projectContainer));
        }

        private void SetupAppSettings()
        {
            // Without vSync
            //QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 60;

            // With vSync & buffer
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 2;
        }

        private IEnumerator Initialize(DIContainer container)
        {
            ListenerService listenerService = container.Resolve<ListenerService>();
            listenerService.Enable();

            ILoadingScreen loadingScreen = container.Resolve<ILoadingScreen>();
            SceneSwitcherService sceneSwitcherService = container.Resolve<SceneSwitcherService>();

            yield return sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu, isSkipFade: true);
        }
    }
}
