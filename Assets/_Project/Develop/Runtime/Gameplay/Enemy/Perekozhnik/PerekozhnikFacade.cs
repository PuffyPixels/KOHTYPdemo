using Assets._Project.Develop.Runtime.Utilities.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Gameplay.Enemy.Perekozhnik
{
    public class PerekozhnikFacade : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private List<AudioClip> _sounds;
        [SerializeField] private float _soundsIntervalMin = 3f;
        [SerializeField] private float _soundsIntervalMax = 8f;
        [SerializeField] private float _maxDistance = 5f;

        private SoundsManager _soundsManager;
        private float _soundTimer;

        private bool _isActive = false;

        public void Init(SoundsManager soundsManager)
        {
            Assert.IsNotNull(soundsManager);

            _soundsManager = soundsManager;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            _soundTimer -= Time.deltaTime;

            if (_soundTimer <= 0f)
            {
                AudioClip clip = _sounds[Random.Range(0, _sounds.Count - 1)];
                PlaySound(clip);

                _soundTimer = Random.Range(_soundsIntervalMin, _soundsIntervalMax);
            }
        }

        protected void PlaySound(AudioClip clip, float lowestPitch = 0.8f, float highestPitch = 1.2f)
        {
            if (_soundsManager == null || clip == null)
                return;

            _soundsManager.PlaySound(
                clip,
                spawnPosition: transform,
                lowestPitch: lowestPitch,
                highestPitch: highestPitch,
                maxDistance: _maxDistance
            );
        }

        private void OnDestroy()
        {
            _isActive = false;
        }
    }
}
