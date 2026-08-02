using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemsSpawner
{
    public class ItemsSpawner : MonoBehaviour
    {
        [SerializeField]
        private InventoryItemsDatabase _itemsDatabase;

        private List<Transform> _spawnPoints;

        private void Awake()
        {
            _spawnPoints = new();

            foreach (Transform child in transform)
                _spawnPoints.Add(child);

            foreach (InventoryItem item in _itemsDatabase)
            {
                if (_spawnPoints.Count == 0)
                    break;

                Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                Instantiate(item.CollectableItem, spawnPoint.position, spawnPoint.rotation);
                _spawnPoints.Remove(spawnPoint);
            }
        }
    }
}