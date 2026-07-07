using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagment
{
    public class SceneLoaderService
    {
        public IEnumerator LoadAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            AsyncOperation wait = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

            yield return new WaitWhile(() => wait.isDone == false);

            Debug.Log($"Scene loaded: {sceneName}");
        }

        public IEnumerator UnloadAsync(string sceneName)
        {
            AsyncOperation wait = SceneManager.UnloadSceneAsync(sceneName);

            yield return new WaitWhile(() => wait.isDone == false);

            Debug.Log($"Scene unloaded: {sceneName}");
        }
    }
}
