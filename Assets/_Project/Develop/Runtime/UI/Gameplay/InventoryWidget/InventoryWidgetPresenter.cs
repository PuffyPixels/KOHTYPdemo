using Assets._Project.Develop.Runtime.Gameplay.Input;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.InventoryWidget
{
    public class InventoryWidgetPresenter : IPresenter
    {
        public event Action<IReadOnlyCollection<InventoryItem>> ItemsChanged;
        public event Action<int> SlotSelected;
        private List<InventoryItem> _items;
        private Inventory _inventory;
        private InventoryWidgetView _view;
        private AdditionalInputController _input;
        private int _selectedSlot = -1;

        public int Capacity => _inventory.Capacity;

        public InventoryWidgetPresenter(Inventory inventory, InventoryWidgetView view, AdditionalInputController input)
        {
            _inventory = inventory;
            _view = view;
            _items = new(_inventory.Capacity);
            _input = input;
            _inventory.ItemAdded += OnItemAdded;
            _inventory.ItemRemoved += OnItemRemoved;
            _inventory.ItemDropped += OnItemRemoved;
            _input.InventorySwitched += OnInventorySwitched;
        }

        public void Initialize()
        {
            _view.SetCapacity(_inventory.Capacity);
            ItemsChanged += _view.OnItemsChanged;
            SlotSelected += _view.OnSlotSelected;
        }

        public void Dispose()
        {
            _inventory.ItemAdded -= OnItemAdded;
            _inventory.ItemRemoved -= OnItemRemoved;
            _inventory.ItemDropped -= OnItemRemoved;
            ItemsChanged -= _view.OnItemsChanged;
            SlotSelected -= _view.OnSlotSelected;
            _input.InventorySwitched -= OnInventorySwitched;
            _input.InventorySelection -= OnSelection;
            _input.InventoryDrop -= OnDroped;
        }

        private void OnItemAdded(Inventory inventory, InventoryItem item)
        {
            _items.Add(item);
            ItemsChanged?.Invoke(_items);
        }

        private void OnItemRemoved(Inventory inventory, InventoryItem item)
        {
            _items.Remove(item);
            ItemsChanged?.Invoke(_items);
        }

        private void OnInventorySwitched(bool isOpen)
        {
            if (isOpen)
            {
                _view.Show();
                _input.InventorySelection += OnSelection; // Перенести в PostShow при анимации
                _input.InventoryDrop += OnDroped;
            }
            else
            {
                SlotSelected?.Invoke(-1);
                _selectedSlot = -1;
                _view.Hide();
                _input.InventorySelection -= OnSelection;
                _input.InventoryDrop -= OnDroped;
            }
        }

        private void OnSelection(bool isNext)
        {
            _selectedSlot = isNext ? (_selectedSlot + 1) % Capacity : (_selectedSlot <= 0 ? Capacity - 1 : _selectedSlot - 1);
            SlotSelected?.Invoke(_selectedSlot);
        }

        private void OnDroped()
        {
            if (_selectedSlot >= 0 && _selectedSlot < _items.Count)
            {
                _inventory.Drop(_items[_selectedSlot]);
            }
        }
    }
}