using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagment
{
    public class SceneSwitcherService
    {
        public const float DEFAULT_FADE_DURATION = 5f;

        private readonly SceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly DIContainer _projectContainer;
        private readonly Fader _fader;

        private DIContainer _currentSceneContainer;

        public SceneSwitcherService(
            SceneLoaderService sceneLoaderService, 
            ILoadingScreen loadingScreen,
            Fader fader,
            DIContainer projectContainer)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreen = loadingScreen;
            _projectContainer = projectContainer;
            _fader = fader;
        }

        public IEnumerator ProcessSwitchTo(string sceneName, float fadeDuration = DEFAULT_FADE_DURATION, bool isSkipFade = false, IInputSceneArgs sceneArgs = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            bool isNeedFading = false;

            if (loadSceneMode == LoadSceneMode.Single && !isSkipFade)
            {
                isNeedFading = true;

                if (_fader.IsFading)
                    _fader.StopFade(() => isNeedFading = false);

                _fader.FadeIn(fadeDuration, () => isNeedFading = false);

                _currentSceneContainer?.Dispose();
            }

            yield return new WaitWhile(() => isNeedFading);

            yield return _sceneLoaderService.LoadAsync(sceneName, loadSceneMode);

            SceneBootstrap sceneBootstrap = GetSceneBootstrap(sceneName);

            if (sceneBootstrap == null)
                throw new NullReferenceException(nameof(sceneBootstrap) + " not found");

            _currentSceneContainer = new DIContainer(_projectContainer);

            sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);

            _currentSceneContainer.Initialize();

            yield return sceneBootstrap.Initialize();

            if (loadSceneMode == LoadSceneMode.Single)
            {
                isNeedFading = true;

                yield return new WaitWhile(() => _fader.IsFading);

                _fader.FadeOut(fadeDuration);
            }

            sceneBootstrap.Run();
        }

        private SceneBootstrap GetSceneBootstrap(string sceneName)
        {
            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            if (!targetScene.isLoaded || !targetScene.IsValid())
                return null;

            GameObject[] rootObjects = targetScene.GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                SceneBootstrap bootstrap = root.GetComponentInChildren<SceneBootstrap>();
                if (bootstrap != null)
                    return bootstrap;
            }

            return null;
        }
    }
}
