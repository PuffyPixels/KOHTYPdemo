using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemStorage
{
    public class ItemsCollectionHandler : MonoBehaviour
    {
        public event Action<bool> Collected;

        [SerializeField]
        private List<string> _correctItems;
        [SerializeField]
        private List<ItemStorage> _storages;

        private Dictionary<ItemStorage, string> _items = new();

        private void OnItemChanged(ItemStorage storage)
        {
            if (_items.ContainsKey(storage))
            {
                _items[storage] = storage.ItemName;
                CheckCorrection();
            }
        }

        private void Awake()
        {
            foreach (var storage in _storages)
            {
                _items.Add(storage, storage.ItemName);
            }
        }

        private void CheckCorrection()
        {
            if (_items.Values.Count(x => !string.IsNullOrEmpty(x)) == _items.Count)
            {
                if (IsCorrectCollection())
                {
                    FireproofResult();
                    Collected?.Invoke(true);
                }
                else
                    Collected?.Invoke(false);
            }
        }

        private bool IsCorrectCollection()
        {
            foreach (string correctItem in _correctItems)
            {
                if (!_items.Values.Contains(correctItem))
                    return false;
            }

            return true;
        }

        private void FireproofResult()
        {
            _items.Clear();

            foreach (var storage in _storages)
            {
                storage.ItemChanged -= OnItemChanged;
                Destroy(storage.gameObject);
            }
        }

        private void OnEnable()
        {
            foreach (var storage in _storages)
            {
                storage.ItemChanged += OnItemChanged;
            }
        }

        private void OnDisable()
        {
            foreach (var storage in _storages)
            {
                storage.ItemChanged -= OnItemChanged;
            }
        }
    }
}
