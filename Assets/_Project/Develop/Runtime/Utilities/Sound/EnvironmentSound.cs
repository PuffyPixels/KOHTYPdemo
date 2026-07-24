using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    //Should be on an environment object that plays sounds
    public class EnvironmentSound : MonoBehaviour
    {
        [SerializeField] private bool _shouldPlayLooped;
        [SerializeField] private bool _isOwned;
        [SerializeField] private bool _is2D;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private float _soundInterval = DEFAULT_SOUND_INTERVAL;
        [Range(0, 1)][SerializeField] private float _playSoundChance;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _maxDistance = 17f;

        private Coroutine _playSoundRoutine;
        private SoundsManager _soundsManager;
        private WaitForSeconds _playSoundWait;
        private bool _shouldPlayInterval;

        private const float DEFAULT_SOUND_INTERVAL = -1f;

        public void Init(SoundsManager soundsManager)
        {
            _soundsManager = soundsManager;

            if (_soundInterval != DEFAULT_SOUND_INTERVAL)
            {
                _shouldPlayInterval = true;
                _playSoundWait = new(_soundInterval);
            }

            if (_shouldPlayLooped)
            {
                PlaySound();
            }
        }

        private void OnEnable()
        {
            if (_shouldPlayInterval)
            {
                if (_playSoundRoutine != null)
                {
                    StopCoroutine(_playSoundRoutine);
                    _playSoundRoutine = null;
                }
                
                _playSoundRoutine = StartCoroutine(PlaySoundRoutine());
            }
        }

        private void OnDisable()
        {
            if (_shouldPlayInterval)
            {
                if (_playSoundRoutine != null)
                {
                    StopCoroutine(_playSoundRoutine);
                    _playSoundRoutine = null;
                }
            }

            if (_soundsManager != null && _isOwned)
            {
                _soundsManager.StopSound(transform);
            }
        }

        /// <summary>
        /// May be used in triggers
        /// </summary>
        public void PlaySound()
        {
            Assert.IsNotNull(_soundsManager);

            _soundsManager.PlaySound(
                _audioClip,
                volume: _volume,
                maxDistance: _maxDistance,
                spawnPosition: transform,
                isLooped: _shouldPlayLooped,
                owner: _isOwned ? transform : null,
                is2D: _is2D);
        }

        public void FadeVolume(float targetVolume, float duration = 1f)
        {
            Assert.IsNotNull(_soundsManager);

            _soundsManager.FadeVolume(
                _isOwned ? transform : null,
                targetVolume,
                duration);
        }

        /// <summary>
        /// Randomly plays a sound every once in a while
        /// </summary>
        private IEnumerator PlaySoundRoutine()
        {
            while (true)
            {
                yield return _playSoundWait;

                if (_soundsManager == null)
                    yield break;

                if (UnityEngine.Random.Range(0f, 1f) < _playSoundChance)
                { 
                    _soundsManager.PlaySound(
                        _audioClip, 
                        volume: _volume, 
                        spawnPosition: transform,
                        is2D: _is2D);
                }
            }
        }
    }
}