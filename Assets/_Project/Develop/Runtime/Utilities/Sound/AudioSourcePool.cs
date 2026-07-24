using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Sound
{
    public class AudioSourcePool
    {
        private readonly AudioSource _prefab;
        private readonly Transform _parent;
        private readonly Queue<AudioSource> _pool = new();
        private readonly List<AudioSource> _active = new();

        public AudioSourcePool(AudioSource prefab, Transform parent, int prewarmCount = 3)
        {
            _prefab = prefab;
            _parent = parent;

            CreatePool(prewarmCount);
        }

        public AudioSource Get()
        {
            AudioSource source;

            if (_pool.Count > 0)
            {
                source = _pool.Dequeue();
                source.gameObject.SetActive(true);
            }
            else
            {
                source = CreateNew();
            }

            _active.Add(source);

            return source;
        }

        public void Return(AudioSource source)
        {
            if (source == null || !_active.Contains(source))
                return;

            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            source.transform.SetParent(_parent);
            _active.Remove(source);
            _pool.Enqueue(source);
        }

        public void ReturnAll()
        {
            List<AudioSource> activeCopy = new(_active);

            foreach (AudioSource source in activeCopy)
                Return(source);
        }

        public void Clear()
        {
            ReturnAll();

            while (_pool.Count > 0)
            {
                AudioSource source = _pool.Dequeue();

                if (source != null)
                    Object.Destroy(source.gameObject);
            }
        }

        private void CreatePool(int prewarmCount)
        {
            for (int i = 0; i < prewarmCount; i++)
            {
                AudioSource source = CreateNew();
                _pool.Enqueue(source);
            }
        }

        private AudioSource CreateNew()
        {
            AudioSource source = Object.Instantiate(_prefab, _parent);
            source.gameObject.SetActive(false);

            return source;
        }
    }
}
