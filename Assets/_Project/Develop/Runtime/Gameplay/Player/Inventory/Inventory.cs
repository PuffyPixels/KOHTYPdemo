using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Player.Inventory
{
    public class Inventory
    {
        public event System.Action<Inventory, InventoryItem> ItemAdded;
        public event System.Action<Inventory, InventoryItem> ItemFailed;
        public event System.Action<Inventory, InventoryItem> ItemRemoved;
        public event System.Action<Inventory, InventoryItem> ItemDropped;

        public bool IsEmpty { get; private set; } = true;

        private readonly List<InventoryItem> _items = new();
        public int Capacity { get; }

        public Inventory(int capacity) => Capacity = capacity > 0 ? capacity : 0;

        public bool TryAdd(InventoryItem item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            if (_items.Count >= Capacity)
            {
                ItemFailed?.Invoke(this, item);
                return false;
            }

            _items.Add(item);
            ItemAdded?.Invoke(this, item);
            IsEmpty = false;

            return true;
        }

        public void Remove(InventoryItem item) => Remove(item, ItemRemoved);

        public void RemoveAll()
        {
            _items.Clear();

            IsEmpty = true;
        }

        public void Drop(InventoryItem item) => Remove(item, ItemDropped);

        public void DropFromSlot(int slotId)
        {
            if (slotId < 0 || slotId >= _items.Count)
                throw new System.ArgumentOutOfRangeException(nameof(slotId));

            Remove(_items[slotId], ItemDropped);
        }

        public IReadOnlyList<InventoryItem> Items => _items;

        private void Remove(InventoryItem item, System.Action<Inventory, InventoryItem> callback)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            if (_items.Remove(item))
                callback?.Invoke(this, item);

            if (_items.Count == 0)
                IsEmpty = true;
        }
    }
}