using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagment
{
    public class SceneSwitcherService
    {
        public const float DEFAULT_FADE_DURATION = 2f;

        private readonly SceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly DIContainer _projectContainer;
        private readonly Fader _fader;
        private bool _isLoading;

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

        public IEnumerator ProcessSwitchTo(
            string sceneName,
            float fadeDuration = DEFAULT_FADE_DURATION,
            bool isSkipFade = false,
            IInputSceneArgs sceneArgs = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            Action callback = null)
        {
            if (_isLoading)
                yield break;

            if (_fader.IsFadeInProcess)
                yield break;

            _isLoading = true;

            if (loadSceneMode == LoadSceneMode.Single && !isSkipFade)
            {
                bool fadeCompleted = false;
                _fader.StopFade();
                _fader.FadeIn(fadeDuration, () => fadeCompleted = true);

                yield return new WaitWhile(() => fadeCompleted == false);

                _currentSceneContainer?.Dispose();
            }

            yield return _sceneLoaderService.LoadAsync(sceneName, loadSceneMode, callback);

            SceneBootstrap sceneBootstrap = GetSceneBootstrap(sceneName);
            Assert.IsNotNull(sceneBootstrap, nameof(sceneBootstrap) + " not found");

            _currentSceneContainer = new DIContainer(_projectContainer);
            sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);
            _currentSceneContainer.Initialize();

            yield return sceneBootstrap.Initialize();

            if (loadSceneMode == LoadSceneMode.Single)
            {
                _fader.FadeOut(fadeDuration);
            }

            _isLoading = false;

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
