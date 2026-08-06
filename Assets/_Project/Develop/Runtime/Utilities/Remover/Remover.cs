using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Project.Develop.Runtime.Utilities.Remover
{
    public static class Remover
    {
        public static void ClearDontDestroyAndLoad()
        {
            GameObject tempObj = new("Temp");
            GameObject.DontDestroyOnLoad(tempObj);
            Scene ddolScene = tempObj.scene;
            GameObject.Destroy(tempObj);

            foreach (GameObject rootObj in ddolScene.GetRootGameObjects())
            {
                GameObject.Destroy(rootObj);
            }
        }
    }
}