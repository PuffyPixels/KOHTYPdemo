using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    [Serializable]
    public struct MixerMapping
    {
        public MixerType Type;
        public AudioMixerGroup MixerGroup;
    }

    [Serializable]
    public struct PooledAudioData
    {
        public AudioSource AudioSource;
        public bool Is2D;

        public PooledAudioData(AudioSource audioSource, bool is2D)
        {
            AudioSource = audioSource;
            Is2D = is2D;
        }
    }

    public enum MixerType
    {
        Master,
        SFX,
        UI,
        Voice,
    }

    public class SoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource3D;
        [SerializeField] private Transform _parent3D;
        [SerializeField] private AudioSource _audioSource2D;
        [SerializeField] private Transform _parent2D;
        [SerializeField] private List<MixerMapping> _mixerMappings = new();

        private readonly List<AudioSource> _activeAudioSources = new();
        private readonly Dictionary<Transform, PooledAudioData> _objectAudioMap = new();
        private readonly Dictionary<MixerType, AudioMixerGroup> _mixerDictionary = new();
        private readonly Dictionary<Transform, Coroutine> _fadeCoroutines = new();

        private AudioSourcePool _pool3D;
        private AudioSourcePool _pool2D;

        private void Awake()
        {
            InitializeMixerDictionary();
            DontDestroyOnLoad(gameObject);

            _pool3D = new AudioSourcePool(_audioSource3D, _parent3D, 5);
            _pool2D = new AudioSourcePool(_audioSource2D, _parent2D, 3);
        }

        public AudioMixerGroup GetMixer()
        { 
            return _mixerMappings[0].MixerGroup;
        }

        /// <summary>
        /// Spawns a sound object.
        /// <param name = "owner"> If there is an owner, the next sounds spawned with this owner will stop the previous one </param>
        /// <param name = "lowestPitch"> Lowest possible random pitch </param>
        /// <param name = "lowestPitch"> Lowest possible random pitch </param>
        /// <param name = "highestPitch"> Highest possible random pitch </param>
        /// </summary>
        public void PlaySound(
            AudioClip clip,
            MixerType mixerType = MixerType.Master,
            float lowestPitch = 1f,
            float highestPitch = 1f,
            Transform spawnPosition = null,
            float volume = 1f,
            float maxDistance = 17f,
            Transform owner = null,
            bool is2D = false,
            bool isLooped = false)
        {
            if(clip == null)
            {
                Debug.LogError($"Sound clip {owner.name} is null");
                return;
            }

            if (owner != null && _objectAudioMap.ContainsKey(owner))
            {
                StopSound(owner);
            }

            Vector3 spawnPos = spawnPosition != null ? spawnPosition.position : transform.position;

            Transform parent;

            if (spawnPosition != null)
            {
                parent = spawnPosition;
            }
            else
            {
                parent = is2D ? _parent2D : _parent3D;
            }

            AudioSource audioSource = is2D ? _pool2D.Get() : _pool3D.Get();

            audioSource.transform.position = spawnPos;
            audioSource.transform.SetParent(parent);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.maxDistance = maxDistance;
            audioSource.loop = isLooped;

            if (_mixerDictionary.TryGetValue(mixerType, out AudioMixerGroup mixerGroup) && mixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = mixerGroup;
            }

            if (lowestPitch != highestPitch)
            {
                float randomPitch = Random.Range(lowestPitch, highestPitch);
                audioSource.pitch = randomPitch;
            }

            audioSource.Play();
            _activeAudioSources.Add(audioSource);

            if (owner != null)
            {
                _objectAudioMap[owner] = new PooledAudioData(audioSource, is2D);
            }

            if (!isLooped)
            {
                StartCoroutine(DestroyAfterPlayback(audioSource, owner, is2D));
            }
        }

        public void FadeVolume(Transform owner, float targetVolume, float duration)
        {
            if (owner == null || !_objectAudioMap.TryGetValue(owner, out PooledAudioData data) || data.AudioSource == null)
                return;

            if (_fadeCoroutines.TryGetValue(owner, out Coroutine oldCoroutine))
            {
                StopCoroutine(oldCoroutine);
                _fadeCoroutines.Remove(owner);
            }

            Coroutine newCoroutine = StartCoroutine(FadeVolumeRoutine(owner, data.AudioSource, targetVolume, duration));
            _fadeCoroutines[owner] = newCoroutine;
        }

        /// <summary>
        /// Stops playing sound if it's already playing
        /// </summary>
        public void StopSound(Transform owner)
        {
            if (owner == null)
                return;

            if (_objectAudioMap.TryGetValue(owner, out PooledAudioData data) && data.AudioSource != null)
            {
                if (_fadeCoroutines.TryGetValue(owner, out Coroutine coroutine))
                {
                    StopCoroutine(coroutine);
                    _fadeCoroutines.Remove(owner);
                }

                data.AudioSource.Stop();
                _objectAudioMap.Remove(owner);

                ReturnToPool(data.AudioSource, data.Is2D);
            }
        }

        public void StopAllSounds()
        {
            if (_fadeCoroutines.Count > 0)
            {
                foreach (var fadeCoroutine in _fadeCoroutines)
                {
                    StopCoroutine(fadeCoroutine.Value);
                }

                _fadeCoroutines.Clear();
            }

            foreach (var owner in _objectAudioMap.Keys.ToList())
            {
                StopSound(owner);
            }
        }

        private void ReturnToPool(AudioSource audioSource, bool is2D)
        {
            if (audioSource == null)
                return;

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.gameObject.SetActive(false);

            if (is2D)
                _pool2D.Return(audioSource);
            else
                _pool3D.Return(audioSource);

            _activeAudioSources.Remove(audioSource);
        }

        private void InitializeMixerDictionary()
        {
            _mixerDictionary.Clear();

            foreach (var mapping in _mixerMappings)
            {
                if (mapping.MixerGroup != null)
                {
                    _mixerDictionary[mapping.Type] = mapping.MixerGroup;
                }
            }
        }

        private IEnumerator FadeVolumeRoutine(Transform owner, AudioSource audioSource, float targetVolume, float duration)
        {
            float startVolume = audioSource.volume;

            if (startVolume == targetVolume)
            {
                _fadeCoroutines.Remove(owner);

                yield break;
            }


            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);

                yield return null;
            }

            audioSource.volume = targetVolume;
            _fadeCoroutines.Remove(owner);
        }

        /// <summary>
        /// Destroys spawned sound object
        /// </summary>
        private IEnumerator DestroyAfterPlayback(AudioSource audioSource, Transform owner, bool is2D)
        {
            yield return new WaitForSeconds(audioSource.clip.length);

            if (audioSource == null)
                yield break;

            if (owner != null && _objectAudioMap.TryGetValue(owner, out PooledAudioData data) && data.AudioSource == audioSource)
            {
                _objectAudioMap.Remove(owner);
            }

            ReturnToPool(audioSource, is2D);
        }

        private void OnDestroy()
        {
            _pool2D?.Clear();
            _pool3D?.Clear();
        }
    }
}