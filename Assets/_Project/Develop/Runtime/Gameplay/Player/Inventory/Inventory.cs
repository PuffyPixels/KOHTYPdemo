using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Inventory
{
    public class Inventory
    {
        public event System.Action<Inventory, InventoryItem> ItemAdded;
        public event System.Action<Inventory, InventoryItem> ItemRemoved;
        public event System.Action<Inventory, InventoryItem> ItemFailed;

        private readonly List<InventoryItem> _items = new();
        private int _capacity;

        public Inventory(int capacity) => _capacity = capacity > 0 ? capacity : 0;

        public void Add(InventoryItem item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            if (_items.Count >= _capacity)
            {
                ItemFailed?.Invoke(this, item);
                return;
            }

            _items.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        public void Remove(InventoryItem item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            _items.Remove(item);
            ItemRemoved?.Invoke(this, item);
        }

        public IReadOnlyList<InventoryItem> Items => _items;
    }
}