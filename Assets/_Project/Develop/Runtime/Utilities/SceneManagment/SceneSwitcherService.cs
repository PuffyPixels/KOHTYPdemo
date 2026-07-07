using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagment
{
    public class SceneSwitcherService
    {
        private readonly SceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly DIContainer _projectContainer;

        private DIContainer _currentSceneContainer;

        public SceneSwitcherService(
            SceneLoaderService sceneLoaderService, 
            ILoadingScreen loadingScreen,
            DIContainer projectContainer)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreen = loadingScreen;
            _projectContainer = projectContainer;
        }

        public IEnumerator ProcessSwitchTo(string sceneName, IInputSceneArgs sceneArgs = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                _loadingScreen.Show();
                _currentSceneContainer?.Dispose();
            }

            yield return _sceneLoaderService.LoadAsync(sceneName, loadSceneMode);

            SceneBootstrap sceneBootstrap = GetSceneBootstrap(sceneName);

            if (sceneBootstrap == null)
                throw new NullReferenceException(nameof(sceneBootstrap) + " not found");

            _currentSceneContainer = new DIContainer(_projectContainer);

            sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);

            _currentSceneContainer.Initialize();

            yield return sceneBootstrap.Initialize();

            if (loadSceneMode == LoadSceneMode.Single)
                _loadingScreen.Hide();

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
