using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.AudioListenerService
{
    public class ListenerService : MonoBehaviour
    {
        [SerializeField] private AudioListener _globalListener;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Enable() => _globalListener.enabled = true;
        public void Disable() => _globalListener.enabled = false;
    }
}
