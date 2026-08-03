using Assets._Project.Develop.Runtime.Utilities.Sound;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.LoadingScreen
{
    public class StandardLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private AudioClip _startGameSound;

        private SoundsManager _soundManager;
        public bool IsShown => gameObject.activeSelf;

        public void Init(SoundsManager soundManager)
        {
            _soundManager = soundManager;
        }

        private void Awake()
        {
            Hide();
            DontDestroyOnLoad(this);
        }

        public void Hide() => gameObject.SetActive(false);

        public void Show() => gameObject.SetActive(true);

        public void PlayLoadingSound() => _soundManager.PlaySound(_startGameSound, volume: 0.2f, is2D: true);

        public float GetSoundDuration() => _startGameSound.length;
    }
}
