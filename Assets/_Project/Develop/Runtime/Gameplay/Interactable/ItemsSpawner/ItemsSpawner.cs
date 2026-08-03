using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemsSpawner
{
    public class ItemsSpawner : MonoBehaviour
    {
        [SerializeField]
        private InventoryItemsDatabase _itemsDatabase;

        private List<SpawnPoint> _spawnPoints;
        private Dictionary<ItemType, Transform> _itemsContainers;

        private void Awake()
        {
            _spawnPoints = new(transform.GetComponentsInChildren<SpawnPoint>());
            List<SpawnPoint> itemsSpawnPoints = _spawnPoints.Where(x => !x.IsBusy && x.Type == ItemType.Item).ToList();

            _itemsContainers = new()
            {
                { ItemType.Item, new GameObject("Items").transform },
                { ItemType.Key, new GameObject("Keys").transform },
                { ItemType.Note, new GameObject("Notes").transform }
            };

            Respawn(_itemsDatabase.Items);
        }

        public void Respawn()
        {
            Respawn(_itemsDatabase.Items);
        }

        public void Respawn(IReadOnlyList<InventoryItem> items)
        {
            List<SpawnPoint> freeSpawnPoints = _spawnPoints.Where(x => !x.IsBusy).ToList();

            foreach (InventoryItem item in items)
            {
                var points = freeSpawnPoints.Where(x => x.Type == item.Type);

                if (points.Count() == 0)
                {
                    Debug.LogWarning("Not enough spawn points.");
                    break;
                }

                SpawnPoint point = points.ElementAt(UnityEngine.Random.Range(0, points.Count()));
                point.Spawn(item, _itemsContainers.GetValueOrDefault(item.Type));
                freeSpawnPoints.Remove(point);
            }
        }
    }
}