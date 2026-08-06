using Assets._Project.Develop.Runtime.Gameplay.Interactable.Item;
using Assets._Project.Develop.Runtime.Gameplay.Settings;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Inventory
{
    public class ItemThrower : IDisposable
    {
        public event Action<CollectableItem> ItemThrown;

        private Inventory _inventory;
        private Transform _thrower;
        private InventoryItemsDatabase _itemsDatabase;

        public ItemThrower(Inventory inventory, InventoryItemsDatabase itemsDatabase, Transform thrower)
        {
            _inventory = inventory;
            _itemsDatabase = itemsDatabase;
            _thrower = thrower;
            _inventory.ItemDropped += OnItemDroppded;
        }

        public void Dispose()
        {
            _inventory.ItemDropped -= OnItemDroppded;
        }

        private void OnItemDroppded(Inventory inventory, InventoryItem item)
        {
            var droppedItem = GameObject.Instantiate(item.CollectableItem, _thrower.position, _thrower.rotation);

            if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = (_thrower.forward + _thrower.up * Settings.Settings.THROW_ITEM_VECTOR_FACTOR).normalized;
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(direction * Settings.Settings.THROW_ITEM_IMPULSE, ForceMode.Impulse);
            }

            droppedItem.Init(inventory, _itemsDatabase);
            ItemThrown?.Invoke(droppedItem);
        }
    }
}